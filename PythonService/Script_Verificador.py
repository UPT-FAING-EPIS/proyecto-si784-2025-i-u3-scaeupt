from flask import Flask, request, jsonify
from deepface import DeepFace
import base64
import tempfile
import os
import logging

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

@app.route('/', methods=['GET'])
def health_check():
    """Endpoint de health check"""
    return jsonify({
        "status": "ok",
        "service": "Face Verification Service",
        "model_loaded": modelo is not None
    })

@app.route('/verificar', methods=['POST'])
def verificar():
    """Endpoint principal para verificación facial"""
    try:
        # Verificar que el modelo esté cargado
        if modelo is None:
            return jsonify({"error": "Modelo no disponible"}), 500
            
        data = request.get_json()
        if not data:
            return jsonify({"error": "No se recibieron datos JSON"}), 400
            
        img1_b64 = data.get("img1")
        img2_b64 = data.get("img2")

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

                # Realizar verificación
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

            return jsonify({
                "resultado": "COINCIDE" if result["verified"] else "NO COINCIDE",
                "confianza": result.get("distance", 0),
                "threshold": result.get("threshold", 0)
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

@app.route('/status', methods=['GET'])
def status():
    """Endpoint de estado del servicio"""
    return jsonify({
        "service": "Face Verification API",
        "version": "1.0.0",
        "status": "running",
        "model_status": "loaded" if modelo is not None else "error"
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