![C:\Users\EPIS\Documents\upt.png](media/image1.png){width="1.0926727909011373in"
height="1.468837489063867in"}

**UNIVERSIDAD PRIVADA DE TACNA**

**FACULTAD DE INGENIERÍA**

**Escuela Profesional de Ingeniería de Sistemas**

**"Sistema de Control de Acceso Electrónico (SCAE-UPT)"**

Curso: *Calidad y Pruebas de Software*

> Docente: Mag. Patrick Cuadros Quiroga
>
> Integrantes:

***Antayhua Mamani, Renzo Antonio (2022074258)***

***Colque Ponce, Sergio Alberto (2022073503)***

**Tacna -- Perú**

***2025***

|                      |               |              |              |            |                  |
|----------------------|---------------|--------------|--------------|------------|------------------|
| CONTROL DE VERSIONES |               |              |              |            |                  |
| Versión              | Hecha por     | Revisada por | Aprobada por | Fecha      | Motivo           |
| 1.0                  | RAM, BAS, ACL | ELV          | ELV          | 30/01/2025 | Versión Original |

Sistema Integral de Control de Acceso Electrónico en la UPT

Documento de Arquitectura de Software

Versión *1.0*

|                      |               |              |              |            |                  |
|----------------------|---------------|--------------|--------------|------------|------------------|
| CONTROL DE VERSIONES |               |              |              |            |                  |
| Versión              | Hecha por     | Revisada por | Aprobada por | Fecha      | Motivo           |
| 1.0                  | RAM, BAS, ACL | ELV          | ELV          | 30/01/2025 | Versión Original |

ÍNDICE GENERAL

