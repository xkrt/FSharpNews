module FSharpNews.Tests.DataPull.Service.GistsTests

open System
open NUnit.Framework
open Suave.Types
open Suave.Http
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

[<SetUp>]
let Setup() = do Storage.deleteAll()

let runGistsWith json =
    GistsApi.runServer (GET >>= url GistsApi.path
                            >>= (set_mime_type "application/json"
                            >> OK json))

[<Test>]
let ``One F# gist returned by api => one activity in storage``() =
    use gs = runGistsWith TestData.Gist.fsharpJson
    use tw = TwitterApi.runEmpty()
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()
    use fs = FsSnipApi.runEmpty()
    use fp = FPishApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    let activities = Storage.getAllActivities()
    let gist =
        activities
        |> List.map fst
        |> List.exactlyOne
        |> function Gist g -> g | x -> failwithf "Expected Gist, but was %O" (x.GetType())

    gist |> assertEqual TestData.Gist.gist
    activities
    |> List.map snd
    |> List.iter (assertEqualDateWithin DateTime.UtcNow (TimeSpan.FromSeconds(15.)))

[<Test>]
let ``Non-F# gists filtered``() =
    use gs = runGistsWith TestData.Gist.nonFsharpJson
    use tw = TwitterApi.runEmpty()
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()
    use fs = FsSnipApi.runEmpty()
    use fp = FPishApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10;

    Storage.getAllActivities() |> assertEqual []

[<Test>]
let ``Gists with non-english description filtered``() =
    use gs = runGistsWith TestData.Gist.japanFsharpJson
    use tw = TwitterApi.runEmpty()
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()
    use fs = FsSnipApi.runEmpty()
    use fp = FPishApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    Storage.getAllActivities() |> assertEqual []

[<Test>]
let ``Gist with empty description saved with null description``() =
    use gs = runGistsWith TestData.Gist.emptyDescription
    use tw = TwitterApi.runEmpty()
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()
    use fs = FsSnipApi.runEmpty()
    use fp = FPishApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    let activities = Storage.getAllActivities()
    let gist =
        activities
        |> List.map fst
        |> List.exactlyOne
        |> function Gist g -> g | x -> failwithf "Expected Gist, but was %O" (x.GetType())

    gist.Description |> assertEqual None
