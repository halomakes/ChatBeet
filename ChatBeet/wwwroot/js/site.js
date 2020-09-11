var updateLoginLinks = function () {
    if (window.location.pathname.toLowerCase().indexOf('logout') === -1 && window.location.pathname.toLowerCase().indexOf('login') === -1) {
        var links = document.getElementsByClassName('login-link');
        for (var i = 0; i < links.length; i++) {
            var link = links[i];
            link.href = link.href + "?ReturnUrl=" + window.location.pathname;
        }
    }
};

if (document.readyState === "complete" || (document.readyState !== "loading" && !document.documentElement.doScroll)) {
    updateLoginLinks();
} else {
    document.addEventListener("DOMContentLoaded", updateLoginLinks);
}