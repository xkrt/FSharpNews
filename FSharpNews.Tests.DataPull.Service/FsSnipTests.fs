module FSharpNews.Tests.DataPull.Service.FsSnipTests

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
let ``One snippet returned by api => one activity in storage``() =
    use fs = FsSnipApi.runServer (GET >>= url FsSnipApi.path
                                      >>= OK TestData.FsSnip.json)

    use tw = TwitterApi.runEmpty()
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    let activities = Storage.getAllActivities()
    let snip =
        activities
        |> List.map fst
        |> List.exactlyOne
        |> function FsSnippet snip -> snip | x -> failwithf "Expected FsSnippet, but was %O" (x.GetType())

    let expected = TestData.FsSnip.snippet

    snip.Id |> assertEqual expected.Id
    snip.Title |> assertEqual expected.Title
    snip.Author |> assertEqual expected.Author
    snip.Url |> assertEqual expected.Url
    snip.PublishedDate |> assertEqualDateWithin expected.PublishedDate (TimeSpan.FromSeconds(15.))

    activities
    |> List.map snd
    |> List.iter (assertEqualDateWithin DateTime.UtcNow (TimeSpan.FromSeconds(15.)))
