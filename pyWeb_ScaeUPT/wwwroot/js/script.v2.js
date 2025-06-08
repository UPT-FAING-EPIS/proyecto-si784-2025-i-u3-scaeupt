let currentUser = null;
let isLoading = false;
let googleAuthStartTime = 0;

// Función para resetear el botón de Google
function resetGoogleButton() {
    const customBtn = document.getElementById('customGoogleBtn');
    const btnText = document.getElementById('btnText');
    const btnLoading = document.getElementById('btnLoading');

    if (customBtn && btnText && btnLoading) {
        customBtn.disabled = false;
        customBtn.style.pointerEvents = 'auto';
        btnText.textContent = 'Iniciar sesión con Google';
        btnLoading.classList.add('hidden');
        isLoading = false;
        googleAuthStartTime = 0;
    }
}

function setLoadingState() {
    const customBtn = document.getElementById('customGoogleBtn');
    const btnText = document.getElementById('btnText');
    const btnLoading = document.getElementById('btnLoading');

    if (customBtn && btnText && btnLoading) {
        customBtn.disabled = true;
        customBtn.style.pointerEvents = 'none';
        btnText.textContent = 'Iniciando sesión...';
        btnLoading.classList.remove('hidden');
        isLoading = true;
        googleAuthStartTime = Date.now();
    }
}

window.addEventListener('focus', function () {
    if (isLoading && googleAuthStartTime > 0 && (Date.now() - googleAuthStartTime) > 2000) {
        resetGoogleButton();
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const customBtn = document.getElementById('customGoogleBtn');
    if (customBtn) {
        customBtn.addEventListener('click', function () {
            if (isLoading) return;

            setLoadingState();

            // Simular click en el botón oculto de Google
            const googleBtn = document.querySelector('.g_id_signin div[role=button]');
            if (googleBtn) {
                googleBtn.click();
            } else {
                // Si no hay botón de Google, resetear después de un tiempo
                setTimeout(resetGoogleButton, 2000);
            }
        });
    }
});

// Mostrar dashboard
function showDashboard() {
    if (!currentUser) return;

    // Actualizar información del usuario
    document.getElementById('userName').textContent = currentUser.name;
    document.getElementById('userEmail').textContent = currentUser.email;
    document.getElementById('userAvatar').textContent = currentUser.avatar;

    // Cambiar de página
    document.getElementById('loginPage').style.display = 'none';
    document.getElementById('dashboard').classList.add('active');
}

// Cerrar sesión
function handleLogout() {
    currentUser = null;
    document.getElementById('dashboard').classList.remove('active');
    document.getElementById('loginPage').style.display = 'flex';
}

function generateQR() {
    // Verificar si hay un token de autenticación
    const token = localStorage.getItem('authToken');
    if (!token) {
        // Mostrar mensaje de error si no hay sesión
        const errorMessage = document.getElementById('errorMessage');
        errorMessage.textContent = 'Debes iniciar sesión para generar un código QR';
        showModal('errorModal');
        logout();
        return;
    }

    // Mostrar indicador de carga en el botón si existe
    const qrButton = document.querySelector('.action-card[onclick="generateQR()"]');
    if (qrButton) {
        qrButton.style.pointerEvents = 'none';
        const icon = qrButton.querySelector('.material-icons');
        if (icon) icon.textContent = 'hourglass_empty';
    }

    // Realizar solicitud al backend para generar el código QR
    fetchAuthenticatedAPI('/api/Home/generate-qr', { method: 'POST' })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error al generar el código QR');
            }
            return response.json();
        })
        .then(data => {
            console.log('QR generado:', data);

            // Actualizar el timestamp en el modal
            document.getElementById('qrTimestamp').textContent = data.timestamp;

            // Actualizar el código QR con la imagen Base64 recibida del servidor
            const qrCode = document.querySelector('.qr-code');
            if (qrCode) {
                // Limpiar cualquier contenido previo
                qrCode.innerHTML = '';

                // Crear y agregar la imagen del QR
                const img = document.createElement('img');
                img.src = 'data:image/png;base64,' + data.qrCodeBase64;
                img.alt = 'Código QR';
                img.style.width = '100%';
                img.style.height = 'auto';
                qrCode.appendChild(img);

                // Guardar el dato encriptado como atributo
                qrCode.setAttribute('data-qr', data.qrData);
            }

            // Mostrar el modal
            showModal('qrModal');
        })
        .catch(error => {
            console.error('Error:', error);
            // Mostrar mensaje de error
            const errorMessage = document.getElementById('errorMessage');
            errorMessage.textContent = 'Error al generar el código QR. Por favor, intenta nuevamente.';
            showModal('errorModal');
        })
        .finally(() => {
            // Restaurar el botón
            if (qrButton) {
                qrButton.style.pointerEvents = 'auto';
                const icon = qrButton.querySelector('.material-icons');
                if (icon) icon.textContent = 'qr_code';
            }
        });
}

