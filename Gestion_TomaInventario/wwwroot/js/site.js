document.addEventListener("DOMContentLoaded", function () {
    const appShell = document.getElementById("appShell");
    const sidebarToggle = document.getElementById("sidebarToggle");
    const menuGestion = document.getElementById("menuGestion");
    const btnGestion = document.getElementById("btnGestion");

    const loginForm = document.getElementById("loginForm");
    const loginButton = document.getElementById("loginButton");

    if (appShell && sidebarToggle) {
        const savedState = localStorage.getItem("gestion.sidebarCollapsed");

        if (savedState === "true") {
            appShell.classList.add("sidebar-collapsed");
            sidebarToggle.setAttribute("aria-label", "Expandir menú");
        }

        sidebarToggle.addEventListener("click", function () {
            appShell.classList.toggle("sidebar-collapsed");
            const collapsed = appShell.classList.contains("sidebar-collapsed");

            localStorage.setItem("gestion.sidebarCollapsed", collapsed.toString());
            sidebarToggle.setAttribute("aria-label", collapsed ? "Expandir menú" : "Contraer menú");

            if (collapsed && menuGestion) {
                const collapse = bootstrap.Collapse.getOrCreateInstance(menuGestion);
                collapse.hide();
            }
        });

        if (btnGestion) {
            btnGestion.addEventListener("click", function (e) {
                if (appShell.classList.contains("sidebar-collapsed")) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
        }
    }

    if (loginForm && loginButton) {
        loginForm.addEventListener("submit", function () {
            if (!loginForm.checkValidity()) {
                return;

                if (typeof $ === "function" && typeof $(loginForm).valid === "function") {
                    $(loginForm).validate();
                    $(loginForm).valid();
                }
                loginForm.classList.add("is-submitting");
                loginButton.disabled = true;
                return
            }
            loginForm.classList.add("is-submitting")
            loginButton.disabled = true;
        });
    }
});