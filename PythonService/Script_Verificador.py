from flask import Flask, request, jsonify
from deepface import DeepFace
import base64
import tempfile
import os
import logging
import cv2
import numpy as np
from PIL import Image
import io
from concurrent.futures import ThreadPoolExecutor
import threading

# Configurar logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = Flask(__name__)

# Variables globales para modelo y threadpool
modelo = None
executor = ThreadPoolExecutor(max_workers=2)
modelo_lock = threading.Lock()

def inicializar_modelo():
    """Inicializa el modelo al arrancar la aplicación"""
    global modelo
    try:
        logger.info("Inicializando modelo ArcFace al arrancar...")
        modelo = DeepFace.build_model("ArcFace")
        logger.info("Modelo ArcFace cargado exitosamente")
        return True
    except Exception as e:
        logger.error(f"Error al cargar modelo: {e}")
        modelo = None
        return False

class FaceValidator:
    """Clase para validación estricta de rostros"""
    
    @staticmethod
    def validar_rostro_detectado(image_path):
        """Valida que haya al menos un rostro bien detectado"""
        try:
            # Usar múltiples backends para detección
            backends = ['opencv', 'retinaface', 'mtcnn']
            rostros_detectados = []
            
            for backend in backends:
                try:
                    faces = DeepFace.extract_faces(
                        img_path=image_path,
                        detector_backend=backend,
                        enforce_detection=False,
                        align=True
                    )
                    if faces and len(faces) > 0:
                        rostros_detectados.append({
                            'backend': backend,
                            'count': len(faces),
                            'faces': faces
                        })
                except Exception as e:
                    logger.debug(f"Backend {backend} falló: {e}")
                    continue
            
            if not rostros_detectados:
                return False, "No se detectaron rostros en la imagen"
            
            # Validar calidad de los rostros detectados
            mejor_deteccion = max(rostros_detectados, key=lambda x: x['count'])
            
            if mejor_deteccion['count'] > 1:
                return False, f"Se detectaron múltiples rostros ({mejor_deteccion['count']})"
            
            # Validar tamaño del rostro detectado
            face = mejor_deteccion['faces'][0]
            if face.shape[0] < 64 or face.shape[1] < 64:
                return False, "Rostro detectado muy pequeño"
            
            return True, {
                'backend_usado': mejor_deteccion['backend'],
                'rostros_detectados': mejor_deteccion['count'],
                'tamaño_rostro': face.shape
            }
            
        except Exception as e:
            logger.error(f"Error en validación de rostro: {e}")
            return False, f"Error en validación: {str(e)}"

    @staticmethod
    def calcular_similitud_facial(img1_path, img2_path):
        """Calcula similitud facial con múltiples métodos"""
        try:
            results = {}
            
            # Método 1: ArcFace (principal)
            try:
                result_arcface = DeepFace.verify(
                    img1_path=img1_path,
                    img2_path=img2_path,
                    model_name="ArcFace",
                    detector_backend="opencv",
                    enforce_detection=True,
                    align=True,
                    normalization="base"
                )
                results['arcface'] = {
                    'verified': result_arcface['verified'],
                    'distance': result_arcface['distance'],
                    'threshold': result_arcface['threshold']
                }
            except Exception as e:
                logger.warning(f"ArcFace falló: {e}")
                results['arcface'] = None
            
            # Método 2: Facenet (respaldo)
            try:
                result_facenet = DeepFace.verify(
                    img1_path=img1_path,
                    img2_path=img2_path,
                    model_name="Facenet",
                    detector_backend="opencv",
                    enforce_detection=True,
                    align=True,
                    normalization="base"
                )
                results['facenet'] = {
                    'verified': result_facenet['verified'],
                    'distance': result_facenet['distance'],
                    'threshold': result_facenet['threshold']
                }
            except Exception as e:
                logger.warning(f"Facenet falló: {e}")
                results['facenet'] = None
            
            # Método 3: VGG-Face (adicional)
            try:
                result_vgg = DeepFace.verify(
                    img1_path=img1_path,
                    img2_path=img2_path,
                    model_name="VGG-Face",
                    detector_backend="opencv",
                    enforce_detection=True,
                    align=True,
                    normalization="base"
                )
                results['vgg'] = {
                    'verified': result_vgg['verified'],
                    'distance': result_vgg['distance'],
                    'threshold': result_vgg['threshold']
                }
            except Exception as e:
                logger.warning(f"VGG-Face falló: {e}")
                results['vgg'] = None
            
            return results
            
        except Exception as e:
            logger.error(f"Error en cálculo de similitud: {e}")
            return None

    @staticmethod
    def decision_final_verificacion(results):
        """Toma decisión final basada en múltiples métodos"""
        if not results:
            return False, 0.0, "Error en verificación"
        
        # Contar métodos exitosos
        metodos_exitosos = []
        total_distance = 0.0
        
        for metodo, resultado in results.items():
            if resultado is not None:
                metodos_exitosos.append({
                    'metodo': metodo,
                    'verified': resultado['verified'],
                    'distance': resultado['distance'],
                    'threshold': resultado['threshold']
                })
                total_distance += resultado['distance']
        
        if not metodos_exitosos:
            return False, 0.0, "Ningún método de verificación funcionó"
        
        # Calcular estadísticas
        verificados = sum(1 for m in metodos_exitosos if m['verified'])
        total_metodos = len(metodos_exitosos)
        promedio_distance = total_distance / total_metodos
        
        # Lógica de decisión estricta
        if total_metodos == 1:
            # Solo un método funcionó - ser más estricto
            metodo = metodos_exitosos[0]
            # Reducir threshold para ser más estricto
            threshold_estricto = metodo['threshold'] * 0.85
            coincide = metodo['distance'] < threshold_estricto
            confianza = 1.0 - (metodo['distance'] / metodo['threshold'])
            
        elif total_metodos == 2:
            # Dos métodos - requiere al menos 2 coincidencias
            coincide = verificados >= 2
            confianza = verificados / total_metodos
            
        else:
            # Tres métodos - requiere mayoría (al menos 2)
            coincide = verificados >= 2
            confianza = verificados / total_metodos
        
        # Ajustar confianza basada en distancia promedio
        if coincide:
            # Penalizar distancias altas aunque estén bajo el threshold
            factor_distancia = max(0.1, 1.0 - (promedio_distance / 0.5))
            confianza = min(confianza, factor_distancia)
        
        detalle = {
            'metodos_exitosos': total_metodos,
            'verificados': verificados,
            'promedio_distance': promedio_distance,
            'detalles_metodos': metodos_exitosos
        }
        
        return coincide, confianza, detalle

