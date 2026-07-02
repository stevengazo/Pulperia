// Manejo del tema diurno/nocturno.
// La preferencia se guarda en localStorage y se aplica como atributo
// data-theme en <html>, que activa la paleta correspondiente en site.css.
window.pulperiaTheme = {
    key: 'pulperia-theme',

    get: function () {
        try {
            return localStorage.getItem(this.key) || 'dark';
        } catch (e) {
            return 'dark';
        }
    },

    set: function (theme) {
        try {
            localStorage.setItem(this.key, theme);
        } catch (e) { /* almacenamiento no disponible */ }
        document.documentElement.setAttribute('data-theme', theme);
    }
};
