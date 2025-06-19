# Dockerfile para el servicio Python de verificación facial
FROM python:3.9-slim

# Instalar dependencias del sistema necesarias para OpenCV y DeepFace
RUN apt-get update && apt-get install -y \
    libglib2.0-0 \
    libsm6 \
    libxext6 \
    libxrender-dev \
    libgomp1 \
    libglib2.0-0 \
    libgtk-3-0 \
    libgl1-mesa-glx \
    libgstreamer1.0-0 \
    libgstreamer-plugins-base1.0-0 \
    wget \
    && rm -rf /var/lib/apt/lists/*

# Establecer directorio de trabajo
WORKDIR /app

# Copiar requirements.txt
COPY requirements.txt .

# Instalar dependencias de Python
RUN pip install --no-cache-dir -r requirements.txt

# Copiar el script de Python
COPY Script_Verificador.py .

# Exponer el puerto 5000
EXPOSE 5000

# Configurar variables de entorno
ENV FLASK_APP=Script_Verificador.py
ENV FLASK_ENV=production
ENV PYTHONUNBUFFERED=1

# Comando para ejecutar la aplicación
CMD ["python", "Script_Verificador.py"]