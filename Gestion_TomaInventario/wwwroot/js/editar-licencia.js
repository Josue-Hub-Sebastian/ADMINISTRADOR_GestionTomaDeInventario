
// Función para actualizar el estilo según el estado seleccionado
function actualizarEstiloEstado() {
    const select = document.getElementById("selectEstado");
    const badge = document.getElementById("estadoBadge");
    const badgeTexto = document.getElementById("estadoBadgeTexto");
    const icon = badge.querySelector("i");
    const gifImg = document.getElementById("gifEstadoImg");
    const gifBox = document.getElementById("gifEstadoBox");
    const miniEstadoBadge = document.getElementById("miniEstadoBadge");

    // Rutas de las imágenes de estado
    const gifActivo = '/images/habilitado_img.png';
    const gifInactivo = '/images/deshabilitado_png.png';

    if (select.value === "true") {
        // Estado ACTIVO
        badge.classList.remove("inactiva");
        badge.classList.add("activa");
        icon.className = "bi bi-check-circle-fill";
        badgeTexto.textContent = "Activa";

        gifImg.src = gifActivo;
        gifBox.style.backgroundColor = "#e8f9ec";
        gifBox.style.borderColor = "#c3e6cb";

        // Actualizar mini card si existe
        if (miniEstadoBadge) {
            miniEstadoBadge.className = "badge bg-success";
            miniEstadoBadge.textContent = "Activa";
        }
    } else {
        // Estado INACTIVO
        badge.classList.remove("activa");
        badge.classList.add("inactiva");
        icon.className = "bi bi-x-circle-fill";
        badgeTexto.textContent = "Inactiva";

        gifImg.src = gifInactivo;
        gifBox.style.backgroundColor = "#fdf2f2";
        gifBox.style.borderColor = "#f5c6cb";

        // Actualizar mini card si existe
        if (miniEstadoBadge) {
            miniEstadoBadge.className = "badge bg-danger";
            miniEstadoBadge.textContent = "Inactiva";
        }
    }
}

// Manejar el envío del formulario con SweetAlert2
function configurarEnvioFormulario() {
    const form = document.getElementById("formEditarLicencia");

    if (!form) return;

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        Swal.fire({
            title: "¿Guardar cambios?",
            text: "Se actualizarán los límites del plan para este cliente.",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Sí, guardar",
            cancelButtonText: "Cancelar",
            confirmButtonColor: "#4e73df",
            cancelButtonColor: "#6c757d",
            reverseButtons: true
        }).then(function (result) {
            if (result.isConfirmed) {
                // Mostrar loading
                Swal.fire({
                    title: "Guardando...",
                    text: "Por favor espere",
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });

                // Reemplazar la llamada fetch actual por esta para debug
                fetch(form.action, {
                    method: 'POST',
                    credentials: 'same-origin',               // envía cookies (auth + antiforgery)
                    body: new FormData(form),
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value,
                        'X-Requested-With': 'XMLHttpRequest' // identifica petición AJAX en el servidor
                    }
                })
                    .then(async response => {
                        const text = await response.text();
                        console.log('POST', form.action, 'status=', response.status, 'body=', text);
                        if (response.ok) {
                            await Swal.fire({ icon: 'success', title: '¡Listo!', timer: 1200, showConfirmButton: false });
                            window.location.href = '/Empresa/Licencias';
                            return;
                        }
                        let mensaje = text;
                        try { mensaje = JSON.parse(text).message ?? text; } catch { }
                        Swal.fire({ icon: 'error', title: `Error ${response.status}`, text: mensaje });
                    })
                    .catch(err => {
                        console.error('Fetch error:', err);
                        Swal.fire({ icon: 'error', title: 'Error de conexión', text: String(err) });
                    });
            }
        });
    });
}

