$(function () {
    var createAutoMoment = function (periodSec) {
        var now = ko.observable(moment());
        window.setInterval(function () { now(moment()); }, periodSec * 1000);
        return now;
    };

    var timeAgoObservable = function (datetimeMoment) {
        var now = createAutoMoment(10);
        var ago = ko.computed(function () { return datetimeMoment.from(now()); });
        return ago;
    };

    var truncateTooLong = function (str, maxLen) {
        return str.length - 3 > maxLen
            ? str.substr(0, maxLen - 3) + '...'
            : str;
    };

    var activityToViewModel = function(activity) {
        var createMoment = moment(activity.CreationDateUnixOffset);
        var createdAgo = timeAgoObservable(createMoment);
        var createdTitle = createMoment.format('YYYY-MM-DD HH:mm:ss') + 'Z';
        return {
            IconUrl: activity.IconUrl,
            IconTitle: activity.IconTitle,
            Text: truncateTooLong(activity.Text, 140),
            Url: activity.Url,
            CreationDateUnixOffset: activity.CreationDateUnixOffset,
            CreationDateAgo: createdAgo,
            CreationDateTitle: createdTitle
        };
    };

    var requestNews = function () {
        var lastActivityDate = pageViewModel.News()[0].CreationDateUnixOffset;
        $.get('/api/news', { fromDate: lastActivityDate })
            .done(function(activities) {
                var vms = activities.map(activityToViewModel);
                vms.reverse();
                vms.forEach(function (vm) { pageViewModel.News.unshift(vm); });
            })
            .always(delayRequestNews);
    };

    var delayRequestNews = function() {
        window.setTimeout(requestNews, 5000);
    };

    var pageViewModel = {
        News: ko.observableArray(window.initialNews.map(activityToViewModel))
    };
    ko.applyBindings(pageViewModel);
    delayRequestNews();
});
