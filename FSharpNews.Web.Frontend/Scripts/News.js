$(function () {
    var requestInterval = 10 * 1000; // ms

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

    var activityToViewModel = function(activity) {
        var createMoment = moment.unix(activity.CreationDateUnix).utc();
        var createdAgo = timeAgoObservable(createMoment);
        var createdTitle = createMoment.format('YYYY-MM-DD HH:mm:ss') + 'Z';
        return {
            IconUrl: activity.IconUrl,
            IconTitle: activity.IconTitle,
            Text: activity.Text,
            Url: activity.Url,
            CreationDateAgo: createdAgo,
            CreationDateTitle: createdTitle,
            AddedAt: activity.AddedDateUnixOffset
        };
    };

    var requestNews = function () {
        if (pageViewModel.News().length === 0)
            return;

        var lastActivityAdded = pageViewModel.News()[0].AddedAt;
        $.get('/api/news', { addedFromDate: lastActivityAdded })
            .done(function(activities) {
                var vms = activities.map(activityToViewModel);
                vms.reverse();
                vms.forEach(function (vm) { pageViewModel.News.unshift(vm); });
            })
            .always(delayRequestNews);
    };

    var delayRequestNews = function() {
        window.setTimeout(requestNews, requestInterval);
    };

    var pageViewModel = {
        News: ko.observableArray(window.initialNews.map(activityToViewModel))
    };
    ko.applyBindings(pageViewModel);
    delayRequestNews();
});
