// wwwroot/js/profile.js

// Add any profile-specific JavaScript here
document.addEventListener('DOMContentLoaded', function () {
    // Example: Add animation to profile card
    const profileCard = document.querySelector('.profile-card');
    if (profileCard) {
        profileCard.style.opacity = '0';
        profileCard.style.transform = 'translateY(20px)';

        setTimeout(() => {
            profileCard.style.transition = 'all 0.5s ease';
            profileCard.style.opacity = '1';
            profileCard.style.transform = 'translateY(0)';
        }, 100);
    }
});