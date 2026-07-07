document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const clearSearch = document.getElementById('clearSearch');
    const table = document.getElementById('tblLicencias');

    if (!table) return;

    const rows = table.querySelectorAll('tbody tr');
    const totalLicenciasSpan = document.getElementById('contadorLicencias');
    const resultadoBusqueda = document.getElementById('resultadoBusqueda');
    const textoResultado = document.getElementById('textoResultado');
    const noResultsDiv = document.getElementById('noResults');
    const terminoBusqueda = document.getElementById('terminoBusqueda');

    // Función de búsqueda
    function buscarLicencias() {
        const searchTerm = searchInput.value.toLowerCase().trim();
        let visibleCount = 0;
        let found = false;

        // Mostrar/ocultar botón de limpiar
        if (searchTerm.length > 0) {
            clearSearch.classList.add('visible');
        } else {
            clearSearch.classList.remove('visible');
        }

        rows.forEach(row => {
            const searchData = row.getAttribute('data-search');

            if (searchTerm === '') {
                // Si no hay búsqueda, mostrar todo
                row.classList.remove('row-hidden');
                visibleCount++;
                limpiarResaltado(row)
                found = true;
            } else {
                // Búsqueda tipo LIKE: busca la palabra en cualquier parte del texto
                if (searchData && searchData.includes(searchTerm)) {
                    row.classList.remove('row-hidden');
                    visibleCount++;
                    found = true;

                    // Resaltar coincidencia (opcional)
                    resaltarCoincidencia(row, searchTerm);
                } else {
                    row.classList.add('row-hidden');
                    // Limpiar resaltado
                    limpiarResaltado(row);
                }
            }
        });

        // Actualizar contador
        if (totalLicenciasSpan) {
            totalLicenciasSpan.textContent = visibleCount;
        }

        // Mostrar/ocultar mensaje de "sin resultados"
        if (!found && searchTerm !== '') {
            noResultsDiv.style.display = 'block';
            terminoBusqueda.textContent = '"' + searchInput.value + '"';
            table.style.display = 'none';
        } else {
            noResultsDiv.style.display = 'none';
            table.style.display = '';
        }

        // Mostrar resultado de búsqueda
        if (searchTerm !== '') {
            resultadoBusqueda.style.display = 'block';
            textoResultado.textContent = visibleCount + ' de ' + rows.length + ' resultados';
        } else {
            resultadoBusqueda.style.display = 'none';
        }
    }

    // Función para resaltar coincidencias
    function resaltarCoincidencia(row, term) {
        const nombreEmpresa = row.querySelector('.nombre-empresa');
        const rucEmpresa = row.querySelector('.ruc-empresa');
        const bdEmpresa = row.querySelector('.bd-empresa');

        [nombreEmpresa, bdEmpresa].forEach(element => {
            if (element) {
                const originalText = element.getAttribute('data-original') || element.textContent;
                if (!element.getAttribute('data-original')) {
                    element.setAttribute('data-original', originalText);
                }

                const regex = new RegExp(`(${escapeRegExp(term)})`, 'gi');
                element.innerHTML = originalText.replace(regex, '<span class="highlight-match">$1</span>');
            }
        });
    }

    // Función para limpiar resaltado
    function limpiarResaltado(row) {
        const elements = row.querySelectorAll('[data-original]');
        elements.forEach(element => {
            element.textContent = element.getAttribute('data-original');
            element.removeAttribute('data-original');
        });
    }

    // Escapar caracteres especiales para RegExp
    function escapeRegExp(string) {
        return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }

    // Evento de búsqueda en tiempo real
    searchInput.addEventListener('input', function () {
        buscarLicencias();
    });

    // También buscar al presionar Enter
    searchInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            buscarLicencias();
        }
    });

    // Función para limpiar búsqueda
    window.limpiarBusqueda = function () {
        searchInput.value = '';
        clearSearch.classList.remove('visible');
        buscarLicencias();
        searchInput.focus();
    };

    // Limpiar búsqueda con Escape
    searchInput.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            limpiarBusqueda();
        }
    });
});