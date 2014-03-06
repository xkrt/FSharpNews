module FSharpNews.Tests.DataPull.Service.FPishTests

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
let ``One question returned by api => one activity in storage``() =
    use fs = FPishApi.runServer (GET >>= url FPishApi.path >>= OK TestData.FPish.xml)
    use tw = TwitterApi.runEmpty()
    use se = StackExchangeApi.runEmpty()
    use nu = NuGetApi.runEmpty()
    use fs = FsSnipApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    let activities = Storage.getAllActivities()
    let quest =
        activities
        |> List.map fst
        |> List.exactlyOne
        |> function FPishQuestion q -> q | x -> failwithf "Expected FPishQuestion, but was %O" (x.GetType())

    quest |> assertEqual TestData.FPish.question
    activities
    |> List.map snd
    |> List.iter (assertEqualDateWithin DateTime.UtcNow (TimeSpan.FromSeconds(15.)))
