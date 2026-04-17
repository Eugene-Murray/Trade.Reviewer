// Sidebar toggle state persistence
(function () {
    const STORAGE_KEY = 'sidebar-collapsed';

    function initSidebar() {
        const checkbox = document.getElementById('sidebar-toggle');

        if (checkbox) {
            // Restore state from localStorage
            const isCollapsed = localStorage.getItem(STORAGE_KEY) === 'true';
            if (checkbox.checked !== isCollapsed) {
                checkbox.checked = isCollapsed;
            }

            // Only add listener if not already added
            if (!checkbox.hasAttribute('data-sidebar-initialized')) {
                checkbox.setAttribute('data-sidebar-initialized', 'true');
                checkbox.addEventListener('change', function () {
                    localStorage.setItem(STORAGE_KEY, this.checked);
                });
            }
        }
    }

    // Initialize on DOMContentLoaded
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSidebar);
    } else {
        initSidebar();
    }

    // Re-initialize after Blazor enhanced navigation
    document.addEventListener('DOMContentLoaded', function () {
        if (typeof Blazor !== 'undefined' && Blazor.addEventListener) {
            Blazor.addEventListener('enhancedload', initSidebar);
        }
    });

    // Also watch for page visibility changes (tab switching)
    document.addEventListener('visibilitychange', function () {
        if (!document.hidden) {
            initSidebar();
        }
    });

    // Expose globally for manual initialization if needed
    window.initSidebar = initSidebar;
})();
