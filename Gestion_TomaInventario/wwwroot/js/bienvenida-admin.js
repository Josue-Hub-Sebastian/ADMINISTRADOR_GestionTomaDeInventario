/**
 * Panel General - Dashboard
 * Fecha, Hora y Clima en tiempo real
 * Sin parpadeos, con actualizaciones suaves ✨
 */

document.addEventListener("DOMContentLoaded", function () {
    // Referencias a elementos del DOM
    const fechaEl = document.getElementById("pgFecha");
    const horaEl = document.getElementById("pgHora");
    const weatherIcon = document.getElementById("pgWeatherIcon");
    const weatherTemp = document.getElementById("pgWeatherTemp");
    const weatherDesc = document.getElementById("pgWeatherDesc");

    // Variables de control
    let intervaloReloj = null;
    let intervaloClima = null;
    let ultimaHoraClima = null;

    // Inicialización
    if (fechaEl && horaEl) {
        iniciarReloj();
    }

    if (weatherIcon && weatherTemp && weatherDesc) {
        obtenerClima(); // Primera carga
        intervaloClima = setInterval(verificarActualizacionClima, 60000); // Verificar cada minuto
    }

    /**
     * Inicia el reloj sin parpadeos
     */
    function iniciarReloj() {
        const actualizarReloj = () => {
            const ahora = new Date();

            // Actualizar fecha (solo cuando cambie)
            const fechaStr = ahora.toLocaleDateString("es-PE", {
                weekday: "long",
                year: "numeric",
                month: "long",
                day: "numeric"
            });

            if (fechaEl.textContent !== fechaStr) {
                fechaEl.textContent = fechaStr;
            }

            // Actualizar hora (cada segundo, sin parpadeo)
            const horas = String(ahora.getHours()).padStart(2, '0');
            const minutos = String(ahora.getMinutes()).padStart(2, '0');
            const segundos = String(ahora.getSeconds()).padStart(2, '0');
            const horaStr = `${horas}:${minutos}:${segundos}`;

            // Solo actualizar el texto, sin animaciones que causen parpadeo
            if (horaEl.textContent !== horaStr) {
                horaEl.textContent = horaStr;
            }
        };

        // Iniciar inmediatamente
        actualizarReloj();

        // Actualizar cada segundo
        intervaloReloj = setInterval(actualizarReloj, 1000);
    }

    /**
     * Verifica si necesita actualizar el clima (cada hora)
     */
    function verificarActualizacionClima() {
        const ahora = new Date();
        const horaActual = ahora.getHours();

        // Si no hay registro o cambió la hora
        if (ultimaHoraClima === null || ultimaHoraClima !== horaActual) {
            obtenerClima();
        }
    }

    /**
     * Obtiene el clima según la hora del día
     * Simulación basada en hora para demo
     * Reemplazar con API real si es necesario
     */
    function obtenerClima() {
        const ahora = new Date();
        const hora = ahora.getHours();
        ultimaHoraClima = hora;

        // Determinar clima según hora del día
        let climaData = getClimaPorHora(hora);

        // Actualizar UI sin parpadeo
        actualizarClimaUI(climaData);
    }

    /**
     * Retorna datos del clima según la hora
     * Estructura de if anidados para determinar condiciones
     */
    function getClimaPorHora(hora) {
        // Simulación de temperatura base (Lima, Perú)
        let temperatura = 22;
        let icono = '☀️';
        let descripcion = 'Soleado';

        if (hora >= 0 && hora < 6) {
            // MADRUGADA: 00:00 - 05:59
            if (hora < 3) {
                temperatura = 18;
                icono = '🌙';
                descripcion = 'Noche clara';
            } else {
                temperatura = 17;
                icono = '🌌';
                descripcion = 'Madrugada fresca';
            }
        } else if (hora >= 6 && hora < 12) {
            // MAÑANA: 06:00 - 11:59
            if (hora < 8) {
                temperatura = 19;
                icono = '🌅';
                descripcion = 'Amanecer';
            } else if (hora < 10) {
                temperatura = 21;
                icono = '🌤️';
                descripcion = 'Parcialmente nublado';
            } else {
                temperatura = 23;
                icono = '☀️';
                descripcion = 'Soleado';
            }
        } else if (hora >= 12 && hora < 18) {
            // TARDE: 12:00 - 17:59
            if (hora < 14) {
                temperatura = 25;
                icono = '🌞';
                descripcion = 'Caluroso';
            } else if (hora < 16) {
                temperatura = 24;
                icono = '⛅';
                descripcion = 'Nublado';
            } else {
                temperatura = 22;
                icono = '🌤️';
                descripcion = 'Atardecer';
            }
        } else if (hora >= 18 && hora < 24) {
            // NOCHE: 18:00 - 23:59
            if (hora < 20) {
                temperatura = 20;
                icono = '🌅';
                descripcion = 'Ocaso';
            } else if (hora < 22) {
                temperatura = 19;
                icono = '🌆';
                descripcion = 'Anochecer';
            } else {
                temperatura = 18;
                icono = '🌙';
                descripcion = 'Noche despejada';
            }
        }

        // Variación aleatoria sutil (±2 grados) para realismo
        temperatura += Math.floor(Math.random() * 3) - 1;

        return {
            temperatura: temperatura,
            icono: icono,
            descripcion: descripcion
        };
    }

    /**
     * Actualiza la UI del clima sin parpadeos
     */
    function actualizarClimaUI(climaData) {
        if (!weatherIcon || !weatherTemp || !weatherDesc) return;

        // Solo actualizar si cambió
        if (weatherIcon.textContent !== climaData.icono) {
            weatherIcon.textContent = climaData.icono;
        }

        const tempStr = `${climaData.temperatura}°C`;
        if (weatherTemp.textContent !== tempStr) {
            weatherTemp.textContent = tempStr;
        }

        if (weatherDesc.textContent !== climaData.descripcion) {
            weatherDesc.textContent = climaData.descripcion;
        }
    }

    // Limpiar intervalos al salir de la página (buena práctica)
    window.addEventListener('beforeunload', () => {
        if (intervaloReloj) clearInterval(intervaloReloj);
        if (intervaloClima) clearInterval(intervaloClima);
    });
});