document.addEventListener('DOMContentLoaded', function () {
    const logoutBtn = document.querySelector('.logout-btn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', logout);
    }

    // Verificar si el usuario ya está autenticado al cargar la página
    const token = localStorage.getItem('authToken');
    if (token) {
        // Validar el token en el servidor
        fetchAuthenticatedAPI('/api/auth/validate')
            .then(response => {
                if (!response.ok) throw new Error('Token inválido');
                return fetchAuthenticatedAPI('/api/Home/user-info');
            })
            .then(userResponse => {
                if (!userResponse.ok) throw new Error('Error al cargar datos del usuario');
                return userResponse.json();
            })
            .then(userData => {
                if (!userData || !userData.name || !userData.email) {
                    throw new Error('Datos de usuario incompletos');
                }
                            
                if(userData.email === localStorage.getItem('userEmail')){
                    cargarDatosUsuario();
                    
                    // Mostrar el dashboard
                    document.querySelector('.login-container').style.display = 'none';
                    document.querySelector('.dashboard').classList.add('active');
                }
                else{
                    logout();
                }

            })
            .catch(error => {
                console.error('Fallo en el proceso de autenticación:', error);
                logout();
            });
    }
});
// Mostrar historial
function showHistory() {
    showModal('historyModal');
}

// Mostrar modal
function showModal(modalId) {
    const modal = document.getElementById(modalId);
    modal.classList.add('active');
    document.body.style.overflow = 'hidden';
}

// Cerrar modal
function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    modal.classList.remove('active');
    document.body.style.overflow = 'auto';
}

// Cerrar modal al hacer clic fuera
document.addEventListener('click', function (e) {
    if (e.target.classList.contains('modal-overlay') && e.target.id !== 'qrModal') {
        e.target.classList.remove('active');
        document.body.style.overflow = 'auto';
    }
});

// Función para maximizar/minimizar el modal QR
function toggleMaximizeModal() {
    const modal = document.getElementById('qrModal');
    const maximizeBtn = document.getElementById('maximizeBtn');
    const maximizeIcon = maximizeBtn.querySelector('.material-icons');

    // Alternar la clase maximized
    modal.classList.toggle('maximized');

    // Cambiar el ícono y el título según el estado
    if (modal.classList.contains('maximized')) {
        maximizeIcon.textContent = 'fullscreen_exit';
        maximizeBtn.title = 'Minimizar';
    } else {
        maximizeIcon.textContent = 'fullscreen';
        maximizeBtn.title = 'Maximizar';
    }

    // Aplicar una transición suave al contenido del QR
    const qrCode = document.querySelector('.qr-code');
    if (qrCode) {
        qrCode.style.transition = 'width 0.3s ease, height 0.3s ease';
    }
}

// Efectos de hover para las tarjetas
document.addEventListener('DOMContentLoaded', function () {
    const actionCards = document.querySelectorAll('.action-card');

    actionCards.forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transform = 'translateY(-8px) scale(1.02)';
        });

        card.addEventListener('mouseleave', function () {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });
});

