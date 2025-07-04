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

def cargar_modelo():
    """Carga el modelo de forma lazy (solo cuando se necesita)"""
    global modelo
    if modelo is None:
        with modelo_lock:
            if modelo is None:  # Double-check locking
                try:
                    logger.info("Cargando modelo Facenet...")
                    modelo = DeepFace.build_model("Facenet")
                    logger.info("Modelo cargado exitosamente")
                except Exception as e:
                    logger.error(f"Error al cargar modelo: {e}")
                    raise e
    return modelo

class FaceValidator:
    """Clase para validaciones adicionales de rostros"""
    
    @staticmethod
    def validar_deteccion_facial(image_path):
        """Valida que realmente se detecte un rostro con buena confianza"""
        try:
            # Usar OpenCV para detectar rostros primero
            img = cv2.imread(image_path)
            if img is None:
                return False, "No se pudo leer la imagen"
            
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            
            # Detector de rostros Haar Cascade (más estricto)
            face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')
            faces = face_cascade.detectMultiScale(gray, 1.1, 4, minSize=(50, 50))
            
            if len(faces) == 0:
                return False, "No se detectó ningún rostro"
            elif len(faces) > 1:
                return False, "Se detectaron múltiples rostros"
            
            # Validar que el rostro ocupe un área mínima significativa
            x, y, w, h = faces[0]
            face_area = w * h
            total_area = img.shape[0] * img.shape[1]
            face_ratio = face_area / total_area
            
            if face_ratio < 0.05:  # El rostro debe ocupar al menos 5% de la imagen
                return False, "Rostro demasiado pequeño en la imagen"
            
            return True, {
                "faces_detected": len(faces),
                "face_area_ratio": float(face_ratio),
                "face_coords": [int(x), int(y), int(w), int(h)]
            }
            
        except Exception as e:
            logger.error(f"Error en validación facial: {e}")
            return False, str(e)

    @staticmethod
    def validar_calidad_imagen(image_path):
        """Valida la calidad de la imagen para reconocimiento"""
        try:
            img = cv2.imread(image_path)
            if img is None:
                return False, "Imagen no válida"
            
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            
            # 1. Verificar nitidez (imágenes borrosas causan falsos positivos)
            laplacian_var = cv2.Laplacian(gray, cv2.CV_64F).var()
            if laplacian_var < 100:  # Muy borrosa
                return False, "Imagen muy borrosa"
            
            # 2. Verificar brillo (imágenes muy oscuras/claras causan problemas)
            mean_brightness = np.mean(gray)
            if mean_brightness < 30 or mean_brightness > 220:
                return False, "Imagen muy oscura o muy clara"
            
            # 3. Verificar contraste
            contrast = np.std(gray)
            if contrast < 15:
                return False, "Imagen con muy poco contraste"
            
            return True, {
                "sharpness": float(laplacian_var),
                "brightness": float(mean_brightness),
                "contrast": float(contrast)
            }
            
        except Exception as e:
            return False, str(e)

