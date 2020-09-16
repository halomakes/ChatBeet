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

function fallbackCopyTextToClipboard(text) {
    var textArea = document.createElement("textarea");
    textArea.value = text;

    // Avoid scrolling to bottom
    textArea.style.top = "0";
    textArea.style.left = "0";
    textArea.style.position = "fixed";

    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    try {
        var successful = document.execCommand('copy');
        var msg = successful ? 'successful' : 'unsuccessful';
        console.log('Fallback: Copying text command was ' + msg);
    } catch (err) {
        console.error('Fallback: Oops, unable to copy', err);
    }

    document.body.removeChild(textArea);
}

function copyTextToClipboard(text) {
    if (!navigator.clipboard) {
        fallbackCopyTextToClipboard(text);
        return;
    }
    navigator.clipboard.writeText(text).then(function () {
        console.log('Async: Copying to clipboard was successful!');
    }, function (err) {
        console.error('Async: Could not copy text: ', err);
    });
}