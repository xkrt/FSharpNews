function Page(config) {
    var createAutoMoment = function (periodSec) {
        var observableNow = ko.observable(moment());
        window.setInterval(function () { observableNow(moment()); }, periodSec * 1000);
        return observableNow;
    };
    var now = createAutoMoment(10);

    var timeAgoObservable = function (moment) {
        return ko.computed(function () { return moment.from(now()); });
    };

    var activityToViewModel = function (activity) {
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
        if (pageViewModel.ShowedNews().length === 0)
            return;

        // todo: extract
        var lastAddedStamp = 0;
        $.each([].concat(pageViewModel.ShowedNews(), pageViewModel.HiddenNews()), function (_, activity) { if (activity.AddedAt > lastAddedStamp) lastAddedStamp = activity.AddedAt; });

        $.get('/api/news', { addedFromDate: lastAddedStamp })
            .done(function (activities) {
                var vms = activities.map(activityToViewModel);
                vms.reverse();
                vms.forEach(function (vm) { pageViewModel.HiddenNews.unshift(vm); });
            })
            .done(function () { pageViewModel.UpdatedDate(moment()); })
            .always(delayRequestNews);
    };

    var delayRequestNews = function () {
        window.setTimeout(requestNews, config.NewsRequestPeriod * 1000);
    };

    var pageViewModel = {
        UpdatedDate: ko.observable(moment()),
        ShowedNews: ko.observableArray(config.InitialNews.map(activityToViewModel)),
        HiddenNews: ko.observableArray([])
    };
    pageViewModel.UpdatedAgo = ko.computed(function () { return pageViewModel.UpdatedDate().from(now()); });
    pageViewModel.UpdatedTitle = ko.computed(function () {
        var updated = 'updated at ' + pageViewModel.UpdatedDate().format('HH:mm:ss');
        return updated + ', updates every ' + config.NewsRequestPeriod + ' secs';
    });
    pageViewModel.showHiddenNews = function () {
        while (this.HiddenNews().length > 0) {
            this.ShowedNews.unshift(this.HiddenNews.pop());
        }
    };

    ko.applyBindings(pageViewModel);
    delayRequestNews();
};
