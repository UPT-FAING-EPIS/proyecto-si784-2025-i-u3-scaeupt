# Dockerfile para el servicio Python de verificación facial con anti-spoofing
FROM python:3.9-slim

# Instalar dependencias del sistema necesarias para OpenCV, DeepFace y procesamiento de imágenes
RUN apt-get update && apt-get install -y \
    # Dependencias básicas del sistema
    libglib2.0-0 \
    libsm6 \
    libxext6 \
    libxrender-dev \
    libgomp1 \
    libgtk-3-0 \
    # Dependencias para OpenCV
    libgl1-mesa-glx \
    libglib2.0-0 \
    libgstreamer1.0-0 \
    libgstreamer-plugins-base1.0-0 \
    libavcodec-dev \
    libavformat-dev \
    libswscale-dev \
    libv4l-dev \
    libxvidcore-dev \
    libx264-dev \
    # Dependencias para procesamiento de imágenes
    libjpeg-dev \
    libpng-dev \
    libtiff-dev \
    libwebp-dev \
    libopenjp2-7-dev \
    # Dependencias adicionales para PIL/Pillow
    libfreetype6-dev \
    liblcms2-dev \
    libopenjp2-7-dev \
    libtiff5-dev \
    tk-dev \
    tcl-dev \
    # Herramientas de compilación necesarias
    build-essential \
    cmake \
    pkg-config \
    # Utilidades y compatibilidad
    wget \
    curl \
    libmagic1 \
    tzdata \
    && rm -rf /var/lib/apt/lists/*

# Establecer directorio de trabajo
WORKDIR /app

# Copiar requirements.txt primero para aprovechar la cache de Docker
COPY requirements.txt .

# Actualizar pip y instalar wheel para compilaciones más eficientes
RUN pip install --upgrade pip setuptools wheel

# Instalar dependencias de Python con optimizaciones
RUN pip install --no-cache-dir -r requirements.txt

# Crear directorio para archivos temporales
RUN mkdir -p /tmp/face_verification && chmod 777 /tmp/face_verification

# Copiar el script de Python actualizado
COPY Script_Verificador.py .

# Configurar variables de entorno
ENV FLASK_APP=Script_Verificador.py
ENV FLASK_ENV=production
ENV PYTHONUNBUFFERED=1
ENV TMPDIR=/tmp/face_verification

# Configurar variables de entorno para OpenCV
ENV OPENCV_LOG_LEVEL=ERROR
ENV MPLBACKEND=Agg

# Configurar variables para TensorFlow (optimización)
ENV TF_CPP_MIN_LOG_LEVEL=2
ENV CUDA_VISIBLE_DEVICES=""

# Exponer el puerto 5000
EXPOSE 5000


# Crear usuario no-root para seguridad
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Crear home para appuser y asignar permisos
RUN mkdir -p /home/appuser && chown -R appuser:appuser /home/appuser

# Establecer HOME para evitar errores al crear carpetas como .deepface
ENV HOME=/home/appuser

# Asignar permisos al appuser para /app y /tmp
RUN chown -R appuser:appuser /app /tmp/face_verification

USER appuser



# Health check para verificar que el servicio esté funcionando
HEALTHCHECK --interval=30s --timeout=30s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:5000/ || exit 1

# Comando para ejecutar la aplicación con gunicorn en producción
CMD ["gunicorn", "--bind", "0.0.0.0:5000", "--workers", "2", "--timeout", "120", "--max-requests", "1000", "--max-requests-jitter", "100", "Script_Verificador:app"]