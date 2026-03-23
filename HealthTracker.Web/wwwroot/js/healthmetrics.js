// wwwroot/js/healthmetrics.js

// Show success message
document.addEventListener('DOMContentLoaded', function () {
    const successMessage = document.getElementById('successMessage');
    if (successMessage && successMessage.value) {
        alert(successMessage.value);
    }
});

// Delete confirmation function
window.confirmDelete = function () {
    return confirm('Are you sure you want to delete this record?');
};