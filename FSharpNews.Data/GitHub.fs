module FSharpNews.Data.GitHub

open System
open System.Text.RegularExpressions
open Octokit
open Octokit.Internal
open FSharpx
open FSharpNews.Utils

type Configuration = { BaseUri: Uri
                       Login: string
                       Password: string }

let private log = Logger.create "GitHub"

let private createClient conf =
    let credStore = InMemoryCredentialStore(Credentials(conf.Login, conf.Password))
    GitHubClient(ProductHeaderValue("FSharpNews"), credStore, conf.BaseUri)

let fetchGists conf (sinceDate: DateTime) =
    let client = createClient conf

    do log.Info "Start fetching gists since %s..." (sinceDate.ToIsoString())
    let allGists =
        client.Gist.GetAllPublic(DateTimeOffset sinceDate)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> Seq.toList
    let fsharpGists =
        allGists
        |> List.filter (fun g -> g.Files |> Seq.map (fun kvp -> kvp.Value) |> Seq.exists (fun f -> f.Language = "F#"))
        |> List.filter (fun g -> match g.Description with
                                 | null -> true
                                 | Regex.Interpreted.Match "[^\u0000-\u0080]+" m ->
                                    do log.Info "Looks like gist is non-english { Id=%s; Description='%s' }" g.Id g.Description
                                    false
                                 | _ -> true)
    do log.Info "Fetched gists f#/all: %d/%d" fsharpGists.Length allGists.Length

    let gistsWithRaws =
        fsharpGists
        |> List.map (fun g -> Activity.Gist { Id = g.Id
                                              Description = g.Description.TrimToNull() |> Option.ofNull
                                              Owner = g.Owner.Login
                                              CreationDate = g.CreatedAt.UtcDateTime
                                              Url = g.HtmlUrl }
                              , Serializer.toJson g)
    let lastGistDate =
        allGists
        |> List.map (fun g -> g.CreatedAt.UtcDateTime)
        |> List.max
    gistsWithRaws, lastGistDate

let fetchNewRepos conf sinceExclusive =
    let client = createClient conf
    let search req =
        let rec loop req acc =
            let batch =
                client.Search.SearchRepo req
                |> Async.AwaitTask
                |> Async.RunSynchronously
                |> (fun res -> res.Items)
                |> Seq.map (fun r -> { Repository.Id = r.Id
                                       Name = r.Name
                                       Description = r.Description.TrimToNull() |> Option.ofNull
                                       Owner = r.Owner.Login
                                       Url = r.HtmlUrl
                                       CreationDate = r.CreatedAt.UtcDateTime } |> Activity.Repository
                                     , Serializer.toJson r)
                |> Seq.toList
            match batch, req.Page with
            | [], _ -> acc
            | rs, _ when rs.Length < 100 -> rs @ acc
            | rs, 10 -> rs @ acc
            | rs, _ -> req.Page <- req.Page + 1
                       loop req (rs @ acc)
        loop req []

    let request = SearchRepositoriesRequest("")
    request.Language <- Nullable Language.FSharp
    request.Fork <- ForkQualifier.ExcludeForks
    request.Created <- DateRange.GreaterThan sinceExclusive
    request.PerPage <- 100
    request.Page <- 1

    do log.Info "Start searching repos since %s..." (sinceExclusive.ToIsoString())
    let repos = search request
    do log.Info "Found F# repos: %d" repos.Length
    repos
