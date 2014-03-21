namespace FSharpNews.Web.Frontend.Models

open System
open System.Web.Http
open FSharpNews.Data
open FSharpNews.Utils

type Link = { Text: string
              Url: string } with override x.ToString() = sprintf "%A" x

type private TweetLink = { Title: string
                           Url: string
                           Start: int
                           End: int }

type ActivityViewModel(lowResIconUrl: string, hiResIconUrl: string, iconTitle: string, links: Link list, photoUrl: string option, photoUrlThumb: string option, creationDateUnixOffset: int64, addedDateUnixOffset: int64) =
    member val IconLowResUrl = lowResIconUrl with get
    member val IconHiResUrl = hiResIconUrl with get
    member val IconTitle = iconTitle with get
    member val Links = links with get
    member val PhotoUrl = (Option.toNull photoUrl) with get
    member val PhotoUrlThumb = (Option.toNull photoUrlThumb) with get
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
                links = [{ Text = sprintf "%s: %s" (decode q.UserDisplayName) (decode q.Title)
                           Url = q.Url }],
                photoUrl = None,
                photoUrlThumb = None,
                creationDateUnixOffset = DateTime.toUnixOffset q.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | Tweet t ->
            let tweetUrl = sprintf "https://twitter.com/%s/status/%d" t.UserScreenName t.Id
            let urls = t.Urls |> List.map (fun u -> { Title = u.DisplayUrl
                                                      Url = u.ExpandedUrl
                                                      Start = u.StartIndex
                                                      End = u.EndIndex })
            let photos = match t.Photo with
                         | Some p -> [{ Title = p.DisplayUrl
                                        Url = p.Url
                                        Start = p.StartIndex
                                        End = p.EndIndex }]
                         | None -> []
            let allUrls = (urls @ photos) |> List.sortWith (fun u1 u2 -> u1.Start.CompareTo(u2.Start))
            let links =
                match allUrls with
                | [] -> [{ Text = sprintf "%s: %s" t.UserScreenName (decode t.Text)
                           Url = tweetUrl }]
                | urls -> seq { let firstUrl = urls.Head
                                yield { Text = sprintf "%s: %s" t.UserScreenName (t.Text.Substring(0, firstUrl.Start)) |> Strings.trim |> decode
                                        Url = tweetUrl }
                                yield { Text = firstUrl.Title.Trim()
                                        Url = firstUrl.Url }
                                let lastEndIndex = ref firstUrl.End
                                for url in urls.Tail do
                                    yield { Text = t.Text.Substring(!lastEndIndex, url.Start - !lastEndIndex - 1) |> Strings.trim |> decode
                                            Url = tweetUrl }
                                    yield { Text = url.Title.Trim()
                                            Url = url.Url }
                                    lastEndIndex := url.End
                                yield { Text = t.Text.Substring(!lastEndIndex, t.Text.Length - !lastEndIndex).Trim()
                                        Url = tweetUrl }
                          } |> Seq.filter (fun l -> l.Text <> "") |> Seq.toList
            let photoUrl = t.Photo |> Option.map (fun p -> sprintf "%s" p.MediaUrl)
            let photoUrlThumb = photoUrl |> Option.map (sprintf "%s:thumb")
            ActivityViewModel(
                lowResIconUrl = imgUrl "twitter16x16.png",
                hiResIconUrl = imgUrl "twitter32x32.png",
                iconTitle = "Twitter",
                links = links,
                photoUrl = photoUrl,
                photoUrlThumb = photoUrlThumb,
                creationDateUnixOffset = DateTime.toUnixOffset t.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | NugetPackage p ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "nuget16x16.png",
                hiResIconUrl = imgUrl "nuget32x32.png",
                iconTitle = "NuGet",
                links = [{ Text = sprintf "%s %s published" p.Id p.Version; Url = p.Url }],
                photoUrl = None,
                photoUrlThumb = None,
                creationDateUnixOffset = DateTime.toUnixOffset p.PublishedDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | FsSnippet s ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "fssnip16x16.png",
                hiResIconUrl = imgUrl "fssnip32x32.png",
                iconTitle = "F# Snippets",
                links = [{ Text = sprintf "%s: %s" s.Author s.Title; Url = s.Url }],
                photoUrl = None,
                photoUrlThumb = None,
                creationDateUnixOffset = DateTime.toUnixOffset s.PublishedDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | FPishQuestion q ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "fpish16x16.png",
                hiResIconUrl = imgUrl "fpish32x32.png",
                iconTitle = "FPish",
                links = [{ Text = sprintf "%s: %s" q.Author q.Title; Url = q.Url }],
                photoUrl = None,
                photoUrlThumb = None,
                creationDateUnixOffset = DateTime.toUnixOffset q.PublishedDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | Gist g ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "github16x16.png",
                hiResIconUrl = imgUrl "github32x32.png",
                iconTitle = "GitHub",
                links = [{ Text = sprintf "Gist by %s: %s" g.Owner (g.Description |> function Some s -> s | None -> "<no description>")
                           Url = g.Url }],
                photoUrl = None,
                photoUrlThumb = None,
                creationDateUnixOffset = DateTime.toUnixOffset g.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | Repository r ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "github16x16.png",
                hiResIconUrl = imgUrl "github32x32.png",
                iconTitle = "GitHub",
                links = [{ Text = sprintf "New repo %s/%s%s" r.Owner r.Name (r.Description |> function Some s -> sprintf ": %s" s | None -> "")
                           Url = r.Url }],
                photoUrl = None,
                photoUrlThumb = None,
                creationDateUnixOffset = DateTime.toUnixOffset r.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
        | GroupTopic t ->
            ActivityViewModel(
                lowResIconUrl = imgUrl "groups16x16.png",
                hiResIconUrl = imgUrl "groups32x32.png",
                iconTitle = "Google Groups",
                links = [{ Text = sprintf " %s:%s" t.Starter t.Title; Url = t.Url }],
                photoUrl = None,
                photoUrlThumb = None,
                creationDateUnixOffset = DateTime.toUnixOffset t.CreationDate,
                addedDateUnixOffset = DateTime.toUnixOffset added)
