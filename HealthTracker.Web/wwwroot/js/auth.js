// wwwroot/js/auth.js
function checkPasswordMatch() {
    var password = document.getElementById("password");
    var confirmPassword = document.getElementById("confirmPassword");
    var message = document.getElementById("passwordMatchMessage");
    var submitBtn = document.getElementById("submitBtn");

    if (password && confirmPassword && message && submitBtn) {
        if (password.value !== confirmPassword.value) {
            message.textContent = "Passwords do not match!";
            submitBtn.disabled = true;
        } else {
            message.textContent = "";
            submitBtn.disabled = false;
        }
    }
}

// Initialize when page loads
document.addEventListener('DOMContentLoaded', function () {
    // Add event listener for confirm password field
    var confirmPassword = document.getElementById("confirmPassword");
    if (confirmPassword) {
        confirmPassword.addEventListener('keyup', checkPasswordMatch);
    }

    // Also check when password changes
    var password = document.getElementById("password");
    if (password) {
        password.addEventListener('change', checkPasswordMatch);
    }
});