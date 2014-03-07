module FSharpNews.Tests.DataPull.Service.RepositoriesTests

open System
open NUnit.Framework
open Suave.Types
open Suave.Http
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

[<SetUp>]
let Setup() = do Storage.deleteAll()

[<Test>]
let ``One repo returned by search api => one activity in storage``() =
    use gh = GitHubApi.runServer (GET >>= url GitHubApi.repoPath
                                      >>= (set_mime_type "application/json"
                                      >> OK TestData.Repositories.json))
    use tw = TwitterApi.runEmpty()
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()
    use fs = FsSnipApi.runEmpty()
    use fp = FPishApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    let activities = Storage.getAllActivities()
    let repo =
        activities
        |> List.map fst
        |> List.exactlyOne
        |> function Repository r -> r | x -> failwithf "Expected Repository, but was %O" (x.GetType())

    repo |> assertEqual TestData.Repositories.repo
    activities
    |> List.map snd
    |> List.iter (assertEqualDateWithin DateTime.UtcNow (TimeSpan.FromSeconds 15.))

