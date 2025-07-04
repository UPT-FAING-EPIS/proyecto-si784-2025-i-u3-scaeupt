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

class LivenessDetector:
    """Clase optimizada para detección de liveness SIN recorte de imagen"""
    
    @staticmethod
    def detectar_liveness_sin_recorte(image_path):
        """
        Detección de liveness SIN recortar las imágenes
        Procesa la imagen completa manteniendo toda la información
        """
        try:
            img = cv2.imread(image_path)
            if img is None:
                return False, {"error": "No se pudo leer la imagen", "total_score": 0.0}
            
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            
            # Análisis completo SIN recorte
            brightness_score = LivenessDetector._analizar_brillo_contraste_completo(gray)
            reflection_score = LivenessDetector._detectar_reflexiones_completo(img)
            sharpness_score = LivenessDetector._calcular_nitidez_completo(gray)
            
            # Pesos para los 3 filtros principales
            total_score = (
                brightness_score * 0.4 +
                reflection_score * 0.35 +
                sharpness_score * 0.25
            )
            
            # Umbral para determinar si es real
            is_live = total_score > 0.55
            
            return is_live, {
                "total_score": float(total_score),
                "brightness_score": float(brightness_score),
                "reflection_score": float(reflection_score),
                "sharpness_score": float(sharpness_score),
                "is_live": bool(is_live),
                "imagen_procesada": "completa_sin_recorte"
            }
            
        except Exception as e:
            logger.error(f"Error en detección de liveness: {e}")
            return False, {"error": str(e), "total_score": 0.0}

    @staticmethod
    def _analizar_brillo_contraste_completo(gray_img):
        """Análisis de brillo y contraste SIN recorte"""
        try:
            # Procesar imagen completa
            mean_brightness = float(np.mean(gray_img))
            std_brightness = float(np.std(gray_img))
            
            # Análisis de histograma completo
            hist = cv2.calcHist([gray_img], [0], None, [256], [0, 256])
            hist_norm = hist.flatten() / gray_img.size
            
            # Detección de patrones de pantalla
            brightness_score = 1.0
            if mean_brightness > 220 or mean_brightness < 40:
                brightness_score = 0.1
            elif mean_brightness > 180 or mean_brightness < 70:
                brightness_score = 0.4
            
            # Análisis de contraste mejorado
            contrast_score = min(1.0, std_brightness / 80.0)
            if std_brightness < 20:
                contrast_score = 0.2
            
            # Análisis de distribución de intensidades
            # Detectar distribuciones artificiales típicas de pantallas
            entropy = -np.sum(hist_norm * np.log(hist_norm + 1e-10))
            entropy_score = min(1.0, entropy / 7.0)  # Normalizar entropia
            
            return float((brightness_score + contrast_score + entropy_score) / 3)
        except:
            return 0.5

    @staticmethod
    def _detectar_reflexiones_completo(img):
        """Detección de reflexiones SIN recorte"""
        try:
            # Procesar imagen completa
            hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
            v_channel = hsv[:, :, 2]
            
            # Análisis completo de píxeles brillantes
            very_bright = int(np.sum(v_channel > 250))
            bright = int(np.sum(v_channel > 230))
            medium_bright = int(np.sum(v_channel > 200))
            total_pixels = int(v_channel.size)
            
            very_bright_ratio = float(very_bright / total_pixels)
            bright_ratio = float(bright / total_pixels)
            medium_bright_ratio = float(medium_bright / total_pixels)
            
            # Análisis de patrones de reflexión
            # Detectar regiones conectadas brillantes (típicas de pantallas)
            _, thresh = cv2.threshold(v_channel, 240, 255, cv2.THRESH_BINARY)
            contours, _ = cv2.findContours(thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            
            large_bright_regions = sum(1 for c in contours if cv2.contourArea(c) > 100)
            
            # Penalización por reflexiones
            if very_bright_ratio > 0.03 or large_bright_regions > 5:
                return 0.1
            elif bright_ratio > 0.15 or large_bright_regions > 3:
                return 0.3
            elif medium_bright_ratio > 0.25:
                return 0.6
            else:
                return 1.0
        except:
            return 0.5

    @staticmethod
    def _calcular_nitidez_completo(gray_img):
        """Cálculo de nitidez SIN recorte"""
        try:
            # Procesar imagen completa
            # Usar múltiples métodos para mejor precisión
            
            # Método 1: Varianza del Laplaciano
            laplacian = cv2.Laplacian(gray_img, cv2.CV_64F)
            laplacian_var = np.var(laplacian)
            
            # Método 2: Gradiente Sobel
            sobel_x = cv2.Sobel(gray_img, cv2.CV_64F, 1, 0, ksize=3)
            sobel_y = cv2.Sobel(gray_img, cv2.CV_64F, 0, 1, ksize=3)
            magnitude = np.sqrt(sobel_x**2 + sobel_y**2)
            sobel_mean = np.mean(magnitude)
            
            # Método 3: Análisis de frecuencias altas
            f_transform = np.fft.fft2(gray_img)
            f_shift = np.fft.fftshift(f_transform)
            magnitude_spectrum = np.abs(f_shift)
            
            # Analizar frecuencias altas (bordes y detalles)
            h, w = gray_img.shape
            center_h, center_w = h // 2, w // 2
            high_freq_mask = np.zeros((h, w), dtype=np.uint8)
            cv2.circle(high_freq_mask, (center_w, center_h), min(h, w) // 4, 255, -1)
            high_freq_mask = 255 - high_freq_mask  # Invertir para obtener frecuencias altas
            
            high_freq_energy = np.sum(magnitude_spectrum * (high_freq_mask / 255.0))
            total_energy = np.sum(magnitude_spectrum)
            freq_ratio = high_freq_energy / (total_energy + 1e-10)
            
            # Combinar métricas
            laplacian_score = min(1.0, laplacian_var / 500.0)
            sobel_score = min(1.0, sobel_mean / 40.0)
            freq_score = min(1.0, freq_ratio * 10.0)
            
            # Promedio ponderado
            sharpness_final = (laplacian_score * 0.4 + sobel_score * 0.4 + freq_score * 0.2)
            
            return float(sharpness_final)
        except:
            return 0.5

def validar_imagen_completa(image_data):
    """Validación completa de imagen SIN recorte"""
    try:
        img = Image.open(io.BytesIO(image_data))
        
        if img.format not in ['JPEG', 'PNG', 'WEBP']:
            return False, "Formato no soportado"
        
        # Información detallada SIN limitaciones de tamaño
        info = {
            "format": img.format,
            "size": [int(img.width), int(img.height)],
            "mode": img.mode,
            "megapixels": round((img.width * img.height) / 1000000, 2)
        }
        
        # Solo advertencias, no limitaciones
        warnings = []
        if img.width < 150 or img.height < 150:
            warnings.append("Imagen muy pequeña (recomendado: >150px)")
        if img.width > 4000 or img.height > 4000:
            warnings.append("Imagen muy grande (puede ser lenta)")
        
        info["warnings"] = warnings
        
        return True, info
    except:
        return False, "Imagen corrupta"

@app.route('/', methods=['GET'])
def health_check():
    model_status = "cargado" if modelo is not None else "no_cargado"
    return jsonify({
        "status": "ok",
        "service": "Face Verification Service - Sin Recorte",
        "version": "3.0.0",
        "modelo": model_status,
        "optimizaciones": [
            "Modelo precargado al inicio",
            "Procesamiento de imagen completa",
            "Sin recorte de imagen",
            "Análisis de liveness mejorado"
        ]
    })

@app.route('/verificar', methods=['POST'])
def verificar():
    """Endpoint para verificación facial SIN recorte"""
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
                # Liveness detection SIN recorte en img2
                is_live, liveness_info = LivenessDetector.detectar_liveness_sin_recorte(f2.name)
                
                if not is_live:
                    return jsonify({
                        "resultado": "IMAGEN_SOSPECHOSA",
                        "motivo": "La imagen capturada no parece real",
                        "liveness_score": float(liveness_info.get("total_score", 0)),
                        "detalles": liveness_info,
                        "info_imagenes": {
                            "img1": info1,
                            "img2": info2
                        }
                    })

                # Verificación facial con modelo precargado
                result = DeepFace.verify(
                    img1_path=f1.name,
                    img2_path=f2.name,
                    model_name="ArcFace",
                    enforce_detection=False
                )

                # Determinar resultado
                face_match = bool(result["verified"])
                confidence = float(result.get("distance", 0))
                
                resultado_final = "COINCIDE" if face_match else "NO_COINCIDE"

                return jsonify({
                    "resultado": resultado_final,
                    "verificacion_facial": {
                        "coincide": bool(face_match),
                        "distancia": float(confidence),
                        "threshold": float(result.get("threshold", 0))
                    },
                    "verificacion_liveness": {
                        "es_real": bool(is_live),
                        "score": float(liveness_info.get("total_score", 0)),
                        "detalles": liveness_info
                    },
                    "info_imagenes": {
                        "img1": info1,
                        "img2": info2
                    },
                    "procesamiento": "imagen_completa_sin_recorte"
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
        return jsonify({"error": "Error interno"}), 500

@app.route('/test-liveness', methods=['POST'])
def test_liveness():
    """Test de liveness SIN recorte"""
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
            
            is_live, liveness_info = LivenessDetector.detectar_liveness_sin_recorte(f.name)
            
            try:
                os.unlink(f.name)
            except:
                pass

        return jsonify({
            "es_real": bool(is_live),
            "score": float(liveness_info.get("total_score", 0)),
            "detalles": liveness_info,
            "info_imagen": info,
            "procesamiento": "imagen_completa_sin_recorte"
        })

    except Exception as e:
        logger.error(f"Error en test liveness: {e}")
        return jsonify({"error": "Error interno"}), 500

@app.route('/status', methods=['GET'])
def status():
    model_status = "cargado_y_listo" if modelo is not None else "error_no_cargado"
    
    return jsonify({
        "service": "Face Verification API - Sin Recorte",
        "version": "3.0.0",
        "status": "running",
        "modelo": model_status,
        "optimizaciones": [
            "Modelo ArcFace precargado al inicio",
            "Procesamiento de imagen completa (sin recorte)",
            "Análisis de liveness mejorado",
            "Validación completa en paralelo",
            "Análisis de frecuencias para nitidez",
            "Detección de patrones de reflexión avanzada"
        ]
    })

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
logger.info("Iniciando aplicación...")
modelo_cargado = inicializar_modelo()

if not modelo_cargado:
    logger.warning("⚠️  ADVERTENCIA: No se pudo cargar el modelo al inicio")
    logger.warning("⚠️  El servicio funcionará pero será más lento en la primera consulta")
else:
    logger.info("✅ Servicio listo con modelo precargado")

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 5000))
    app.run(host="0.0.0.0", port=port, debug=False)
