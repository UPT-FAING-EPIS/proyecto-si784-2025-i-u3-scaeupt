from flask import Flask, request, jsonify
from deepface import DeepFace
import base64
import tempfile
import os
import logging
import cv2
import numpy as np
from PIL import Image, ImageEnhance
import io

# Configurar logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = Flask(__name__)

# Cargar modelo una sola vez al iniciar la aplicación
logger.info("Cargando modelo Facenet...")
try:
    modelo = DeepFace.build_model("Facenet")
    logger.info("Modelo cargado exitosamente")
except Exception as e:
    logger.error(f"Error al cargar modelo: {e}")
    modelo = None

def detectar_liveness(image_path):
    """
    Detecta si una imagen es 'viva' (foto real) o 'falsa' (pantalla, impresa)
    usando múltiples técnicas de anti-spoofing
    """
    try:
        # Leer imagen
        img = cv2.imread(image_path)
        if img is None:
            return False, "No se pudo leer la imagen"
        
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        
        # 1. Análisis de textura usando Variación Local Binaria (LBP)
        texture_score = calcular_textura_lbp(gray)
        
        # 2. Análisis de brillo y contraste
        brightness_score = analizar_brillo_contraste(gray)
        
        # 3. Detección de bordes y nitidez
        sharpness_score = calcular_nitidez(gray)
        
        # 4. Análisis de reflexión especular
        reflection_score = detectar_reflexiones(img)
        
        # 5. Análisis de calidad de imagen
        quality_score = analizar_calidad_imagen(img)
        
        # Calcular puntuación total
        total_score = (
            texture_score * 0.25 +
            brightness_score * 0.20 +
            sharpness_score * 0.20 +
            reflection_score * 0.20 +
            quality_score * 0.15
        )
        
        # Umbral para determinar si es imagen viva
        is_live = total_score > 0.6
        
        return is_live, {
            "total_score": total_score,
            "texture_score": texture_score,
            "brightness_score": brightness_score,
            "sharpness_score": sharpness_score,
            "reflection_score": reflection_score,
            "quality_score": quality_score,
            "is_live": is_live
        }
        
    except Exception as e:
        logger.error(f"Error en detección de liveness: {e}")
        return False, f"Error: {e}"

def calcular_textura_lbp(gray_img):
    """Calcula la textura usando Local Binary Pattern"""
    try:
        # Implementación simple de LBP
        rows, cols = gray_img.shape
        lbp = np.zeros((rows-2, cols-2), dtype=np.uint8)
        
        for i in range(1, rows-1):
            for j in range(1, cols-1):
                center = gray_img[i, j]
                code = 0
                code |= (gray_img[i-1, j-1] >= center) << 7
                code |= (gray_img[i-1, j] >= center) << 6
                code |= (gray_img[i-1, j+1] >= center) << 5
                code |= (gray_img[i, j+1] >= center) << 4
                code |= (gray_img[i+1, j+1] >= center) << 3
                code |= (gray_img[i+1, j] >= center) << 2
                code |= (gray_img[i+1, j-1] >= center) << 1
                code |= (gray_img[i, j-1] >= center) << 0
                lbp[i-1, j-1] = code
        
        # Calcular histograma y uniformidad
        hist = cv2.calcHist([lbp], [0], None, [256], [0, 256])
        uniformity = np.sum(hist ** 2) / (lbp.size ** 2)
        
        # Normalizar score (mayor uniformidad = menor probabilidad de ser real)
        return min(1.0, 1.0 - uniformity * 10)
        
    except:
        return 0.5

def analizar_brillo_contraste(gray_img):
    """Analiza brillo y contraste para detectar pantallas"""
    try:
        mean_brightness = np.mean(gray_img)
        std_brightness = np.std(gray_img)
        
        # Las fotos de pantalla tienden a tener brillo muy alto o muy bajo
        # y menor variación de contraste
        brightness_score = 1.0
        if mean_brightness > 200 or mean_brightness < 50:
            brightness_score *= 0.3
        
        contrast_score = min(1.0, std_brightness / 100.0)
        
        return (brightness_score + contrast_score) / 2
        
    except:
        return 0.5

def calcular_nitidez(gray_img):
    """Calcula la nitidez de la imagen"""
    try:
        # Usar operador Laplaciano para detectar bordes
        laplacian = cv2.Laplacian(gray_img, cv2.CV_64F)
        sharpness = np.var(laplacian)
        
        # Normalizar (imágenes de pantalla suelen ser menos nítidas)
        normalized_sharpness = min(1.0, sharpness / 1000.0)
        
        return normalized_sharpness
        
    except:
        return 0.5

