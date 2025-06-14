# language: es
Característica: Generación de Código QR para Estudiantes
  Como un estudiante autenticado con correo institucional
  Quiero generar mi código QR personal
  Para poder acceder a las instalaciones universitarias

  Antecedentes:
    Dado que soy un estudiante registrado con DNI "12345678"
    Y mi matrícula es "2021001"
    Y mi correo es "estudiante@virtual.upt.pe"
    Y he iniciado sesión correctamente

  Escenario: Generación exitosa de código QR por primera vez
    Cuando solicito generar mi código QR
    Entonces debería recibir un código QR válido
    Y el código QR debería contener mis datos encriptados
    Y debería guardarse un token en la base de datos
    Y la respuesta debería incluir la fecha y hora actual de Lima

  Escenario: Regeneración de código QR existente
    Dado que ya tengo un código QR generado previamente
    Cuando solicito generar mi código QR nuevamente
    Entonces debería recibir un nuevo código QR
    Y el token anterior debería ser actualizado en la base de datos
    Y el nuevo código QR debería tener una nueva marca de tiempo

  Escenario: Generación de código QR con usuario no válido
    Dado que mi token JWT no contiene un ID de usuario válido
    Cuando solicito generar mi código QR
    Entonces debería recibir un error "ID de usuario no válido"
    Y no debería generarse ningún código QR

  Escenario: Generación de código QR para estudiante no encontrado
    Dado que mi ID de usuario es "999" pero no existo en la base de datos
    Cuando solicito generar mi código QR
    Entonces debería recibir un error "Estudiante no encontrado"
    Y no debería generarse ningún código QR

  Escenario: Verificación de duración del código QR
    Dado que he generado un código QR
    Cuando pasan 5 minutos desde la generación
    Entonces el código QR debería considerarse expirado
    Y debería necesitar regenerar un nuevo código QR