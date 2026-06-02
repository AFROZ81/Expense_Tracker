// Premium Custom Global Toast System
function showToast(message, type = 'success') {
    // Normalize type to lowercase
    const normType = type.toLowerCase() === 'failure' ? 'error' : type.toLowerCase();
    
    // Ensure toast container exists
    let container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        document.body.appendChild(container);
    }

    // Determine icon and title based on toast type
    let iconSvg = '';
    let titleText = 'Notification';

    if (normType === 'success') {
        titleText = 'Success';
        iconSvg = `
            <svg viewBox="0 0 24 24" width="24" height="24">
                <polyline points="20 6 9 17 4 12"></polyline>
            </svg>
        `;
    } else if (normType === 'error') {
        titleText = 'Error';
        iconSvg = `
            <svg viewBox="0 0 24 24" width="24" height="24">
                <circle cx="12" cy="12" r="10"></circle>
                <line x1="12" y1="8" x2="12" y2="12"></line>
                <line x1="12" y1="16" x2="12.01" y2="16"></line>
            </svg>
        `;
    } else if (normType === 'delete') {
        titleText = 'Deleted';
        iconSvg = `
            <svg viewBox="0 0 24 24" width="24" height="24">
                <polyline points="3 6 5 6 21 6"></polyline>
                <path d="M19 6v14a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2V6m3 0V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2"></path>
                <line x1="10" y1="11" x2="10" y2="17"></line>
                <line x1="14" y1="11" x2="14" y2="17"></line>
            </svg>
        `;
    }

    // Create toast element
    const toast = document.createElement('div');
    toast.className = `custom-toast toast-${normType}`;
    
    toast.innerHTML = `
        <div class="toast-icon-wrap">
            ${iconSvg}
        </div>
        <div class="toast-content">
            <span class="toast-title">${titleText}</span>
            <span class="toast-message">${message}</span>
        </div>
        <button class="toast-close-btn" aria-label="Close">
            <svg viewBox="0 0 24 24" width="16" height="16" stroke="currentColor" stroke-width="2.5" fill="none">
                <line x1="18" y1="6" x2="6" y2="18"></line>
                <line x1="6" y1="6" x2="18" y2="18"></line>
            </svg>
        </button>
        <div class="toast-progress"></div>
    `;

    // Append to container
    container.appendChild(toast);

    // Trigger show transition
    requestAnimationFrame(() => {
        toast.classList.add('show');
    });

    // Setup auto-dismiss after exactly 2 seconds
    let dismissTimeout = setTimeout(() => {
        dismissToast(toast);
    }, 2000);

    // Setup manual close button
    const closeBtn = toast.querySelector('.toast-close-btn');
    closeBtn.addEventListener('click', () => {
        clearTimeout(dismissTimeout);
        dismissToast(toast);
    });
}

function dismissToast(toast) {
    toast.classList.add('hide');
    toast.classList.remove('show');
    
    // Wait for slide-out CSS transition before removing from DOM
    toast.addEventListener('transitionend', (e) => {
        if (e.propertyName === 'transform' || e.propertyName === 'opacity') {
            toast.remove();
        }
    });
}