class LivenessDetector:
    """Clase optimizada para detección de liveness SIN recorte de imagen"""
    
    @staticmethod
    def detectar_liveness_sin_recorte(image_path):
        """
        Detección de liveness SIN recortar las imágenes
        Procesamiento más estricto para evitar falsos positivos
        """
        try:
            img = cv2.imread(image_path)
            if img is None:
                return False, {"error": "No se pudo leer la imagen", "total_score": 0.0}
            
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            
            # Análisis mejorado con umbrales más estrictos
            brightness_score = LivenessDetector._analizar_brillo_contraste_mejorado(gray)
            reflection_score = LivenessDetector._detectar_reflexiones_mejorado(img)
            sharpness_score = LivenessDetector._calcular_nitidez_mejorado(gray)
            texture_score = LivenessDetector._analizar_textura_piel(gray)
            
            # Nuevos pesos más estrictos
            total_score = (
                brightness_score * 0.25 +
                reflection_score * 0.30 +
                sharpness_score * 0.25 +
                texture_score * 0.20
            )
            
            # Umbral más estricto para determinar si es real
            is_live = total_score > 0.65
            
            return is_live, {
                "total_score": float(total_score),
                "brightness_score": float(brightness_score),
                "reflection_score": float(reflection_score),
                "sharpness_score": float(sharpness_score),
                "texture_score": float(texture_score),
                "is_live": bool(is_live),
                "umbral_usado": 0.65
            }
            
        except Exception as e:
            logger.error(f"Error en detección de liveness: {e}")
            return False, {"error": str(e), "total_score": 0.0}

    @staticmethod
    def _analizar_brillo_contraste_mejorado(gray_img):
        """Análisis mejorado de brillo y contraste"""
        try:
            mean_brightness = float(np.mean(gray_img))
            std_brightness = float(np.std(gray_img))
            
            # Análisis de histograma
            hist = cv2.calcHist([gray_img], [0], None, [256], [0, 256])
            hist_norm = hist.flatten() / gray_img.size
            
            # Detección más estricta de patrones artificiales
            brightness_score = 1.0
            
            # Penalizar extremos más fuertemente
            if mean_brightness > 210 or mean_brightness < 50:
                brightness_score = 0.05
            elif mean_brightness > 190 or mean_brightness < 70:
                brightness_score = 0.2
            elif mean_brightness > 170 or mean_brightness < 90:
                brightness_score = 0.6
            
            # Contraste más estricto
            if std_brightness < 25:
                contrast_score = 0.1
            elif std_brightness < 35:
                contrast_score = 0.4
            else:
                contrast_score = min(1.0, std_brightness / 70.0)
            
            # Análisis de distribución más detallado
            entropy = -np.sum(hist_norm * np.log(hist_norm + 1e-10))
            
            # Detectar distribuciones muy uniformes (típicas de pantallas)
            if entropy < 5.5:
                entropy_score = 0.1
            elif entropy < 6.5:
                entropy_score = 0.5
            else:
                entropy_score = min(1.0, entropy / 7.5)
            
            return float((brightness_score + contrast_score + entropy_score) / 3)
        except:
            return 0.3

    @staticmethod
    def _detectar_reflexiones_mejorado(img):
        """Detección mejorada de reflexiones"""
        try:
            hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
            v_channel = hsv[:, :, 2]
            
            # Análisis más detallado de píxeles brillantes
            very_bright = int(np.sum(v_channel > 245))
            bright = int(np.sum(v_channel > 220))
            medium_bright = int(np.sum(v_channel > 190))
            total_pixels = int(v_channel.size)
            
            very_bright_ratio = float(very_bright / total_pixels)
            bright_ratio = float(bright / total_pixels)
            medium_bright_ratio = float(medium_bright / total_pixels)
            
            # Análisis de patrones geométricos (típicos de pantallas)
            _, thresh = cv2.threshold(v_channel, 235, 255, cv2.THRESH_BINARY)
            contours, _ = cv2.findContours(thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            
            # Detectar regiones rectangulares brillantes
            rectangular_regions = 0
            large_regions = 0
            
            for contour in contours:
                area = cv2.contourArea(contour)
                if area > 50:
                    large_regions += 1
                    
                    # Verificar si es rectangular (típico de pantallas)
                    epsilon = 0.02 * cv2.arcLength(contour, True)
                    approx = cv2.approxPolyDP(contour, epsilon, True)
                    if len(approx) == 4:
                        rectangular_regions += 1
            
            # Penalización más estricta
            if very_bright_ratio > 0.02 or rectangular_regions > 2:
                return 0.05
            elif bright_ratio > 0.08 or large_regions > 4:
                return 0.2
            elif medium_bright_ratio > 0.15 or large_regions > 2:
                return 0.5
            else:
                return 1.0
        except:
            return 0.3

    @staticmethod
    def _calcular_nitidez_mejorado(gray_img):
        """Cálculo mejorado de nitidez"""
        try:
            # Método 1: Varianza del Laplaciano mejorado
            laplacian = cv2.Laplacian(gray_img, cv2.CV_64F)
            laplacian_var = np.var(laplacian)
            
            # Método 2: Gradiente Sobel mejorado
            sobel_x = cv2.Sobel(gray_img, cv2.CV_64F, 1, 0, ksize=3)
            sobel_y = cv2.Sobel(gray_img, cv2.CV_64F, 0, 1, ksize=3)
            magnitude = np.sqrt(sobel_x**2 + sobel_y**2)
            sobel_mean = np.mean(magnitude)
            
            # Método 3: Análisis de bordes con Canny
            edges = cv2.Canny(gray_img, 50, 150)
            edge_ratio = np.sum(edges > 0) / edges.size
            
            # Umbrales más estrictos para nitidez
            laplacian_score = min(1.0, laplacian_var / 600.0)
            sobel_score = min(1.0, sobel_mean / 45.0)
            edge_score = min(1.0, edge_ratio * 15.0)
            
            # Si la nitidez es muy baja, penalizar fuertemente
            if laplacian_var < 100:
                laplacian_score = 0.1
            if sobel_mean < 15:
                sobel_score = 0.1
            if edge_ratio < 0.02:
                edge_score = 0.1
            
            return float((laplacian_score + sobel_score + edge_score) / 3)
        except:
            return 0.3

    @staticmethod
    def _analizar_textura_piel(gray_img):
        """Nuevo: Análisis de textura de piel real"""
        try:
            # Análisis de patrones locales binarios (LBP)
            def local_binary_pattern(img, radius=3, n_points=24):
                """Implementación simple de LBP"""
                h, w = img.shape
                lbp = np.zeros((h, w), dtype=np.uint8)
                
                for i in range(radius, h - radius):
                    for j in range(radius, w - radius):
                        center = img[i, j]
                        code = 0
                        for k in range(n_points):
                            angle = 2 * np.pi * k / n_points
                            x = int(i + radius * np.cos(angle))
                            y = int(j + radius * np.sin(angle))
                            if 0 <= x < h and 0 <= y < w:
                                if img[x, y] >= center:
                                    code |= (1 << k)
                        lbp[i, j] = code
                
                return lbp
            
            # Calcular LBP
            lbp = local_binary_pattern(gray_img, radius=2, n_points=16)
            
            # Histograma de patrones LBP
            hist_lbp = cv2.calcHist([lbp], [0], None, [256], [0, 256])
            hist_lbp_norm = hist_lbp.flatten() / lbp.size
            
            # Análisis de uniformidad (piel real tiene patrones más uniformes)
            uniformity = np.sum(hist_lbp_norm ** 2)
            
            # Análisis de varianza local
            kernel = np.ones((5, 5), np.float32) / 25
            local_mean = cv2.filter2D(gray_img.astype(np.float32), -1, kernel)
            local_var = cv2.filter2D((gray_img.astype(np.float32) - local_mean) ** 2, -1, kernel)
            avg_local_var = np.mean(local_var)
            
            # Puntaje de textura
            uniformity_score = min(1.0, uniformity * 20.0)
            variance_score = min(1.0, avg_local_var / 200.0)
            
            # Piel real tiene cierta uniformidad pero también varianza
            if uniformity < 0.02:  # Muy uniforme (posible pantalla)
                texture_score = 0.2
            elif avg_local_var < 50:  # Muy poca varianza (posible imagen procesada)
                texture_score = 0.3
            else:
                texture_score = (uniformity_score + variance_score) / 2
            
            return float(texture_score)
            
        except:
            return 0.5

def validar_imagen_completa(image_data):
    """Validación completa de imagen con análisis mejorado"""
    try:
        img = Image.open(io.BytesIO(image_data))
        
        if img.format not in ['JPEG', 'PNG', 'WEBP']:
            return False, "Formato no soportado"
        
        # Información detallada
        info = {
            "format": img.format,
            "size": [int(img.width), int(img.height)],
            "mode": img.mode,
            "megapixels": round((img.width * img.height) / 1000000, 2)
        }
        
        # Validaciones más estrictas
        warnings = []
        if img.width < 200 or img.height < 200:
            warnings.append("Imagen muy pequeña (recomendado: >200px)")
        if img.width > 3000 or img.height > 3000:
            warnings.append("Imagen muy grande")
        
        # Análisis de calidad básico
        img_array = np.array(img)
        if len(img_array.shape) == 3:
            # Verificar que no sea imagen muy oscura o muy clara
            mean_brightness = np.mean(img_array)
            if mean_brightness < 30:
                warnings.append("Imagen muy oscura")
            elif mean_brightness > 225:
                warnings.append("Imagen muy clara")
        
        info["warnings"] = warnings
        info["quality_check"] = {
            "brightness": float(mean_brightness) if 'mean_brightness' in locals() else None
        }
        
        return True, info
    except Exception as e:
        return False, f"Imagen corrupta: {str(e)}"

@app.route('/', methods=['GET'])
def health_check():
    model_status = "cargado" if modelo is not None else "no_cargado"
    return jsonify({
        "status": "ok",
        "service": "Face Verification Service - Mejorado",
        "version": "4.0.0",
        "modelo": model_status,
        "mejoras": [
            "Verificación con múltiples modelos",
            "Validación estricta de rostros",
            "Análisis de textura de piel",
            "Umbrales más estrictos",
            "Detección mejorada de falsos positivos"
        ]
    })

@app.route('/verificar', methods=['POST'])
def verificar():
    """Endpoint mejorado para verificación facial"""
    try:
        # Verificar que el modelo esté cargado
        if modelo is None:
            return jsonify({"error": "Modelo no disponible"}), 500
            
        data = request.get_json()
        if not data:
            return jsonify({"error": "No se recibieron datos"}), 400
            
        img1_b64 = data.get("img1")  # Imagen de referencia
        img2_b64 = data.get("img2")  # Imagen capturada

        if not img1_b64 or not img2_b64:
            return jsonify({"error": "Faltan imágenes"}), 400

        try:
            # Limpiar formato base64
            if img1_b64.startswith('data:image'):
                img1_b64 = img1_b64.split(',')[1]
            if img2_b64.startswith('data:image'):
                img2_b64 = img2_b64.split(',')[1]
                
            img1_data = base64.b64decode(img1_b64)
            img2_data = base64.b64decode(img2_b64)
        except:
            return jsonify({"error": "Error decodificando imágenes"}), 400

        # Validación completa en paralelo
        future1 = executor.submit(validar_imagen_completa, img1_data)
        future2 = executor.submit(validar_imagen_completa, img2_data)
        
        valid1, info1 = future1.result()
        valid2, info2 = future2.result()
        
        if not valid1 or not valid2:
            return jsonify({"error": "Imágenes no válidas"}), 400

        # Crear archivos temporales
        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f1, \
             tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f2:

            f1.write(img1_data)
            f2.write(img2_data)
            f1.flush()
            f2.flush()
            
            temp_files = [f1.name, f2.name]

            try:
                # 1. Validar rostros en ambas imágenes
                valid_face1, face_info1 = FaceValidator.validar_rostro_detectado(f1.name)
                valid_face2, face_info2 = FaceValidator.validar_rostro_detectado(f2.name)
                
                if not valid_face1:
                    return jsonify({
                        "resultado": "ERROR_IMAGEN_REFERENCIA",
                        "motivo": f"Imagen de referencia inválida: {face_info1}",
                        "info_imagenes": {"img1": info1, "img2": info2}
                    })
                
                if not valid_face2:
                    return jsonify({
                        "resultado": "ERROR_IMAGEN_CAPTURADA",
                        "motivo": f"Imagen capturada inválida: {face_info2}",
                        "info_imagenes": {"img1": info1, "img2": info2}
                    })
                
                # 2. Detección de liveness mejorada
                is_live, liveness_info = LivenessDetector.detectar_liveness_sin_recorte(f2.name)
                
                if not is_live:
                    return jsonify({
                        "resultado": "IMAGEN_SOSPECHOSA",
                        "motivo": "La imagen capturada no parece real",
                        "liveness_score": float(liveness_info.get("total_score", 0)),
                        "detalles": liveness_info,
                        "info_imagenes": {"img1": info1, "img2": info2}
                    })

                # 3. Verificación facial con múltiples métodos
                similarity_results = FaceValidator.calcular_similitud_facial(f1.name, f2.name)
                
                if not similarity_results:
                    return jsonify({
                        "resultado": "ERROR_VERIFICACION",
                        "motivo": "No se pudo realizar la verificación facial",
                        "info_imagenes": {"img1": info1, "img2": info2}
                    })

                # 4. Decisión final
                face_match, confidence, decision_details = FaceValidator.decision_final_verificacion(similarity_results)
                
                resultado_final = "COINCIDE" if face_match else "NO_COINCIDE"

                return jsonify({
                    "resultado": resultado_final,
                    "verificacion_facial": {
                        "coincide": bool(face_match),
                        "confianza": float(confidence),
                        "detalles": decision_details
                    },
                    "verificacion_liveness": {
                        "es_real": bool(is_live),
                        "score": float(liveness_info.get("total_score", 0)),
                        "detalles": liveness_info
                    },
                    "validacion_rostros": {
                        "img1": face_info1,
                        "img2": face_info2
                    },
                    "info_imagenes": {
                        "img1": info1,
                        "img2": info2
                    },
                    "version": "4.0.0_mejorado"
                })

            finally:
                # Limpiar archivos
                for temp_file in temp_files:
                    try:
                        os.unlink(temp_file)
                    except:
                        pass

    except Exception as e:
        logger.error(f"Error en verificación: {e}")
        return jsonify({"error": "Error interno del servidor"}), 500

@app.route('/test-liveness', methods=['POST'])
def test_liveness():
    """Test de liveness mejorado"""
    try:
        data = request.get_json()
        img_b64 = data.get("imagen")
        
        if not img_b64:
            return jsonify({"error": "Imagen requerida"}), 400

        if img_b64.startswith('data:image'):
            img_b64 = img_b64.split(',')[1]
        
        img_data = base64.b64decode(img_b64)
        
        # Validar imagen completa
        valid, info = validar_imagen_completa(img_data)
        if not valid:
            return jsonify({"error": f"Imagen inválida: {info}"}), 400
        
        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f:
            f.write(img_data)
            f.flush()
            
            # Validar rostro
            valid_face, face_info = FaceValidator.validar_rostro_detectado(f.name)
            if not valid_face:
                return jsonify({
                    "error": f"Rostro no válido: {face_info}",
                    "info_imagen": info
                })
            
            # Análisis de liveness
            is_live, liveness_info = LivenessDetector.detectar_liveness_sin_recorte(f.name)
            
            try:
                os.unlink(f.name)
            except:
                pass

        return jsonify({
            "es_real": bool(is_live),
            "score": float(liveness_info.get("total_score", 0)),
            "detalles": liveness_info,
            "validacion_rostro": face_info,
            "info_imagen": info,
            "version": "4.0.0_mejorado"
        })

    except Exception as e:
        logger.error(f"Error en test liveness: {e}")
        return jsonify({"error": "Error interno"}), 500

@app.route('/status', methods=['GET'])
def status():
    model_status = "cargado_y_listo" if modelo is not None else "error_no_cargado"
    
    return jsonify({
        "service": "Face Verification API - Mejorado",
        "version": "4.0.0",
        "status": "running",
        "modelo": model_status,
        "mejoras_v4": [
            "Verificación con múltiples modelos (ArcFace, Facenet, VGG-Face)",
            "Validación estricta de detección de rostros",
            "Análisis de textura de piel real",
            "Umbrales más estrictos para liveness",
            "Detección mejorada de patrones de pantalla",
            "Lógica de decisión basada en consenso de múltiples métodos",
            "Análisis de calidad de imagen mejorado",
            "Manejo de errores más robusto"
        ]
    })

@app.route('/test-face-detection', methods=['POST'])
def test_face_detection():
    """Endpoint para probar solo la detección de rostros"""
    try:
        data = request.get_json()
        img_b64 = data.get("imagen")
        
        if not img_b64:
            return jsonify({"error": "Imagen requerida"}), 400

        if img_b64.startswith('data:image'):
            img_b64 = img_b64.split(',')[1]
        
        img_data = base64.b64decode(img_b64)
        
        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f:
            f.write(img_data)
            f.flush()
            
            valid_face, face_info = FaceValidator.validar_rostro_detectado(f.name)
            
            try:
                os.unlink(f.name)
            except:
                pass

        return jsonify({
            "rostro_detectado": bool(valid_face),
            "detalles": face_info,
            "version": "4.0.0_mejorado"
        })

    except Exception as e:
        logger.error(f"Error en test detección: {e}")
        return jsonify({"error": "Error interno"}), 500

@app.route('/verificar-solo-rostros', methods=['POST'])
def verificar_solo_rostros():
    """Endpoint para verificar solo rostros sin liveness"""
    try:
        data = request.get_json()
        img1_b64 = data.get("img1")
        img2_b64 = data.get("img2")

        if not img1_b64 or not img2_b64:
            return jsonify({"error": "Faltan imágenes"}), 400

        # Decodificar imágenes
        if img1_b64.startswith('data:image'):
            img1_b64 = img1_b64.split(',')[1]
        if img2_b64.startswith('data:image'):
            img2_b64 = img2_b64.split(',')[1]
            
        img1_data = base64.b64decode(img1_b64)
        img2_data = base64.b64decode(img2_b64)

        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f1, \
             tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f2:

            f1.write(img1_data)
            f2.write(img2_data)
            f1.flush()
            f2.flush()
            
            temp_files = [f1.name, f2.name]

            try:
                # Validar rostros
                valid_face1, face_info1 = FaceValidator.validar_rostro_detectado(f1.name)
                valid_face2, face_info2 = FaceValidator.validar_rostro_detectado(f2.name)
                
                if not valid_face1 or not valid_face2:
                    return jsonify({
                        "resultado": "ERROR_ROSTROS",
                        "img1_valido": bool(valid_face1),
                        "img2_valido": bool(valid_face2),
                        "detalles": {
                            "img1": face_info1,
                            "img2": face_info2
                        }
                    })

                # Verificación facial
                similarity_results = FaceValidator.calcular_similitud_facial(f1.name, f2.name)
                face_match, confidence, decision_details = FaceValidator.decision_final_verificacion(similarity_results)
                
                resultado_final = "COINCIDE" if face_match else "NO_COINCIDE"

                return jsonify({
                    "resultado": resultado_final,
                    "coincide": bool(face_match),
                    "confianza": float(confidence),
                    "detalles_verificacion": decision_details,
                    "rostros_detectados": {
                        "img1": face_info1,
                        "img2": face_info2
                    }
                })

            finally:
                for temp_file in temp_files:
                    try:
                        os.unlink(temp_file)
                    except:
                        pass

    except Exception as e:
        logger.error(f"Error en verificación solo rostros: {e}")
        return jsonify({"error": "Error interno"}), 500

@app.route('/reload-model', methods=['POST'])
def reload_model():
    """Endpoint para recargar el modelo manualmente"""
    try:
        success = inicializar_modelo()
        if success:
            return jsonify({"message": "Modelo recargado exitosamente"})
        else:
            return jsonify({"error": "Error al recargar modelo"}), 500
    except Exception as e:
        return jsonify({"error": str(e)}), 500

# Inicializar modelo al arrancar
logger.info("Iniciando aplicación mejorada v4.0.0...")
modelo_cargado = inicializar_modelo()

if not modelo_cargado:
    logger.warning("⚠️  ADVERTENCIA: No se pudo cargar el modelo al inicio")
    logger.warning("⚠️  El servicio funcionará pero será más lento en la primera consulta")
else:
    logger.info("✅ Servicio listo con modelo precargado y mejoras v4.0.0")

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 5000))
    app.run(host="0.0.0.0", port=port, debug=False)
