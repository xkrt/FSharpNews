namespace FSharpNews.Web.Frontend.Models

open System
open System.Web.Http
open FSharpNews.Data
open FSharpNews.Utils

type ActivityViewModel(iconUrl: string, iconTitle: string, text: string, url: string, creationDateUnix: int, addedDateUnixOffset: int64) =
    member val IconUrl = iconUrl with get
    member val IconTitle = iconTitle with get
    member val Text = text with get
    member val Url = url with get
    member val CreationDateUnix = creationDateUnix with get
    member val AddedDateUnixOffset = addedDateUnixOffset with get

    static member Create(activity: Activity, added: DateTime) =
        let decode = Net.WebUtility.HtmlDecode
        match activity with
        | StackExchangeQuestion q ->
            let iconUrl, iconTitle = match q.Site with
                                     | Stackoverflow -> "http://cdn.sstatic.net/stackoverflow/img/favicon.ico", "StackOverflow"
                                     | Programmers -> "http://cdn.sstatic.net/programmers/img/favicon.ico", "Programmers"
                                     | CodeReview -> "http://cdn.sstatic.net/codereview/img/favicon.ico", "Code Review"
                                     | CodeGolf -> "http://cdn.sstatic.net/codegolf/img/favicon.ico", "Programming Puzzles & Code Golf"
            ActivityViewModel(
                iconUrl = iconUrl,
                iconTitle = iconTitle,
                text = sprintf "%s: %s" q.UserDisplayName (decode q.Title),
                url = q.Url,
                creationDateUnix = DateTime.toUnix q.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | Tweet t ->
            ActivityViewModel(
                iconUrl = "http://abs.twimg.com/favicons/favicon.ico",
                iconTitle = "Twitter",
                text = sprintf "%s: %s" t.UserScreenName (decode t.Text),
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
        | FsSnippet s ->
            ActivityViewModel(
                iconUrl = "http://fssnip.net/favicon.ico",
                iconTitle = "F# Snippets",
                text = sprintf "%s: %s published" s.Author s.Title,
                url = s.Url,
                creationDateUnix = DateTime.toUnix s.PublishedDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