def detectar_reflexiones(img):
    """Detecta reflexiones especulares típicas de pantallas"""
    try:
        # Convertir a HSV para mejor detección de brillo
        hsv = cv2.cvtColor(img, cv2.COLOR_BGR2HSV)
        v_channel = hsv[:, :, 2]
        
        # Detectar píxeles muy brillantes (posibles reflexiones)
        bright_pixels = np.sum(v_channel > 240)
        total_pixels = v_channel.size
        
        reflection_ratio = bright_pixels / total_pixels
        
        # Penalizar imágenes con muchas reflexiones
        if reflection_ratio > 0.05:  # Más del 5% de píxeles muy brillantes
            return 0.2
        elif reflection_ratio > 0.02:  # Entre 2-5%
            return 0.6
        else:
            return 1.0
            
    except:
        return 0.5

def analizar_calidad_imagen(img):
    """Analiza la calidad general de la imagen"""
    try:
        # Calcular métricas de calidad
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        
        # 1. Detectar compresión JPEG excesiva
        jpeg_quality = estimar_calidad_jpeg(img)
        
        # 2. Analizar ruido
        noise_level = calcular_ruido(gray)
        
        # 3. Resolución efectiva
        resolution_score = min(1.0, (img.shape[0] * img.shape[1]) / (640 * 480))
        
        quality_score = (jpeg_quality + (1.0 - noise_level) + resolution_score) / 3
        
        return quality_score
        
    except:
        return 0.5

def estimar_calidad_jpeg(img):
    """Estima la calidad JPEG de la imagen"""
    try:
        # Convertir a escala de grises
        gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
        
        # Calcular gradientes
        grad_x = cv2.Sobel(gray, cv2.CV_64F, 1, 0, ksize=3)
        grad_y = cv2.Sobel(gray, cv2.CV_64F, 0, 1, ksize=3)
        
        # Calcular magnitud del gradiente
        magnitude = np.sqrt(grad_x**2 + grad_y**2)
        
        # Estimar calidad basada en la suavidad de gradientes
        quality = 1.0 - min(1.0, np.std(magnitude) / 50.0)
        
        return quality
        
    except:
        return 0.5

def calcular_ruido(gray_img):
    """Calcula el nivel de ruido en la imagen"""
    try:
        # Usar desviación estándar del Laplaciano como medida de ruido
        laplacian = cv2.Laplacian(gray_img, cv2.CV_64F)
        noise_level = np.std(laplacian) / 100.0
        
        return min(1.0, noise_level)
        
    except:
        return 0.5


def validar_metadata_imagen(image_data):
    """Valida metadatos de la imagen para detectar manipulación"""
    try:
        # Convertir bytes a PIL Image
        img = Image.open(io.BytesIO(image_data))

        # Verificar formato
        if img.format not in ['JPEG', 'PNG']:
            return False, "Formato de imagen no válido"

        # Verificar dimensiones mínimas
        warning = None
        if img.width < 200 or img.height < 200:
            warning = "Imagen pequeña, los resultados pueden no ser confiables"
            logger.warning(f"{warning} ({img.width}x{img.height}px)")

        # Verificar si tiene metadatos EXIF (fotos reales suelen tenerlos)
        has_exif = hasattr(img, '_getexif') and img._getexif() is not None

        return True, {
            "has_exif": has_exif,
            "format": img.format,
            "size": (img.width, img.height),
            "warning": warning
        }

    except Exception as e:
        return False, f"Error validando metadata: {e}"

@app.route('/', methods=['GET'])
def health_check():
    """Endpoint de health check"""
    return jsonify({
        "status": "ok",
        "service": "Enhanced Face Verification Service",
        "model_loaded": modelo is not None,
        "features": ["face_verification", "liveness_detection", "anti_spoofing"]
    })

