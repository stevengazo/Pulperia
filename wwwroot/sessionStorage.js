window.appStorage = {
    // persistent = true  -> localStorage (sobrevive al cierre del navegador)
    // persistent = false -> sessionStorage (se borra al cerrar la pestaña)
    save: (key, value, persistent) =>
        (persistent ? localStorage : sessionStorage).setItem(key, value),
    load: (key) =>
        localStorage.getItem(key) ?? sessionStorage.getItem(key),
    remove: (key) => {
        localStorage.removeItem(key);
        sessionStorage.removeItem(key);
    }
};