[1 Introducción 4](#introducción)

> [1.1 Propósito 4](#propósito)
>
> [1.2 Alcance 5](#alcance)
>
> [1.3 Definición, siglas y abreviaturas
> 5](#definición-siglas-y-abreviaturas)
>
> [1.4 Referencias 5](#referencias)
>
> [1.5 Visión General 5](#visión-general)

[2 Representación Arquitectónica 6](#representación-arquitectónica)

> [2.1 Escenarios 6](#escenarios)
>
> [2.2 Vista Lógica 6](#vista-lógica)
>
> [2.3 Vista del Proceso 6](#_heading=h.17dp8vu)
>
> [2.4 Vista del Desarrollo 6](#_heading=h.3rdcrjn)
>
> [2.5 Vista Física 6](#_heading=h.26in1rg)

[3 Objetivos y Limitaciones arquitectónicas
6](#objetivos-y-limitaciones-arquitectónicas)

> [3.1 Disponibilidad 6](#disponibilidad)
>
> [3.2 Seguridad 6](#seguridad)
>
> [3.3 Adaptibilidad 6](#adaptabilidad)
>
> [3.4 Rendimiento 6](#rendimiento)

[4 Análisis de Requerimientos 7](#análisis-de-requerimientos)

> [4.1 Requerimientos Funcionales 7](#requerimientos-funcionales)
>
> [4.2 Requerimientos No Funcionales 7](#requerimientos-no-funcionales)

[5 Vistas de Caso de Uso 7](#vistas-de-caso-de-uso)

[6 Vista Lógica 7](#vista-lógica-1)

> [6.1 Diagrama Contextual 7](#diagrama-contextual)

[7 Vista de Procesos 7](#vista-de-procesos)

> [7.1 Diagrama de Proceso Actual 7](#diagrama-de-proceso-actual)
>
> [7.2 Diagrama de Proceso Propuesto 7](#diagrama-de-proceso-propuesto)

[8 Vista de Despliegue 7](#vista-de-despliegue)

> [8.1 Diagrama de Contenedor 7](#_heading=h.1pxezwc)

[9 Vista de Implementación 8](#vista-de-implementación)

> [9.1 Diagrama de Componentes
> 8](#diagrama-de-paquetes-de-sistema-escritorio-scae)

[10 Vista de Datos 8](#_heading=h.147n2zr)

> [10.1 Diagrama Entidad Relación 8](#diagrama-entidad-relación)

[11 Calidad 8](#_heading=h.23ckvvd)

> [11.1 Escenarios de Seguridad 8](#_heading=h.ihv636)
>
> [11.2 Escenario de Usabilidad 8](#_heading=h.32hioqz)
>
> [11.3 Escenario de Adaptabilidad 8](#_heading=h.1hmsyys)
>
> [11.4 Escenario de Disponibilidad 8](#_heading=h.41mghml)
>
> [11.5 Otro Escenario 8](#_heading=h.2grqrue)

# Introducción

## Propósito

> El propósito de este documento es proporcionar una descripción
> detallada de la estructura, componentes y diseño del sistema SCAE-UPT
> (Sistema Integral de Control de Acceso Electronico en la UPT). Servirá
> como guía principal para los equipos de desarrollo, implementación y
> mantenimiento, permitiendo una comprensión clara de la arquitectura
> del sistema. El objetivo principal del SCAE-UPT es la gestión del
> acceso al estacionamiento en la Universidad Privada de Tacna (UPT)
> mediante tecnologías avanzadas de autenticación por credenciales RFID,
> así como la automatización de notificaciones y reportes.

## Alcance

> El alcance de este documento abarca los aspectos técnicos y
> funcionales del SCAE-UPT, detallando los elementos esenciales que
> componen la solución, como la autenticación por credenciales RFID, el
> registro de entradas y salidas de personas en la UPT, y la generación
> de reportes en formato PDF. La arquitectura documentada en el SAD
> contempla tanto el hardware (lectores RFID, computadora) como el
> software (Drivers de Arduino, base de datos para la gestión de
> registros) y su integración en la infraestructura de la UPT.

## Definición, siglas y abreviaturas

> Este apartado contiene una lista de términos clave para la correcta
> comprensión de los conceptos y tecnologías utilizadas en el documento
> y en el desarrollo del SCAE-UPT:

- **SICAV-UPT:** Sistema Integral de Control de Acceso Electronico en la
  Universidad Privada de Tacna.

- **UPETINO:** Término usado para referirse a estudiantes, docentes o
  trabajadores afiliados a la UPT.

- **INVITADO:** Término usado para las personas que son ajena a la UPT.

- **RFID:** Siglas de \"Radio Frequency Identification\", es una
  tecnología que permite identificar objetos mediante ondas de radio de
  manera única para autenticar la identidad de UPETINOS mediante escaneo
  en el sistema.

## Referencias

Documento de Factibilidad: FD01-Documentacion_Factibilidad

Documento de Visión: FD02-Documentacion_Visión

Documento de SRS: FD03-Documentacion_SRS

## Visión General

> Este documento SAD proporciona una visión completa de la arquitectura
> del sistema SCAE-UPT. Se describen las diversas vistas arquitectónicas
> que componen el sistema, como la vista lógica, la vista de procesos,
> la vista de despliegue y la vista de datos. También se detallan los
> requisitos funcionales y no funcionales, los casos de uso, y los
> componentes del sistema necesarios para lograr los objetivos de
> seguridad, eficiencia y adaptabilidad en la gestión del
> estacionamiento de la UPT. La arquitectura está diseñada para ofrecer
> una experiencia de usuario intuitiva y un alto nivel de seguridad,
> permitiendo a los equipos técnicos comprender cada aspecto del sistema
> y su funcionamiento integral.

# Representación Arquitectónica

## Escenarios

- **Escenario de Funcionalidad**

Necesidad: Permitir que los usuarios (guardias, administradores,
docentes) puedan acceder al sistema SCAE-UPT en cualquier momento del
día para gestionar y monitorear el control de acceso.

Solución: Mantener el sistema operativo 24/7, con mecanismos de respaldo
y servidores redundantes para garantizar la disponibilidad continua. En
caso de inconvenientes, se implementará una línea de soporte para dar
acceso a usuarios autorizados.

Justificación: Al tratarse de un sistema de control de acceso y
monitoreo en tiempo real, la disponibilidad continua es esencial para
asegurar la operatividad del campus.

- **Escenario de Usabilidad**

Necesidad: Los guardias y administradores requieren una interfaz de
usuario intuitiva y fácil de aprender para gestionar el acceso.

Solución: Implementar una interfaz gráfica sencilla y estructurada, con
un diseño claro y botones de acción intuitivos.

Justificación: Un sistema fácil de usar permite que el personal se
adapte rápidamente y reduce la probabilidad de errores en las tareas
críticas de control de acceso y registro.

- **Escenario de Confiabilidad**

Necesidad: Garantizar que los usuarios confíen en el sistema y que el
sistema mantenga la seguridad de los datos de acceso, y datos de
usuarios (docentes y visitantes).

Solución: Implementar medidas de autenticación robusta, como cuentas de
usuario con contraseñas seguras y autenticación por RFID , y proteger la
base de datos con cifrado. Además, implementar auditorías y registros de
acceso para identificar cualquier actividad inusual.

Justificación: La seguridad es esencial para mantener la
confidencialidad de la información y dar confianza a los usuarios de que
los datos almacenados están protegidos y que el sistema es seguro.

- **Escenario de Mantenibilidad**

Necesidad: Asegurar que el sistema SCAE-UPT sea fácil de mantener y
actualizar para adaptarse a nuevas necesidades o resolver posibles
fallos sin interrumpir el servicio.

Solución: Establecer un protocolo de mantenimiento programado, realizar
revisiones constantes del sistema y contar con registros de errores que
permitan detectar y solucionar fallos de manera proactiva.

Justificación: La mantenibilidad del sistema asegura que pueda adaptarse
a cambios y seguir siendo confiable a largo plazo, además de permitir
futuras actualizaciones con un mínimo impacto en la operatividad diaria
del sistema.

## Vista Lógica

![](media/image2.png){width="5.905216535433071in"
height="4.305555555555555in"}

[]{#_heading=h.17dp8vu .anchor}*Descripción: La Figura 1 muestra la
estructura principal de las distintas clases que conforman el proyecto
SCAE-- UPT, las relacionas como sus cardinalidades, en la cual se
observa que la mayoría de clases dependen de la clase ClsConexion,
debido a que sin ella no podrían tener acceso a la base de datos.*

# Objetivos y Limitaciones arquitectónicas

## Disponibilidad

- **Objetivo**: Garantizar que el sistema SCAE-UPT esté disponible para
  el control de acceso al campus de la UPT en todo momento,
  especialmente en horarios de alta afluencia, minimizando tiempos de
  inactividad no planificados.

- **Limitación**: Habrá periodos de inactividad planificados para tareas
  de mantenimiento y actualización del sistema, durante los cuales
  algunas funcionalidades podrían no estar accesibles, como el registro
  automático de entradas y salidas.

## Seguridad

- **Objetivo**: Proteger los datos del sistema (información de vehículos
  y usuarios) y garantizar que solo usuarios autorizados (guardias y
  administradores) puedan acceder a las funciones de control y
  monitoreo.

- **Limitación**: Aunque se implementarán medidas de seguridad como
  autenticación mediante credenciales y cifrado de datos, la protección
  total contra todas las amenazas no puede garantizarse, especialmente
  frente a ataques cibernéticos avanzados.

## Adaptabilidad

- **Objetivo**: Asegurar que el sistema SCAE-UPT pueda adaptarse a
  cambios en los requisitos de la UPT, como la integración de nuevos
  módulos o la expansión de funciones para cubrir otros accesos en el
  campus.

- **Limitación**: La adaptabilidad del sistema puede estar restringida
  por limitaciones de recursos técnicos y financieros, lo que podría
  ralentizar la implementación de mejoras o nuevas funciones,
  particularmente si requieren rediseños importantes.

## Rendimiento

- **Objetivo**: Optimizar el rendimiento del sistema para asegurar que
  el reconocimiento de RFID funcione en tiempo real, brindando una
  experiencia de uso rápida y eficiente.

- **Limitación**: Aunque se aplicarán optimizaciones, el rendimiento
  podría verse afectado en situaciones de alta demanda o bajo cargas de
  procesamiento intensivas, como horas pico, lo cual podría generar un
  leve retraso en el procesamiento de imágenes y la actualización de
  datos en el sistema web.

# Análisis de Requerimientos

## Requerimientos Funcionales

> Tabla 1. Cuadro de requerimientos funcionales. Fuente: Elaboración
> Propia.

|                |         |           |
|----------------|---------|-----------|
| **Escritorio** | **Web** | **Mixto** |

|                            |                                                                  |                                                                                                                                                                                                                                                  |           |                |                      |             |
|----------------------------|------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------|----------------|----------------------|-------------|
| REQUERIMIENTOS FUNCIONALES |                                                                  |                                                                                                                                                                                                                                                  |           |                |                      |             |
| Código                     | Requerimiento Funcional                                          | Descripción del Requerimiento                                                                                                                                                                                                                    | Prioridad | Urgencia       | Estado de Desarrollo | Estabilidad |
| FRQ-Q001                   | Visualizar estadística de los registros                          | El administrador visualiza un resumen de los registros mediante distintas graficas (pie: Niveles de Usuario, barras: Ingresos por hora y lineal: Ingresos por tipo de persona).                                                                  | Alta      | Inmediatamente | Validado             | Alta        |
| FRQ-Q002                   | Mostrar perfil detallado                                         | Se podrá visualizar más detalle con respecto a los datos del perfil seleccionado, como del mismo usuario que ingreso al sistema.                                                                                                                 | Alta      | Inmediatamente | Validado             | Alta        |
| FRQ-Q003                   | Registrar el ingreso y salida al campus                          | El guardia registrará el ingreso y salida de upetinos (mediante tarjeta RFID UHF) que entran al campus, como visitantes (mediante consulta DNI o CARNET DE EXTRANJERÍA)                                                                          | Alta      | Inmediatamente | Validado             | Media       |
| FRQ-Q004                   | Generar Reportes                                                 | El usuario (Administrador, guardia) generar distintos tipos de reporte, entre los cuales están por Tipo, Persona, Rango de fechas o una fecha específica.                                                                                        | Alta      | Inmediatamente | Validado             | Media       |
| FRQ-Q05                    | Mostrar historial de registros.                                  | El administrador y guardia pueden visualizar el historial de los registros de control de acceso electrónico.                                                                                                                                     | Alta      | Inmediatamente | Validado             | Media       |
| FRQ-Q006                   | Bloquear las tarjetas RFID                                       | El Administrador como el guardia podrán bloquear la tarjeta del usuario final (upetinos) en caso de robo o pérdida.                                                                                                                              | Alta      | Inmediatamente | Validado             | Media       |
| FRQ-Q007                   | Gestionar Usuarios                                               | El Administrador tendrá acceso total para gestionar usuarios, configurar accesos y visualizar estadísticas. El Guardia de seguridad podrá registrar ingresos y verificar accesos. Los Empleados tendrán permisos limitados según sus necesidades | Media     | Necesario      | Validado             | Alta        |
| FRQ-Q08                    | Mostrar los registros de acceso en tiempo real                   | El usuario (Administrador y Guardia) pueden visualizar los registros de acceso del día en tiempo real.                                                                                                                                           | Media     | Necesario      | Validado             | Media       |
| FRQ-Q09                    | Descargar Reportes                                               | Se podrán descargar reportes en los siguientes formatos de archivo: PDF, EXCEL.                                                                                                                                                                  | Media     | Necesario      | Validado             | Media       |
| FRQ-Q010                   | Visualizar estadística Generales                                 | Se podrá visualizar la cantidad de accesos, usuarios dentro del campus y alertas, asimismo un gráfico con la tendencia por día, semana y mes.                                                                                                    | Alta      | Inmediatamente | Validado             | Alta        |
| FRQ-Q011                   | Identificación de usuarios con acceso restringido                | Cuando un usuario con acceso restringido intente ingresar, el sistema deberá generar una notificación al Administrador y al Guardia de seguridad                                                                                                 | Media     | Inmediatamente | Validado             | Alta        |
| FRQ-Q012                   | Filtros avanzados por usuario, rango de fechas, tipo de acceso.  | Permitir la búsqueda y visualización de registros de acceso mediante filtros avanzados, incluyendo usuario específico, rango de fechas y tipo de acceso (entrada, salida, acceso denegado, entre otros).                                         | Media     | Necesario      | Validado             | Alta        |
| FRQ-Q013                   | Notificar a estudiantes o empleados sobre novedades del sistema. | Enviar notificaciones a upetinos (estudiantes y empleados) sobre novedades relevantes: actividad sospechosa.                                                                                                                                     | Baja      | Necesario      | Validado             | Alta        |
| FRQ-Q014                   | Consultar historial de acceso                                    | El upetino puede ver el historial de sus registros de acceso propios.                                                                                                                                                                            | Baja      | Necesario      | Validado             | Alta        |

## Requerimientos No Funcionales  {#requerimientos-no-funcionales}

> Tabla 2. Cuadro de requerimientos no funcionales. Fuente: Elaboración
> Propia.

|                |         |           |
|----------------|---------|-----------|
| **Escritorio** | **Web** | **Mixto** |

|                               |                            |                                                                                                |           |           |                      |             |
|-------------------------------|----------------------------|------------------------------------------------------------------------------------------------|-----------|-----------|----------------------|-------------|
| REQUERIMIENTOS NO FUNCIONALES |                            |                                                                                                |           |           |                      |             |
| Código                        | Requerimiento No Funcional | Descripción del Requerimiento                                                                  | Prioridad | Urgencia  | Estado de Desarrollo | Estabilidad |
| NFR-Q001                      | Seguridad                  | Los administradores inician sesión mediante un usuario y contraseña.                           | Media     | Necesario | Validado             | Alta        |
|                               |                            | Los guardias podrán autenticarse mediante una tarjeta RFID UHF.                                | Media     | Necesario | Validado             | Alta        |
|                               |                            | Control de intentos fallidos de login (bloqueo tras varios intentos incorrectos).              | Media     | Necesario | Validado             | Alta        |
| NFR-Q002                      | Tiempo de respuesta        | El sistema debe realizar las operaciones correspondientes en menos de 3 segundos               | Alta      | Necesario | Validado             | Alta        |
| NFR-Q003                      | Protección de datos        | La plataforma deberá contar con autenticación segura (HTTPS) y roles de acceso bien definidos. | Alta      | Urgente   | Validado             | Alta        |
| NFR-Q004                      | Compatibilidad             | Acceso desde navegadores web modernos y dispositivos móviles.                                  | Media     | Necesario | Validado             | Alta        |

# Vistas de Caso de Uso

![](media/image3.jpg){width="5.905216535433071in"
height="2.8472222222222223in"}

***Comentario:*** Tenemos el diagrama de Casos de Uso del Sistema
Escritorio SCAE.

![](media/image4.png){width="5.905216535433071in"
height="3.2916666666666665in"}

***Comentario:*** Tenemos el diagrama de Casos de Uso del Sistema Web
SCAE.

# Vista Lógica

1.  **Diagrama de Secuencia**

**6.1.1 Iniciar sesión por Contraseña**

![](media/image5.jpg){width="5.3447922134733155in"
height="3.346386701662292in"}

**6.1.2 Iniciar sesión por Contraseña**

![](media/image6.png){width="5.905216535433071in" height="3.625in"}

**6.1.3 Registrar Visitante**

![](media/image7.png){width="5.905216535433071in"
height="4.013888888888889in"}

**6.1.4 Bloquear Tarjeta RFID**

![](media/image8.png){width="5.905216535433071in"
height="3.263888888888889in"}

**6.1.5 Usuario Visitante**

![](media/image9.png){width="5.905555555555556in"
height="3.342642169728784in"}

**6.1.6 Usuario Upetino**

![](media/image10.png){width="5.905216535433071in"
height="2.6666666666666665in"}

**6.1.7 Usuario Guardia**

![](media/image11.jpg){width="5.905555555555556in"
height="3.880614610673666in"}

**6.1.8 Usuario Administrador**

![](media/image12.jpg){width="5.905555555555556in"
height="4.6729943132108485in"}

## Diagrama Contextual

![](media/image13.jpg){width="5.084375546806649in"
height="3.4433858267716535in"}

# Vista de Procesos

## Diagrama de Proceso Actual

![](media/image14.png){width="4.798369422572178in"
height="3.1563265529308837in"}

> Descripción: El diagrama de procesos actual adjunto representa la
> situación real al momento del ingreso de los estudiantes

## Diagrama de Proceso Propuesto

![](media/image15.png){width="5.905216535433071in"
height="3.7222222222222223in"}

> Descripción: El diagrama de procesos Propuesto adjunto representa la
> situación real al momento del ingreso de los estudiantes

# Vista de Despliegue

# Diagrama de Contenedor del Sistema Escritorio SCAE  {#diagrama-de-contenedor-del-sistema-escritorio-scae}

![](media/image16.jpg){width="5.905216535433071in"
height="4.472222222222222in"}

> Descripción: El diagrama de despliegue de sistema de escritorio SCAE.

# Diagrama de Contenedor del Sistema Web SCAE  {#diagrama-de-contenedor-del-sistema-web-scae}

![](media/image17.jpg){width="5.905216535433071in"
height="5.902777777777778in"}

> Descripción: El diagrama de despliegue de sistema web SCAE.

# Vista de Implementación

## Diagrama de Paquetes de Sistema escritorio SCAE

> ![](media/image18.jpg){width="5.905216535433071in"
> height="3.3194444444444446in"}
>
> *Comentario:* Tenemos el diagrama de paquetes del Sistema Escritorio
> SCAE.

## Diagrama de Paquetes de Sistema web SCAE

![](media/image19.jpg){width="5.905216535433071in"
height="2.513888888888889in"}

*Comentario:* Tenemos el diagrama de paquetes del Sistema Web SCAE.

## Diagrama de Componentes de Sistema escritorio SCAE

![](media/image20.jpg){width="5.905216535433071in"
height="3.5972222222222223in"}

*Comentario:* Tenemos el diagrama de componentes del Sistema Escritorio
SCAE.

## Diagrama de Componentes de Sistema Web SCAE

> ![](media/image21.jpg){width="5.905216535433071in"
> height="3.486111111111111in"}

*Comentario:* Tenemos el diagrama de componentes del Sistema Web SCAE.

## Diagrama Entidad Relación

![](media/image22.png){width="5.905216535433071in"
height="5.194444444444445in"}
