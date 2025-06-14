# 🎓 SCAE-UPT - Sistema de Control de Acceso Electrónico
Sistema Integral de Control de Acceso Electrónico para la Universidad Privada de Tacna

## 📋 Descripción
SCAE-UPT es un sistema moderno y completo de control de acceso diseñado específicamente para la Universidad Privada de Tacna. El sistema moderniza el proceso de ingreso al campus universitario mediante tecnología QR y autenticación institucional, proporcionando una solución segura, eficiente y fácil de usar.

### 🎯 Características Principales
- 🔐 Autenticación Segura : Login con cuentas institucionales de Google
- 📱 Códigos QR Dinámicos : Generación de códigos QR únicos con expiración de 5 minutos
- 🖥️ Aplicación de Escritorio : Interface para personal de seguridad con escaneo QR
- 🌐 Plataforma Web : Portal estudiantil para generación de códigos de acceso
- 📊 Reportes y Auditoría : Historial completo de ingresos y salidas
- ⚡ Tiempo Real : Verificación instantánea de credenciales
## 🏗️ Arquitectura del Sistema
El proyecto está estructurado en dos componentes principales:

### 🖥️ Aplicación de Escritorio (DesktopApp)
Tecnologías : C# .NET Framework 4.7.2, Windows Forms, MySQL

- Propósito : Interface para personal de vigilancia
- Funcionalidades :
  - Escaneo y validación de códigos QR
  - Verificación de identidad en tiempo real
  - Registro automático de ingresos/salidas
  - Gestión de visitantes
  - Generación de reportes
### 🌐 Aplicación Web (WebApp)
Tecnologías : ASP.NET Core 8.0, Entity Framework, MySQL, Docker

- Propósito : Portal para estudiantes y administración
- Funcionalidades :
  - Autenticación con Google OAuth
  - Generación de códigos QR personales
  - Historial de accesos
  - API REST para integración
  - JWT para seguridad
## 🚀 Inicio Rápido
### 📱 Para Estudiantes - Plataforma Web
1. Accede al sistema : https://scae-upt-app.azurewebsites.net
2. Inicia sesión con tu cuenta institucional de Google (@virtual.upt.pe)
3. Genera tu código QR con un solo clic
4. Presenta el código en el acceso a la universidad
⏰ Importante : Los códigos QR tienen una validez de 5 minutos por seguridad

### 🖥️ Para Personal de Seguridad - Aplicación de Escritorio
Credenciales de Acceso :

- Usuario : segurid
- Contraseña : 123
Descarga : Enlace de descarga del instalador

## 🛠️ Instalación y Desarrollo
### Prerrequisitos
- .NET 8.0 SDK
- .NET Framework 4.7.2
- MySQL Server 8.0+
- Visual Studio 2022 o VS Code
- Docker (opcional)
### Configuración del Entorno
1. Clona el repositorio :
```
git clone https://github.com/usuario/proyecto-si784-2025-i-u2-scae-upt.git
cd proyecto-si784-2025-i-u2-scae-upt
```
2. Configura las variables de entorno :
```
# WebApp/.env
GOOGLE_CLIENT_ID=tu_google_client_id
GOOGLE_CLIENT_SECRET=tu_google_client_secret
JWT_SECRET_KEY=tu_jwt_secret_key
MYSQL_CONNECTION_STRING=tu_connection_string
```
3. Ejecuta la aplicación web :
```
cd WebApp/pyWeb_ScaeUPT
dotnet restore
dotnet run
```
4. Compila la aplicación de escritorio :
```
cd DesktopApp
nuget restore SCAE-UPT.sln
msbuild SCAE-UPT.sln /p:Configuration=Release
```
### 🐳 Usando Docker
```
cd WebApp/pyWeb_ScaeUPT
docker build -t scae-upt-web .
docker run -p 8080:8080 scae-upt-web
```
## 🧪 Testing
El proyecto incluye pruebas automatizadas con diferentes enfoques:

### Pruebas BDD (Behavior Driven Development)
```
cd WebApp/pyWeb_ScaeUPT.Tests
dotnet test
```
### Cobertura de Código
- Cobertura actual : 76.92%
- Reportes : Disponibles en TestResults/coverage.cobertura.xml
### Pruebas de Mutación
Se ejecutan automáticamente en el pipeline de CI/CD para validar la calidad de las pruebas.

## 📊 CI/CD y DevOps
El proyecto incluye pipelines automatizados de GitHub Actions:

- 🔨 Build & Release : Compilación automática de la aplicación de escritorio
- 📋 BDD Reports : Generación de reportes de pruebas de comportamiento
- 📈 Coverage Reports : Reportes de cobertura de código
- 🔒 Security Scans : Análisis de seguridad con Semgrep y Snyk
- ☁️ Terraform : Infraestructura como código para Azure
## 🏛️ Estructura del Proyecto
```
proyecto-si784-2025-i-u2-scae-upt/
├── 📱 DesktopApp/                 # Aplicación de escritorio
│   ├── pyDesktop_ScaeUPT/         # Código fuente principal
│   ├── SCAE-UPT.Installer/        # Instalador Inno Setup
│   └── SCAE-UPT.sln              # Solución Visual Studio
├── 🌐 WebApp/                     # Aplicación web
│   ├── pyWeb_ScaeUPT/            # API y frontend
│   ├── pyWeb_ScaeUPT.Tests/      # Pruebas automatizadas
│   └── ReporteBDD/               # Reportes de pruebas
├── 📚 Documentación/              # Informes técnicos
├── 🔧 .github/workflows/         # Pipelines CI/CD
└── 📄 README.md                  # Este archivo
```
## 🔒 Seguridad
- Autenticación OAuth 2.0 con Google
- JWT Tokens para sesiones seguras
- Códigos QR con expiración (5 minutos)
- Cifrado de datos sensibles
- Auditoría completa de accesos
- Análisis de seguridad automatizado
## 📖 Documentación Técnica
- 📋 Informe de Factibilidad
- 🎯 Documento de Visión
- 📝 Especificación de Requerimientos
- 🏗️ Arquitectura de Software
- 📊 Informe Final del Proyecto
## 🤝 Contribución
¡Tu ayuda es muy valiosa! Puedes contribuir de las siguientes maneras:

### 🐛 Reportar Errores
1. Ve a la sección Issues
2. Crea un nuevo issue describiendo el problema
3. Incluye pasos para reproducir el error
4. Adjunta capturas de pantalla si es necesario
### ✨ Sugerir Mejoras
1. Abre una Discussion
2. Describe tu idea o sugerencia
3. Explica cómo beneficiaría al proyecto
### 🔧 Contribuir Código
1. Fork el repositorio
2. Crea una rama para tu feature ( git checkout -b feature/nueva-funcionalidad )
3. Commit tus cambios ( git commit -m 'Añadir nueva funcionalidad' )
4. Push a la rama ( git push origin feature/nueva-funcionalidad )
5. Abre un Pull Request
## 👥 Equipo de Desarrollo
- Antayhua Mamani, Renzo Antonio (2022074258)
- Colque Ponce, Sergio Alberto (2022073503)
Docente : Mag. Patrick Cuadros Quiroga Curso : Calidad y Pruebas de Software Universidad : Universidad Privada de Tacna Facultad : Ingeniería - Escuela Profesional de Ingeniería de Sistemas