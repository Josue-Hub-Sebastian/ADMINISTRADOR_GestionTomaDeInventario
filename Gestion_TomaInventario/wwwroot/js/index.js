function filtrarEmpresas(filtro) {
    const cards = document.querySelectorAll('.empresa-card');
    const btnTodas = document.getElementById('btnTodas');
    const btnActivas = document.getElementById('btnActivas');
    const btnInactivas = document.getElementById('btnInactivas');
    const visibleCountSpan = document.getElementById('visibleCount');

    let visibleCount = 0;
    const totalCount = cards.length;

    // Remover clase active de todos los botones
    [btnTodas, btnActivas, btnInactivas].forEach(btn => {
        if (btn) btn.classList.remove('active');
    });

    // Aplicar filtro
    switch (filtro) {
        case 'todas':
            if (btnTodas) btnTodas.classList.add('active');
            cards.forEach(card => {
                card.classList.remove('hidden-card');
                visibleCount++;
            });
            break;

        case 'activas':
            if (btnActivas) btnActivas.classList.add('active');
            cards.forEach(card => {
                if (card.getAttribute('data-estado') === 'activa') {
                    card.classList.remove('hidden-card');
                    visibleCount++;
                } else {
                    card.classList.add('hidden-card');
                }
            });
            break;

        case 'inactivas':
            if (btnInactivas) btnInactivas.classList.add('active');
            cards.forEach(card => {
                if (card.getAttribute('data-estado') === 'inactiva') {
                    card.classList.remove('hidden-card');
                    visibleCount++;
                } else {
                    card.classList.add('hidden-card');
                }
            });
            break;
    }

    // Actualizar contador
    if (visibleCountSpan) {
        visibleCountSpan.textContent = visibleCount;
    }

    // Animar cards visibles secuencialmente
    const visibleCards = document.querySelectorAll('.empresa-card:not(.hidden-card)');
    visibleCards.forEach((card, index) => {
        card.style.animation = 'none';
        card.offsetHeight; // Trigger reflow
        card.style.animation = `fadeInUp 0.35s ease ${index * 0.05}s both`;
    });
}


