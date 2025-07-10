window.themeHelper = {
    setTheme: function (isDark) {
        localStorage.setItem('theme', isDark ? 'dark' : 'light');
    },
    getTheme: function () {
        return localStorage.getItem('theme') === 'dark';
    },
    applyTheme: function (theme) {
        document.body.classList.remove('light', 'dark');
        document.body.classList.add(theme);
    }
};