// Animaciones de entrada para los elementos
function animateOnScroll() {
    const elements = document.querySelectorAll('.action-card');

    elements.forEach((element, index) => {
        setTimeout(() => {
            element.style.opacity = '0';
            element.style.transform = 'translateY(40px)';
            element.style.transition = 'all 0.6s cubic-bezier(0.4, 0, 0.2, 1)';

            setTimeout(() => {
                element.style.opacity = '1';
                element.style.transform = 'translateY(0)';
            }, 100);
        }, index * 200);
    });
}

// Ejecutar animaciones cuando se muestra el dashboard
const originalShowDashboard = showDashboard;
showDashboard = function () {
    originalShowDashboard();
    setTimeout(animateOnScroll, 1300);
};

// Efecto de partículas para el fondo del login
function createParticles() {
    const loginContainer = document.querySelector('.login-container');

    for (let i = 0; i < 20; i++) {
        const particle = document.createElement('div');
        particle.style.position = 'absolute';
        particle.style.width = Math.random() * 4 + 2 + 'px';
        particle.style.height = particle.style.width;
        particle.style.background = 'rgba(255, 255, 255, 0.1)';
        particle.style.borderRadius = '50%';
        particle.style.left = Math.random() * 100 + '%';
        particle.style.top = Math.random() * 100 + '%';
        particle.style.animation = `float ${Math.random() * 6 + 4}s ease-in-out infinite`;
        particle.style.animationDelay = Math.random() * 2 + 's';

        loginContainer.appendChild(particle);
    }
}

// Animación de flotación para las partículas
const floatKeyframes = `
            @keyframes float {
                0%, 100% { transform: translateY(0px) rotate(0deg); opacity: 0.1; }
                50% { transform: translateY(-20px) rotate(180deg); opacity: 0.3; }
            }
        `;

const styleSheet = document.createElement('style');
styleSheet.textContent = floatKeyframes;
document.head.appendChild(styleSheet);

// Inicializar partículas al cargar la página
document.addEventListener('DOMContentLoaded', function () {
    createParticles();

    // Precargar íconos de Google Fonts
    const link = document.createElement('link');
    link.href = 'https://fonts.googleapis.com/icon?family=Material+Icons';
    link.rel = 'preload';
    link.as = 'style';
    document.head.appendChild(link);
});

// Ripple effect para botones
function createRipple(event) {
    const button = event.currentTarget;
    const circle = document.createElement('span');
    const diameter = Math.max(button.clientWidth, button.clientHeight);
    const radius = diameter / 2;

    circle.style.width = circle.style.height = `${diameter}px`;
    circle.style.left = `${event.clientX - button.offsetLeft - radius}px`;
    circle.style.top = `${event.clientY - button.offsetTop - radius}px`;
    circle.classList.add('ripple');

    const ripple = button.getElementsByClassName('ripple')[0];
    if (ripple) {
        ripple.remove();
    }

    button.appendChild(circle);
}

// Agregar efecto ripple a todos los botones
document.addEventListener('DOMContentLoaded', function () {
    const buttons = document.querySelectorAll('.btn, .google-login-btn, .action-card');
    buttons.forEach(button => {
        button.addEventListener('click', createRipple);
    });
});

// CSS para el efecto ripple
const rippleCSS = `
            .ripple {
                position: absolute;
                border-radius: 50%;
                transform: scale(0);
                animation: ripple 600ms linear;
                background-color: rgba(255, 255, 255, 0.3);
            }
            
            @keyframes ripple {
                to {
                    transform: scale(4);
                    opacity: 0;
                }
            }
        `;

const rippleStyleSheet = document.createElement('style');
rippleStyleSheet.textContent = rippleCSS;
document.head.appendChild(rippleStyleSheet);

// Funcionalidad de tema oscuro/claro (opcional)
function toggleTheme() {
    document.body.classList.toggle('dark-theme');
    localStorage.setItem('darkTheme', document.body.classList.contains('dark-theme'));
}

// Cargar tema guardado
document.addEventListener('DOMContentLoaded', function () {
    if (localStorage.getItem('darkTheme') === 'true') {
        document.body.classList.add('dark-theme');
    }
});

