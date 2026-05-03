
// Handle button clicks (reload page with query parameter)
document.querySelectorAll('.pages .page-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        if (this.disabled) return;
        const page = this.dataset.page;
        if (page) {
            const url = new URL(window.location.href);
            url.searchParams.set('page', page);
            window.location.href = url; // reload page
        }
    });
});