// Inicializar cuando el DOM esté listo
document.addEventListener("DOMContentLoaded", function () {
    const ddlPlan = document.getElementById("IdPlan");
    const chkPersonalizado = document.getElementById("EsPersonalizado");

    const txtProductos = document.getElementById("ProductosMax");
    const txtAlmacenes = document.getElementById("AlmacenesMax");
    const txtUbicaciones = document.getElementById("UbicacionesMax");
    const txtInventarios = document.getElementById("InventariosPreparadosMax");
    const txtUsuariosAdmin = document.getElementById("UsuariosAdminMax");
    const txtUsuariosOperador = document.getElementById("UsuariosOperadorMax");

    const txtInicio = document.getElementById("InicioSuscripcion");
    const txtMeses = document.getElementById("MesesContratados");
    const txtFin = document.getElementById("FinSuscripcion");

    const camposLimite = [
        txtProductos,
        txtAlmacenes,
        txtUbicaciones,
        txtInventarios,
        txtUsuariosAdmin,
        txtUsuariosOperador
    ];

    function calcularFinSuscripcion() {

        if (!txtInicio || !txtMeses || !txtFin)
            return;

        const inicio = txtInicio.value;
        const meses = parseInt(txtMeses.value);

        if (!inicio || isNaN(meses) || meses <= 0) {
            txtFin.value = "";
            return;
        }

        const fecha = new Date(inicio + "T00:00:00");
        fecha.setMonth(fecha.getMonth() + meses);

        const yyyy = fecha.getFullYear();
        const mm = String(fecha.getMonth() + 1).padStart(2, "0");
        const dd = String(fecha.getDate()).padStart(2, "0");

        txtFin.value = `${yyyy}-${mm}-${dd}`;
    }

    function setCamposReadonly(readonly) {

        camposLimite.forEach(campo => {

            if (!campo)
                return;

            campo.readOnly = readonly;
            campo.classList.toggle("bg-light", readonly);
        });

    }
    function limpiarCamposLimite() {

        camposLimite.forEach(campo => {

            if (campo)
                campo.value = "";

        });

    }

    function cargarPlanSeleccionado() {

        if (!ddlPlan)
            return;

        const opcion = ddlPlan.options[ddlPlan.selectedIndex];

        if (!opcion || !opcion.value) {
            limpiarCamposLimite();
            return;
        }

        txtAlmacenes.value = opcion.dataset.almacenes || "";
        txtUbicaciones.value = opcion.dataset.ubicaciones || "";
        txtProductos.value = opcion.dataset.productos || "";
        txtUsuariosAdmin.value = opcion.dataset.admin || "";
        txtUsuariosOperador.value = opcion.dataset.operador || "";
        txtInventarios.value = opcion.dataset.inventarios || "";

    }

    function aplicarModoEdicion() {

        const personalizado = chkPersonalizado.checked;

        if (personalizado) {

            // NO BORRAR EL PLAN
            ddlPlan.disabled = true;
            // Permitir editar limites
            setCamposReadonly(false);

        }
        else {
            ddlPlan.disabled = false;

            if (ddlPlan.value) {
                cargarPlanSeleccionado();
                setCamposReadonly(true);
            }
            else {
                limpiarCamposLimite();
                setCamposReadonly(false);
            }
        }
    }

    
    if (ddlPlan) {

        ddlPlan.addEventListener("change", function () {
            if (chkPersonalizado.checked)
                return;

            cargarPlanSeleccionado();

            if (ddlPlan.value)
                setCamposReadonly(true);
            else
                setCamposReadonly(false);
        });
    }

    if (chkPersonalizado) {
        chkPersonalizado.addEventListener("change", aplicarModoEdicion);
    }

    if (txtInicio)
        txtInicio.addEventListener("change", calcularFinSuscripcion);

    if (txtMeses)
        txtMeses.addEventListener("input", calcularFinSuscripcion);

    
    aplicarModoEdicion();
    calcularFinSuscripcion();
    configurarEnvioFormulario();
});





//document.addEventListener("DOMContentLoaded", function () {

//    const inicio = document.getElementById("InicioSuscripcion");
//    const meses = document.getElementById("MesesContratados");
//    const fin = document.getElementById("FinSuscripcion");

//    function calcularFechaFin() {

//        if (!inicio.value || !meses.value) {
//            fin.value = "";
//            return;
//        }

//        const partes = inicio.value.split("-");

//        let fecha = new Date(
//            parseInt(partes[0]),       // año
//            parseInt(partes[1]) - 1,   // mes (0-11)
//            parseInt(partes[2])        // día
//        );

//        fecha.setMonth(fecha.getMonth() + parseInt(meses.value));

//        const año = fecha.getFullYear();
//        const mes = String(fecha.getMonth() + 1).padStart(2, "0");
//        const dia = String(fecha.getDate()).padStart(2, "0");

//        fin.value = `${año}-${mes}-${dia}`;
//    }

//    inicio.addEventListener("change", calcularFechaFin);
//    meses.addEventListener("input", calcularFechaFin);

//    calcularFechaFin();
//});



//document.addEventListener("DOMContentLoaded", function () {
//    const inicioInput = document.getElementById("InicioSuscripcion");
//    const mesesInput = document.getElementById("MesesContratados")
//    const finInput = document.getElementById("FinSuscripcion"); 

//    function calcularFinSuscripcion() {
//        const inicio = inicioInput.value;
//        const meses = parseInt(mesesInput.value);

//        if (!inicio || isNaN(meses) || meses <= 0) {
//            finInput.value = "";
//            return;
//        }

//        const fechaInicio = new Date(inicio + "T00:00:00");
//        fechaInicio.setMonth(fechaInicio.getMonth() + meses);

//        const anio = fechaInicio.getFullYear();
//        const mes = String(fechaInicio.getMonth() + 1).padStart(2, "0");
//        const dia = String(fechaInicio.getDate()).padStart(2, "0");

//        finInput.value = `${anio}-${mes}-${dia}`;
//    }

//    inicioInput.addEventListener("change", calcularFinSuscripcion);
//    mesesInput.addEventListener("input", calcularFinSuscripcion);

//    calcularFinSuscripcion();



//});