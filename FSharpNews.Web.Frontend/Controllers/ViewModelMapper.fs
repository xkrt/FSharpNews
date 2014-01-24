namespace FSharpNews.Web.Frontend.Controllers
open System
open System.Web.Http
open FSharpNews.Data
open FSharpNews.Utils

type ActivityViewModel(iconUrl: string, iconTitle: string, text: string, url: string, creationDateUnix: int, addedDateUnixOffset: int64) =
    member val IconUrl = iconUrl with get, set
    member val IconTitle = iconTitle with get, set
    member val Text = text with get, set
    member val Url = url with get, set
    member val CreationDateUnix = creationDateUnix with get, set
    member val AddedDateUnixOffset = addedDateUnixOffset with get, set

    static member Create(activity: Activity, added: DateTime) =
        match activity with
        | StackExchangeQuestion q ->
            let iconUrl, iconTitle = match q.Site with
                                     | Stackoverflow -> "http://cdn.sstatic.net/stackoverflow/img/favicon.ico", "StackOverflow"
                                     | Programmers -> "http://cdn.sstatic.net/programmers/img/favicon.ico", "Programmers"
            ActivityViewModel(
                iconUrl = iconUrl,
                iconTitle = iconTitle,
                text = sprintf "%s: %s" q.UserDisplayName q.Title,
                url = q.Url,
                creationDateUnix = DateTime.toUnix q.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | Tweet t ->
            ActivityViewModel(
                iconUrl = "http://abs.twimg.com/favicons/favicon.ico",
                iconTitle = "Twitter",
                text = sprintf "%s: %s" t.UserScreenName t.Text,
                url = sprintf "https://twitter.com/%s/status/%d" t.UserScreenName t.Id,
                creationDateUnix = DateTime.toUnix t.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | NugetPackage p ->
            ActivityViewModel(
                iconUrl = "https://www.nuget.org/favicon.ico",
                iconTitle = "NuGet",
                text = sprintf "%s %s published" p.Id p.Version,
                url = p.Url,
                creationDateUnix = DateTime.toUnix p.PublishedDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
