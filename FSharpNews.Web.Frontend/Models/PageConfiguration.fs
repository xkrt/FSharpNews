namespace FSharpNews.Web.Frontend.Models

type PageConfiguration(initialNews: ActivityViewModel list, newsRequestPeriod: int) =
    member val InitialNews = initialNews with get
    member val NewsRequestPeriod = newsRequestPeriod with get