@app.route('/verificar', methods=['POST'])
def verificar():
    """Endpoint principal para verificación facial con anti-spoofing"""
    try:
        # Verificar que el modelo esté cargado
        if modelo is None:
            return jsonify({"error": "Modelo no disponible"}), 500
            
        data = request.get_json()
        if not data:
            return jsonify({"error": "No se recibieron datos JSON"}), 400
            
        img1_b64 = data.get("img1")  # Imagen de referencia (RENIEC)
        img2_b64 = data.get("img2")  # Imagen a verificar (usuario)

        # Validación básica
        if not img1_b64 or not img2_b64:
            return jsonify({"error": "Imágenes no recibidas correctamente"}), 400

        # Validar que las imágenes base64 tengan formato correcto
        try:
            # Remover prefijo data:image si existe
            if img1_b64.startswith('data:image'):
                img1_b64 = img1_b64.split(',')[1]
            if img2_b64.startswith('data:image'):
                img2_b64 = img2_b64.split(',')[1]
                
            img1_data = base64.b64decode(img1_b64)
            img2_data = base64.b64decode(img2_b64)
        except Exception as e:
            return jsonify({"error": "Error al decodificar imágenes base64"}), 400

        # Validar metadata de las imágenes
        metadata1_valid, metadata1_info = validar_metadata_imagen(img1_data)
        metadata2_valid, metadata2_info = validar_metadata_imagen(img2_data)
        
        if not metadata1_valid or not metadata2_valid:
            return jsonify({
                "error": "Imágenes no válidas",
                "details": {
                    "img1": metadata1_info if not metadata1_valid else "OK",
                    "img2": metadata2_info if not metadata2_valid else "OK"
                }
            }), 400

        # Guardar temporalmente y procesar
        temp_files = []
        try:
            with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f1, \
                 tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f2:

                f1.write(img1_data)
                f2.write(img2_data)
                f1.flush()
                f2.flush()
                
                temp_files = [f1.name, f2.name]

                # PASO 1: Detección de liveness en la imagen del usuario (img2)
                is_live, liveness_info = detectar_liveness(f2.name)
                
                if not is_live:
                    return jsonify({
                        "resultado": "IMAGEN SOSPECHOSA",
                        "motivo": "La imagen del usuario no parece ser una foto real",
                        "liveness_score": liveness_info.get("total_score", 0) if isinstance(liveness_info, dict) else 0,
                        "detalles_liveness": liveness_info if isinstance(liveness_info, dict) else str(liveness_info)
                    })

                # PASO 2: Verificación facial solo si pasa el test de liveness
                result = DeepFace.verify(
                    img1_path=f1.name,
                    img2_path=f2.name,
                    model_name="Facenet",
                    enforce_detection=False
                )

            # Limpiar archivos temporales
            for temp_file in temp_files:
                try:
                    os.unlink(temp_file)
                except:
                    pass

            # Determinar resultado final
            face_match = result["verified"]
            confidence = result.get("distance", 0)
            threshold = result.get("threshold", 0)
            
            # Resultado final combinando verificación facial y liveness
            if face_match and is_live:
                resultado_final = "COINCIDE"
            elif face_match and not is_live:
                resultado_final = "SOSPECHOSA"
            else:
                resultado_final = "NO COINCIDE"

            return jsonify({
                "resultado": resultado_final,
                "verificacion_facial": {
                    "coincide": face_match,
                    "confianza": confidence,
                    "threshold": threshold
                },
                "verificacion_liveness": {
                    "es_imagen_real": is_live,
                    "score": liveness_info.get("total_score", 0) if isinstance(liveness_info, dict) else 0,
                    "detalles": liveness_info if isinstance(liveness_info, dict) else str(liveness_info)
                },
                "metadata": {
                    "img1": metadata1_info,
                    "img2": metadata2_info
                }
            })

        except Exception as e:
            # Limpiar archivos temporales en caso de error
            for temp_file in temp_files:
                try:
                    os.unlink(temp_file)
                except:
                    pass
            raise e

    except Exception as e:
        logger.error(f"Error en /verificar: {e}")
        return jsonify({"error": "Error interno del servidor"}), 500

@app.route('/test-liveness', methods=['POST'])
def test_liveness():
    """Endpoint para probar solo la detección de liveness"""
    try:
        data = request.get_json()
        if not data:
            return jsonify({"error": "No se recibieron datos JSON"}), 400
            
        img_b64 = data.get("imagen")
        if not img_b64:
            return jsonify({"error": "Imagen no recibida"}), 400

        # Decodificar imagen
        if img_b64.startswith('data:image'):
            img_b64 = img_b64.split(',')[1]
        img_data = base64.b64decode(img_b64)

        # Validar metadata
        metadata_valid, metadata_info = validar_metadata_imagen(img_data)
        
        # Guardar temporalmente y analizar
        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f:
            f.write(img_data)
            f.flush()
            
            is_live, liveness_info = detectar_liveness(f.name)
            
            try:
                os.unlink(f.name)
            except:
                pass

        return jsonify({
            "es_imagen_real": is_live,
            "detalles_liveness": liveness_info,
            "metadata": metadata_info
        })

    except Exception as e:
        logger.error(f"Error en /test-liveness: {e}")
        return jsonify({"error": "Error interno del servidor"}), 500

@app.route('/status', methods=['GET'])
def status():
    """Endpoint de estado del servicio"""
    return jsonify({
        "service": "Enhanced Face Verification API",
        "version": "2.0.0",
        "status": "running",
        "model_status": "loaded" if modelo is not None else "error",
        "features": {
            "face_verification": True,
            "liveness_detection": True,
            "anti_spoofing": True,
            "metadata_validation": True
        }
    })

@app.errorhandler(404)
def not_found(error):
    return jsonify({"error": "Endpoint no encontrado"}), 404

@app.errorhandler(500)
def internal_error(error):
    return jsonify({"error": "Error interno del servidor"}), 500

if __name__ == '__main__':
    port = int(os.environ.get('PORT', 5000))
    app.run(host="0.0.0.0", port=port, debug=False)