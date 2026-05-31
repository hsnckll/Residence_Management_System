document.addEventListener('DOMContentLoaded', () => {
    // Sidebar Navigation Logic for Resident Panel
    const navItems = document.querySelectorAll('.nav-item');
    const sections = document.querySelectorAll('.panel-section');

    navItems.forEach(item => {
        item.addEventListener('click', () => {
            // Remove active class from all nav items
            navItems.forEach(nav => nav.classList.remove('active'));
            // Add active class to clicked
            item.classList.add('active');

            // Hide all sections
            sections.forEach(sec => sec.classList.remove('active'));

            // Show target section
            const targetId = 's-' + item.getAttribute('data-target');
            const targetSection = document.getElementById(targetId);
            if (targetSection) {
                targetSection.classList.add('active');
            }
        });
    });
});
