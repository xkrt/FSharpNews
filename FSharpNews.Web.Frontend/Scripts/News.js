function PageViewModel(config) {
    var createAutoMoment = function (periodSec) {
        var observableNow = ko.observable(moment());
        window.setInterval(function () { observableNow(moment()); }, periodSec * 1000);
        return observableNow;
    };
    var now = createAutoMoment(30);

    var title = $('title');
    var setTitleCount = function (hiddenNews) {
        if (hiddenNews.length === 0)
            title.text('F# News');
        else
            title.text('(' + hiddenNews.length + ') F# News');
    };

    var timeAgoObservable = function (moment) {
        return ko.computed(function () { return moment.from(now()); });
    };

    var buildActivityViewModel = function (activity) {
        var createMoment = moment(activity.CreationDateUnixOffset).utc();
        var createdAgo = timeAgoObservable(createMoment);
        var createdTitle = createMoment.format('YYYY-MM-DD HH:mm:ss') + 'Z';
        var vm = {
            IconLowResUrl: activity.IconLowResUrl,
            IconHiResUrl: activity.IconHiResUrl,
            IconTitle: activity.IconTitle,
            Text: activity.Text,
            Url: activity.Url,
            CreationDateAgo: createdAgo,
            CreationDateTitle: createdTitle,
            CreationDate: activity.CreationDateUnixOffset,
            AddedAt: activity.AddedDateUnixOffset
        };
        vm.IconUrl = ko.computed(function() {
            return window.devicePixelRatio > 1
                ? this.IconHiResUrl
                : this.IconLowResUrl;
        }, vm);
        return vm;
    };

    var self = this;
    this.UpdatedDate = ko.observable(moment());
    this.UpdatedAgo = ko.computed(function() { return self.UpdatedDate().from(now()); }),
        this.UpdatedTitle = ko.computed(function() {
            var updated = 'updated at ' + self.UpdatedDate().format('HH:mm:ss');
            return updated + ', updates every ' + config.NewsRequestPeriod + ' secs';
        });
    this.ShowedNews = ko.observableArray(config.InitialNews.map(buildActivityViewModel));
    this.HiddenNews = ko.observableArray([]);
    this.HasMoreOldNews = ko.observable(true);
    this.showHiddenNews = function () {
        this.ShowedNews.unshift.apply(this.ShowedNews, this.HiddenNews.removeAll());
        setTitleCount(this.HiddenNews());
    };
    this.loadMore = function() {
        var showedNews = self.ShowedNews();
        var oldestShowedActivity = showedNews[showedNews.length - 1];
        return $.get('/api/news/earlier', { time: oldestShowedActivity.CreationDate })
            .done(function(activities) {
                self.HasMoreOldNews(activities.length === config.BatchSize);
                var avms = activities.map(buildActivityViewModel);
                self.ShowedNews.push.apply(self.ShowedNews, avms);
            });
    };
    this._getOldestActivityAddedStamp = function() {
        var allActivities = [].concat(this.ShowedNews(), this.HiddenNews());
        var lastAddedStamp = 0;
        $.each(allActivities, function (_, activity) {
            if (activity.AddedAt > lastAddedStamp)
                lastAddedStamp = activity.AddedAt;
        });
        return lastAddedStamp;
    };
    this.requestNews = function() {
        $.get('/api/news/since', { time: this._getOldestActivityAddedStamp() })
            .done(function(activities) {
                var vms = activities.map(buildActivityViewModel);
                self.HiddenNews.unshift.apply(self.HiddenNews, vms);
                setTitleCount(self.HiddenNews());
            }.bind(this))
            .done(function() { self.UpdatedDate(moment()); })
            .always(this.delayRequestNews);
    };
    this.delayRequestNews = function() {
        window.setTimeout(self.requestNews.bind(self), config.NewsRequestPeriod * 1000);
    };
};
