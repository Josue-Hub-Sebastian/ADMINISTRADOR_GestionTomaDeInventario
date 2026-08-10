document.addEventListener("DOMContentLoaded", () => {
    const panelSinContacto = document.getElementById("panelSinContacto");
    const panelFormulario = document.getElementById("panelFormulario");
    const btnNuevo = document.getElementById("btnNuevoContacto");
    const btnEditar = document.getElementById("btnEditarContacto");
    const btnCancelar = document.getElementById("btnCancelarContacto");
    const acciones = document.getElementById("accionesContacto");
    const frm = document.getElementById("frmContacto");
    if (!frm)
        return;

    const inputs = frm.querySelectorAll(".contacto-input");
    const estadoInicial = {};
    inputs.forEach(i => estadoInicial[i.name] = i.value);

    function habilitarFormulario() {
        inputs.forEach(i => {
            i.readOnly = false;
            i.classList.remove("bg-light");
        });
        acciones.classList.remove("d-none");
        if (btnEditar)
            btnEditar.classList.add("d-none");
    }

    function bloquearFormulario() {
        inputs.forEach(i => {
            i.readOnly = true;
            i.classList.add("bg-light");
            i.classList.remove("is-invalid");
        });
        if (acciones)
            acciones.classList.add("d-none");
        if (btnEditar)
            btnEditar.classList.remove("d-none");
    }

    if (btnNuevo) {
        btnNuevo.addEventListener("click", () => {
            panelSinContacto.classList.add("d-none");
            panelFormulario.classList.remove("d-none");
            habilitarFormulario();
        });
    }

    if (btnEditar) {
        btnEditar.addEventListener("click", () => {
            habilitarFormulario();
        });
    }

    if (btnCancelar) {
        btnCancelar.addEventListener("click", () => {
            Object.keys(estadoInicial).forEach(k => {
                frm.elements[k].value = estadoInicial[k];
            });
            inputs.forEach(i => i.classList.remove("is-invalid"));
            bloquearFormulario();
        });
    }

    bloquearFormulario();

    frm.addEventListener("submit", async function (e) {
        e.preventDefault();

        // Validación nativa antes de pegarle al servidor
        if (!frm.checkValidity()) {
            inputs.forEach(i => {
                i.classList.toggle("is-invalid", !i.checkValidity());
            });
            frm.classList.add("was-validated");
            return;
        }
        frm.classList.remove("was-validated");

        const btnGuardar = frm.querySelector("button.btn-success");
        const textoOriginal = btnGuardar.innerHTML;
        const form = new FormData(frm);

        btnGuardar.disabled = true;
        btnGuardar.innerHTML = `<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Guardando...`;

        try {
            const response = await fetch("/Empresa/GuardarContacto", {
                method: "POST",
                body: form
            });

            if (response.ok) {
                await Swal.fire({
                    icon: "success",
                    title: "Contacto guardado",
                    text: "La información fue actualizada correctamente."
                });
                location.reload();
                return; // evitamos re-habilitar el btn antes del reload
            }

            let mensaje = "No fue posible guardar el contacto.";
            try {
                const data = await response.json();
                if (data && data.message) mensaje = data.message;
            } catch {
                const texto = await response.text().catch(() => null);
                if (texto) mensaje = texto;
            }

            await Swal.fire({
                icon: "error",
                title: "Error",
                text: mensaje
            });
        } catch (err) {
            await Swal.fire({
                icon: "error",
                title: "Error de conexión",
                text: "No se pudo contactar al servidor. Verifica tu conexión e intenta de nuevo."
            });
        } finally {
            btnGuardar.disabled = false;
            btnGuardar.innerHTML = textoOriginal;
        }
    });
});