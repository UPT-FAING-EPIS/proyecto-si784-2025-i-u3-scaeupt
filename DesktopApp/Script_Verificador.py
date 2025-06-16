from flask import Flask, request, jsonify
from deepface import DeepFace
import base64
import tempfile
import os

app = Flask(__name__)

# Cargar modelo una sola vez
modelo = DeepFace.build_model("Facenet")

@app.route('/verificar', methods=['POST'])
def verificar():
    try:
        data = request.get_json()
        img1_b64 = data.get("img1")
        img2_b64 = data.get("img2")

        # Validación básica
        if not img1_b64 or not img2_b64:
            return jsonify({"error": "Imágenes no recibidas correctamente"}), 400

        # Guardar temporalmente
        with tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f1, \
             tempfile.NamedTemporaryFile(delete=False, suffix=".jpg") as f2:

            f1.write(base64.b64decode(img1_b64))
            f2.write(base64.b64decode(img2_b64))
            f1.flush()
            f2.flush()

            result = DeepFace.verify(
                img1_path=f1.name,
                img2_path=f2.name,
                model_name="Facenet",
                enforce_detection=False
            )


        os.unlink(f1.name)
        os.unlink(f2.name)

        return jsonify({
            "resultado": "COINCIDE" if result["verified"] else "NO COINCIDE"
        })

    except Exception as e:
        print("Error en /verificar:", e)
        return jsonify({"error": str(e)}), 500


if __name__ == '__main__':
    app.run(host="0.0.0.0", port=5000)
