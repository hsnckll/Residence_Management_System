document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');
    const roleSelect = document.getElementById('role');
    const usernameInput = document.getElementById('username');
    const passwordInput = document.getElementById('password');
    const errorMessage = document.getElementById('errorMessage');

    loginForm.addEventListener('submit', (e) => {
        // e.preventDefault();

        // Reset error message state
        errorMessage.style.display = 'none';

        const role = roleSelect.value;
        const username = usernameInput.value.trim();
        const password = passwordInput.value.trim();

        // Basic mock authentication:
        // If Admin -> username: admin, pass: admin
        // If Resident -> username: resident, pass: resident
        // For demo purposes, we will just accept anything and redirect based on role.

        if (username && password) {
            // Simulate loading state
            const btn = loginForm.querySelector('.login-btn');
            const originalText = btn.innerHTML;
            btn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i><span>Giriş Yapılıyor...</span>';

            setTimeout(() => {
                if (role === 'admin') {
                    window.location.href = 'admin.html';
                } else {
                    window.location.href = 'resident.html';
                }
            }, 800);

        } else {
            errorMessage.style.display = 'block';
        }
    });
});
