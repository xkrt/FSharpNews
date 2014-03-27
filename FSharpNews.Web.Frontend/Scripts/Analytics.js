(function (i, s, o, g, r, a, m) {
    i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
        (i[r].q = i[r].q || []).push(arguments);
    }, i[r].l = 1 * new Date(); a = s.createElement(o),
    m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m);
})(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');
ga('create', 'UA-48090222-1', 'fsharpnews.org');
ga('send', 'pageview');

(function gaHeartbeat() {
    ga('send', 'event', 'Heartbeat', 'Heartbeat');
    setTimeout(gaHeartbeat, 5 * 60 * 1000);
})();

function Analytics() {
    this.sendEvent = function (category, action) {
        ga('send', 'event', category, action);
    };
};
