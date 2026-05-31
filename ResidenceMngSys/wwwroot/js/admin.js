document.addEventListener('DOMContentLoaded', () => {

    // 1. Sidebar Navigation Logic
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

                // Re-render charts if overview is active to fix potential sizing issues
                if (item.getAttribute('data-target') === 'overview') {
                    // Normally we might need to update chart size, but Chartjs handles resize mostly.
                }
            }
        });
    });

    // 2. Chart.js Initialization
    initCharts();

    // 3. Simple Modal Logic Config
    setupModal('newResidentBtn', 'newResidentModal', 'closeResidentModal');
    setupModal('newDueBtn', 'newDueModal', 'closeDueModal');
    setupModal('newAnnouncementBtn', 'newAnnouncementModal', 'closeAnnouncementModal');

});

function initCharts() {
    //const ctxPayments = document.getElementById('paymentsChart');
    const ctxDebt = document.getElementById('debtChart');

    // Chart defaults for premium look
    Chart.defaults.font.family = "'Inter', sans-serif";
    Chart.defaults.color = "#64748B"; // var(--text-muted)

    if (ctxPayments) {
        new Chart(ctxPayments, {
            type: 'bar',
            data: {
                labels: ['Oca', 'Şub', 'Mar', 'Nis', 'May', 'Haz'],
                datasets: [{
                    label: 'Tahsil Edilen (₺)',
                    data: [12000, 15000, 14500, 18000, 16000, 21000],
                    backgroundColor: '#4F46E5', // Primary
                    borderRadius: 6,
                    barPercentage: 0.6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: { legend: { display: false } },
                scales: {
                    y: {
                        beginAtZero: true,
                        grid: { color: '#E2E8F0', drawBorder: false }
                    },
                    x: {
                        grid: { display: false, drawBorder: false }
                    }
                }
            }
        });
    }

    if (ctxDebt) {
        new Chart(ctxDebt, {
            type: 'doughnut',
            data: {
                labels: ['Ödenen', 'Gecikmiş', 'Bekleyen'],
                datasets: [{
                    data: [124500, 12000, 6200],
                    backgroundColor: [
                        '#10B981', // Success
                        '#EF4444', // Danger
                        '#F59E0B'  // Warning
                    ],
                    borderWidth: 0,
                    hoverOffset: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '75%',
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: { usePointStyle: true, padding: 20 }
                    }
                }
            }
        });
    }
}

function setupModal(btnId, modalId, closeBtnId) {
    const btn = document.getElementById(btnId);
    const modal = document.getElementById(modalId);
    const closeBtn = document.getElementById(closeBtnId);

    if (!btn || !modal || !closeBtn) return;

    btn.addEventListener('click', () => {
        modal.classList.add('active');
    });

    closeBtn.addEventListener('click', () => {
        modal.classList.remove('active');
    });

    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            modal.classList.remove('active');
        }
    });
}



const dueBtn = document.getElementById('newDueBtn');
if (dueBtn) {
    dueBtn.addEventListener('click', function () {
        document.getElementById('newDueModal').style.display = 'flex';
    });

    document.getElementById('closeDueModal').addEventListener('click', function () {
        document.getElementById('newDueModal').style.display = 'none';
    });

    document.getElementById('newDueModal').addEventListener('click', function (e) {
        if (e.target === this) {
            this.style.display = 'none';
        }
    });
}

