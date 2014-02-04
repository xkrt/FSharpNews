namespace FSharpNews.Web.Frontend.Models

type PageConfiguration(initialNews: ActivityViewModel list, newsRequestPeriod: int, batchSize: int) =
    member val InitialNews = initialNews with get
    member val NewsRequestPeriod = newsRequestPeriod with get
    member val BatchSize = batchSize with get
