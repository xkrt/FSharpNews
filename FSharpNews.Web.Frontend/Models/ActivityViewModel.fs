namespace FSharpNews.Web.Frontend.Models

open System
open System.Web.Http
open FSharpNews.Data
open FSharpNews.Utils

type ActivityViewModel(lowResIconUrl: string, hiResIconUrl: string, iconTitle: string, text: string, url: string, creationDateUnixOffset: int64, addedDateUnixOffset: int64) =
    member val IconLowResUrl = lowResIconUrl with get
    member val IconHiResUrl = hiResIconUrl with get
    member val IconTitle = iconTitle with get
    member val Text = text with get
    member val Url = url with get
    member val CreationDateUnixOffset = creationDateUnixOffset with get
    member val AddedDateUnixOffset = addedDateUnixOffset with get

    static member Create (activity: Activity, added: DateTime) =
        let decode = Net.WebUtility.HtmlDecode
        let imgUrl fname = sprintf "/Content/Images/%s" fname

        match activity with
        | StackExchangeQuestion q ->
            let iconUrl, retinaUrl, iconTitle =
                match q.Site with
                | Stackoverflow -> imgUrl "so16x16.png", imgUrl "so32x32.png", "StackOverflow"
                | Programmers -> imgUrl "programmers16x16.png", imgUrl "programmers32x32.png", "Programmers"
                | CodeReview -> imgUrl "codereview16x16.png", imgUrl "codereview32x32.png", "Code Review"
                | CodeGolf -> imgUrl "codegolf16x16.png", imgUrl "codegolf32x32.png", "Programming Puzzles & Code Golf"
            ActivityViewModel(
                lowResIconUrl = iconUrl,
                hiResIconUrl = retinaUrl,
                iconTitle = iconTitle,
                text = sprintf "%s: %s" (decode q.UserDisplayName) (decode q.Title),
                url = q.Url,
                creationDateUnixOffset = DateTime.toUnixOffset q.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | Tweet t ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "twitter16x16.png",
                hiResIconUrl = imgUrl "twitter32x32.png",
                iconTitle = "Twitter",
                text = sprintf "%s: %s" t.UserScreenName (decode t.Text),
                url = sprintf "https://twitter.com/%s/status/%d" t.UserScreenName t.Id,
                creationDateUnixOffset = DateTime.toUnixOffset t.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | NugetPackage p ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "nuget16x16.png",
                hiResIconUrl = imgUrl "nuget32x32.png",
                iconTitle = "NuGet",
                text = sprintf "%s %s published" p.Id p.Version,
                url = p.Url,
                creationDateUnixOffset = DateTime.toUnixOffset p.PublishedDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | FsSnippet s ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "fssnip16x16.png",
                hiResIconUrl = imgUrl "fssnip32x32.png",
                iconTitle = "F# Snippets",
                text = sprintf "%s: %s" s.Author s.Title,
                url = s.Url,
                creationDateUnixOffset = DateTime.toUnixOffset s.PublishedDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | FPishQuestion q ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "fpish16x16.png",
                hiResIconUrl = imgUrl "fpish32x32.png",
                iconTitle = "FPish",
                text = sprintf "%s: %s" q.Author q.Title,
                url = q.Url,
                creationDateUnixOffset = DateTime.toUnixOffset q.PublishedDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | Gist g ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "github16x16.png",
                hiResIconUrl = imgUrl "github32x32.png",
                iconTitle = "GitHub",
                text = sprintf "Gist by %s: %s" g.Owner (g.Description |> function Some s -> s | None -> "<no description>"),
                url = g.Url,
                creationDateUnixOffset = DateTime.toUnixOffset g.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | Repository r ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "github16x16.png",
                hiResIconUrl = imgUrl "github32x32.png",
                iconTitle = "GitHub",
                text = sprintf "New repo %s/%s%s" r.Owner r.Name (r.Description |> function Some s -> sprintf ": %s" s | None -> ""),
                url = r.Url,
                creationDateUnixOffset = DateTime.toUnixOffset r.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