class LivenessDetector:
    """Clase optimizada para detección de liveness"""
    
    @staticmethod
    def detectar_liveness_rapido(image_path):
        """
        Versión optimizada de detección de liveness
        Solo aplica los filtros más efectivos para reducir tiempo de procesamiento
        """
        try:
            img = cv2.imread(image_path)
            if img is None:
                return False, {"error": "No se pudo leer la imagen", "total_score": 0.0}
            
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            
            # Solo usar los 3 filtros más efectivos para optimizar velocidad
            # 1. Análisis de brillo y contraste (más rápido y efectivo)
            brightness_score = LivenessDetector._analizar_brillo_contraste_rapido(gray)
            
            # 2. Detección de reflexiones especulares (detecta pantallas)
            reflection_score = LivenessDetector._detectar_reflexiones_rapido(img)
            
            # 3. Análisis de nitidez (detecta fotos impresas)
            sharpness_score = LivenessDetector._calcular_nitidez_rapido(gray)
            
            # Pesos ajustados para los 3 filtros principales
            total_score = (
                brightness_score * 0.4 +
                reflection_score * 0.35 +
                sharpness_score * 0.25
            )
            
            # Umbral más estricto para liveness
            is_live = total_score > 0.65  # Aumentado de 0.55 a 0.65
            
            return is_live, {
                "total_score": float(total_score),
                "brightness_score": float(brightness_score),
                "reflection_score": float(reflection_score),
                "sharpness_score": float(sharpness_score),
                "is_live": bool(is_live)
            }
            
        except Exception as e:
            logger.error(f"Error en detección de liveness: {e}")
            return False, {"error": str(e), "total_score": 0.0}

    @staticmethod
    def _analizar_brillo_contraste_rapido(gray_img):
        """Versión optimizada del análisis de brillo y contraste"""
        try:
            # Usar subsample para imágenes grandes (más rápido)
            h, w = gray_img.shape
            if h > 400 or w > 400:
                factor = max(h//400, w//400)
                gray_img = gray_img[::factor, ::factor]
            
            mean_brightness = float(np.mean(gray_img))
            std_brightness = float(np.std(gray_img))
            
            # Detección más agresiva de pantallas
            brightness_score = 1.0
            if mean_brightness > 220 or mean_brightness < 40:  # Pantallas muy brillantes u oscuras
                brightness_score = 0.1
            elif mean_brightness > 180 or mean_brightness < 70:
                brightness_score = 0.4
            
            # Contraste bajo indica pantalla
            contrast_score = min(1.0, std_brightness / 80.0)
            if std_brightness < 20:  # Muy poco contraste
                contrast_score = 0.2
            
            return float((brightness_score + contrast_score) / 2)
        except:
            return 0.5

    @staticmethod
    def _detectar_reflexiones_rapido(img):
        """Detección optimizada de reflexiones de pantalla"""
        try:
            # Subsample para velocidad
            h, w = img.shape[:2]
            if h > 300 or w > 300:
                factor = max(h//300, w//300)
                img = img[::factor, ::factor]
            
            # Convertir a HSV solo el canal V
            hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
            v_channel = hsv[:, :, 2]
            
            # Detectar píxeles excesivamente brillantes (reflexiones de pantalla)
            very_bright = int(np.sum(v_channel > 250))
            bright = int(np.sum(v_channel > 230))
            total_pixels = int(v_channel.size)
            
            very_bright_ratio = float(very_bright / total_pixels)
            bright_ratio = float(bright / total_pixels)
            
            # Penalización más severa para reflexiones
            if very_bright_ratio > 0.03:  # Más del 3% muy brillante = pantalla
                return 0.1
            elif bright_ratio > 0.15:  # Más del 15% brillante = sospechoso
                return 0.3
            elif bright_ratio > 0.08:
                return 0.6
            else:
                return 1.0
        except:
            return 0.5

    @staticmethod
    def _calcular_nitidez_rapido(gray_img):
        """Cálculo optimizado de nitidez"""
        try:
            # Subsample para velocidad
            h, w = gray_img.shape
            if h > 400 or w > 400:
                factor = max(h//400, w//400)
                gray_img = gray_img[::factor, ::factor]
            
            # Usar Sobel en lugar de Laplaciano (más rápido)
            sobel_x = cv2.Sobel(gray_img, cv2.CV_64F, 1, 0, ksize=3)
            sobel_y = cv2.Sobel(gray_img, cv2.CV_64F, 0, 1, ksize=3)
            
            # Magnitud del gradiente
            magnitude = np.sqrt(sobel_x**2 + sobel_y**2)
            sharpness = np.mean(magnitude)
            
            # Normalizar (fotos impresas tienen menos nitidez)
            normalized_sharpness = min(1.0, sharpness / 30.0)
            
            return normalized_sharpness
        except:
            return 0.5

def validar_imagen_basica(image_data):
    """Validación básica optimizada de imagen"""
    try:
        img = Image.open(io.BytesIO(image_data))
        
        if img.format not in ['JPEG', 'PNG', 'WEBP']:
            return False, "Formato no soportado"
        
        warning = None
        if img.width < 200 or img.height < 200:  # Aumentado requisito mínimo
            warning = "Imagen muy pequeña para reconocimiento preciso"
        
        return True, {
            "format": img.format,
            "size": [int(img.width), int(img.height)],
            "warning": warning
        }
    except:
        return False, "Imagen corrupta"

@app.route('/', methods=['GET'])
def health_check():
    return jsonify({
        "status": "ok",
        "service": "Optimized Face Verification Service",
        "version": "2.2.0"
    })

@app.route('/verificar', methods=['POST'])
def verificar():
    """Endpoint optimizado para verificación facial con validaciones estrictas"""
    try:
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

        # Validación básica en paralelo
        future1 = executor.submit(validar_imagen_basica, img1_data)
        future2 = executor.submit(validar_imagen_basica, img2_data)
        
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
                # VALIDACIONES ADICIONALES PARA REDUCIR FALSOS POSITIVOS
                
                # 1. Validar detección facial en ambas imágenes
                face_valid1, face_info1 = FaceValidator.validar_deteccion_facial(f1.name)
                face_valid2, face_info2 = FaceValidator.validar_deteccion_facial(f2.name)
                
                if not face_valid1:
                    return jsonify({
                        "resultado": "ERROR_IMAGEN_REFERENCIA",
                        "motivo": f"Imagen de referencia: {face_info1}",
                        "codigo": "FACE_NOT_DETECTED_REF"
                    }), 400
                
                if not face_valid2:
                    return jsonify({
                        "resultado": "ERROR_IMAGEN_CAPTURADA",
                        "motivo": f"Imagen capturada: {face_info2}",
                        "codigo": "FACE_NOT_DETECTED_CAP"
                    }), 400
                
                # 2. Validar calidad de ambas imágenes
                quality_valid1, quality_info1 = FaceValidator.validar_calidad_imagen(f1.name)
                quality_valid2, quality_info2 = FaceValidator.validar_calidad_imagen(f2.name)
                
                if not quality_valid1:
                    return jsonify({
                        "resultado": "ERROR_CALIDAD_REFERENCIA",
                        "motivo": f"Imagen de referencia: {quality_info1}",
                        "codigo": "QUALITY_LOW_REF"
                    }), 400
                
                if not quality_valid2:
                    return jsonify({
                        "resultado": "ERROR_CALIDAD_CAPTURADA",
                        "motivo": f"Imagen capturada: {quality_info2}",
                        "codigo": "QUALITY_LOW_CAP"
                    }), 400
                
                # 3. Liveness detection solo para imagen capturada
                is_live, liveness_info = LivenessDetector.detectar_liveness_rapido(f2.name)
                
                if not is_live:
                    return jsonify({
                        "resultado": "IMAGEN_SOSPECHOSA",
                        "motivo": "La imagen capturada no parece real",
                        "liveness_score": float(liveness_info.get("total_score", 0)),
                        "detalles": liveness_info,
                        "codigo": "LIVENESS_FAILED"
                    })

                # 4. Verificación facial con configuraciones más estrictas
                try:
                    # Usar múltiples modelos para mayor precisión
                    models_to_try = ["ArcFace", "Facenet", "VGG-Face"]
                    results = []
                    
                    for model_name in models_to_try:
                        try:
                            result = DeepFace.verify(
                                img1_path=f1.name,
                                img2_path=f2.name,
                                model_name=model_name,
                                enforce_detection=True,  # ¡IMPORTANTE! Cambiar a True
                                distance_metric="cosine"
                            )
                            results.append({
                                "model": model_name,
                                "verified": result["verified"],
                                "distance": result["distance"],
                                "threshold": result["threshold"]
                            })
                        except Exception as e:
                            logger.warning(f"Error con modelo {model_name}: {e}")
                            continue
                    
                    if not results:
                        return jsonify({
                            "resultado": "ERROR_VERIFICACION",
                            "motivo": "No se pudo procesar ningún modelo de verificación",
                            "codigo": "MODEL_PROCESSING_ERROR"
                        }), 500
                    
                    # Análisis de consenso entre modelos
                    verified_count = sum(1 for r in results if r["verified"])
                    total_models = len(results)
                    
                    # Requerir consenso de al menos 2 modelos o 66% de acuerdo
                    consensus_threshold = max(2, int(total_models * 0.66))
                    face_match = verified_count >= consensus_threshold
                    
                    # Calcular distancia promedio
                    avg_distance = sum(r["distance"] for r in results) / len(results)
                    
                    # Threshold más estricto para distancia
                    strict_distance_threshold = 0.4  # Ajustar según necesidades
                    distance_check = avg_distance < strict_distance_threshold
                    
                    # Resultado final: debe pasar tanto consenso como distancia
                    final_match = face_match and distance_check
                    
                    resultado_final = "COINCIDE" if final_match else "NO_COINCIDE"
                    
                    return jsonify({
                        "resultado": resultado_final,
                        "verificacion_facial": {
                            "coincide": bool(final_match),
                            "consenso_modelos": f"{verified_count}/{total_models}",
                            "distancia_promedio": float(avg_distance),
                            "threshold_estricto": float(strict_distance_threshold),
                            "paso_consenso": bool(face_match),
                            "paso_distancia": bool(distance_check),
                            "resultados_modelos": results
                        },
                        "verificacion_liveness": {
                            "es_real": bool(is_live),
                            "score": float(liveness_info.get("total_score", 0)),
                            "detalles": liveness_info
                        },
                        "validaciones_adicionales": {
                            "rostro_referencia": face_info1,
                            "rostro_capturado": face_info2,
                            "calidad_referencia": quality_info1,
                            "calidad_capturada": quality_info2
                        }
                    })
                    
                except Exception as e:
                    logger.error(f"Error en verificación facial: {e}")
                    return jsonify({
                        "resultado": "ERROR_VERIFICACION",
                        "motivo": "Error en el proceso de verificación facial",
                        "codigo": "VERIFICATION_PROCESS_ERROR"
                    }), 500

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
    """Test solo de liveness para imagen capturada"""
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
            
            # Validaciones adicionales para test
            face_valid, face_info = FaceValidator.validar_deteccion_facial(f.name)
            quality_valid, quality_info = FaceValidator.validar_calidad_imagen(f.name)
            is_live, liveness_info = LivenessDetector.detectar_liveness_rapido(f.name)
            
            try:
                os.unlink(f.name)
            except:
                pass

        return jsonify({
            "es_real": bool(is_live),
            "score": float(liveness_info.get("total_score", 0)),
            "detalles": liveness_info,
            "validaciones": {
                "rostro_detectado": bool(face_valid),
                "info_rostro": face_info,
                "calidad_ok": bool(quality_valid),
                "info_calidad": quality_info
            }
        })

    except Exception as e:
        logger.error(f"Error en test liveness: {e}")
        return jsonify({"error": "Error interno"}), 500

@app.route('/status', methods=['GET'])
def status():
    return jsonify({
        "service": "Optimized Face Verification API",
        "version": "2.2.0",
        "status": "running",
        "improvements": [
            "Strict face detection validation",
            "Image quality validation",
            "Multi-model consensus verification",
            "Enforce detection enabled",
            "Stricter distance thresholds",
            "Enhanced error reporting"
        ]
    })

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 5000))
    app.run(host="0.0.0.0", port=port, debug=False)