// Manejo de errores de red (simulado)
function handleNetworkError() {
    const errorModal = document.getElementById('errorModal');
    const errorMessage = document.getElementById('errorMessage');
    errorMessage.textContent = 'Error de conexión. Por favor, verifica tu conexión a internet e intenta nuevamente.';
    showModal('errorModal');
}


function handleCredentialResponse(response) {
    // Obtener el token ID de Google
    const idToken = response.credential;
    console.log('ID Token:', idToken);

    // Enviar el token al backend para verificación
    fetch('/api/auth/google', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ idToken: idToken })
    })
        .then(response => {
            console.log('Response status:', response.status);
            return response.json().then(data => ({
                status: response.status,
                data: data
            }));
        })
        .then(({ status, data }) => {
            if (status === 200) {
                // Éxito - Guardar el token JWT devuelto por el servidor
                localStorage.setItem('authToken', data.token);

                // Redirigir al usuario a la página principal o mostrar contenido autenticado
                document.querySelector('.login-container').style.display = 'none';
                document.querySelector('.dashboard').classList.add('active');

                // Actualizar la UI con la información del usuario
                if (data.user) {
                    almacenarDatoLS(data.user.email, data.user.picture, data.user.name);
                    cargarDatosUsuario();
                }
            } else {
                // Error del servidor - Mostrar mensaje específico
                console.error('Error de autenticación:', data);
                const errorModal = document.getElementById('errorModal');
                const errorMessage = document.getElementById('errorMessage');
                errorMessage.textContent = data.error || 'Error de autenticación. Por favor, intenta nuevamente.';
                showModal('errorModal');
            }
        })
        .catch(error => {
            console.error('Error al autenticar:', error);
            // Mostrar mensaje de error genérico
            const errorModal = document.getElementById('errorModal');
            const errorMessage = document.getElementById('errorMessage');
            errorMessage.textContent = 'Error de conexión. Por favor, intenta nuevamente.';
            showModal('errorModal');
        })
        .finally(() => {
            resetGoogleButton();
        });
}

// Función para realizar solicitudes autenticadas
function fetchAuthenticatedAPI(url, options = {}) {
    const token = localStorage.getItem('authToken');
    if (!token) {
        // Redirigir a login si no hay token
        document.querySelector('.dashboard').classList.remove('active');
        document.querySelector('.login-container').style.display = 'flex';
        return Promise.reject('No autenticado');
    }

    // Agregar token a las cabeceras
    const headers = options.headers || {};
    headers['Authorization'] = "Bearer " + token;
    return fetch(url, {
        ...options,
        headers
    });
}

// historial regitroszzz
function cargarDatosEstudiante(id) {
    fetchAuthenticatedAPI(`/api/Home/estudiante/${id}`)
        .then(response => response.json())
        .then(data => {
            console.log('Datos del estudiante:', data);
            // Actualizar la UI con los datos
        })
        .catch(error => {
            console.error('Error:', error);
        });
}

function logout() {
    document.querySelector('.login-container').style.display = 'flex';
    document.querySelector('.dashboard').classList.remove('active');
    localStorage.removeItem('authToken');
    localStorage.removeItem('userEmail');
    localStorage.removeItem('userPicture');
    localStorage.removeItem('userName');
}

function almacenarDatoLS(emailLS, pictureLS, nameLS){

    localStorage.setItem('userEmail', emailLS);
    localStorage.setItem('userPicture', pictureLS);
    localStorage.setItem('userName', nameLS);
}

function cargarDatosUsuario() {

    const pictureLS = localStorage.getItem('userPicture');
    const nameLS = localStorage.getItem('userName');
    const emailLS = localStorage.getItem('userEmail');


    if (pictureLS && nameLS && emailLS) {
        document.getElementById('userName').textContent = nameLS;
        document.getElementById('userEmail').textContent = emailLS;
        if (pictureLS) {
            document.getElementById('userAvatar').innerHTML = `<img src="${pictureLS}" alt="${nameLS}">`;
        } else {
            const initials = data.user.name.split(' ').map(n => n[0]).join('');
            document.getElementById('userAvatar').textContent = initials;
        }
    }
}
