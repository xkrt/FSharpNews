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

let fetchGists conf sinceDate =
    let credStore = InMemoryCredentialStore(Credentials(conf.Login, conf.Password))
    let client = GitHubClient(ProductHeaderValue("FSharpNews"), credStore, conf.BaseUri)
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
