// wwwroot/js/profile.js

document.addEventListener('DOMContentLoaded', function () {
    // Animate profile card on load
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

    // Add loading animation to AI Insights button
    const insightsForm = document.querySelector('.insights-form');
    const insightsButton = document.querySelector('.btn-insights');

    if (insightsForm && insightsButton) {
        insightsForm.addEventListener('submit', function () {
            if (insightsButton && !insightsButton.disabled) {
                // Store original button text
                const originalText = insightsButton.innerHTML;

                // Add loading class and change text
                insightsButton.classList.add('loading');
                insightsButton.innerHTML = '<i class="bi bi-stars"></i> Analyzing your health data...';

                // Store original text to restore if needed (optional)
                insightsButton.setAttribute('data-original-text', originalText);
            }
        });
    }
});