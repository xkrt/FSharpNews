function Page(config) {
    var title = $('title');
    var setTitleCount = function (hiddenNews) {
        if (hiddenNews.length === 0)
            title.text('F# News');
        else
            title.text('(' + hiddenNews.length + ') F# News');
    };

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

    var addHidden = function(activities) {
        var vms = activities.map(activityToViewModel);
        vms.reverse();
        vms.forEach(function(vm) { pageViewModel.HiddenNews.unshift(vm); });
        setTitleCount(pageViewModel.HiddenNews());
    };

    var requestNews = function () {
        if (pageViewModel.ShowedNews().length === 0)
            return;

        // todo: extract
        var lastAddedStamp = 0;
        $.each([].concat(pageViewModel.ShowedNews(), pageViewModel.HiddenNews()), function (_, activity) { if (activity.AddedAt > lastAddedStamp) lastAddedStamp = activity.AddedAt; });

        $.get('/api/news/since', { time: lastAddedStamp })
            .done(addHidden.bind(this))
            .done(function () { pageViewModel.UpdatedDate(moment()); })
            .always(delayRequestNews);
    };

    var delayRequestNews = function () {
        window.setTimeout(requestNews.bind(this), config.NewsRequestPeriod * 1000);
    };

    var pageViewModel = {
        UpdatedDate: ko.observable(moment()),
        ShowedNews: ko.observableArray(config.InitialNews.map(activityToViewModel)),
        HiddenNews: ko.observableArray([]),
        HasMoreOldNews: ko.observable(true)
    };
    pageViewModel.UpdatedAgo = ko.computed(function () { return pageViewModel.UpdatedDate().from(now()); });
    pageViewModel.UpdatedTitle = ko.computed(function () {
        var updated = 'updated at ' + pageViewModel.UpdatedDate().format('HH:mm:ss');
        return updated + ', updates every ' + config.NewsRequestPeriod + ' secs';
    });
    pageViewModel.showHiddenNews = function () {
        // todo use apply
        while (this.HiddenNews().length > 0) {
            this.ShowedNews.unshift(this.HiddenNews.pop());
        }
        this.setTitleCount(this.HiddenNews());
    };
    pageViewModel.loadMore = function () {
        var showedNews = pageViewModel.ShowedNews();
        var oldestActivity = showedNews[showedNews.length - 1];
        return $.get('/api/news/earlier', { time: oldestActivity.AddedAt })
                .done(function (activities) {
                    pageViewModel.HasMoreOldNews(activities.length === config.BatchSize);
                    activities
                        .map(activityToViewModel)
                        .forEach(function (vm) { pageViewModel.ShowedNews.push(vm); }); // todo apply?
                });
    };

    ko.applyBindings(pageViewModel);
    delayRequestNews();
};
