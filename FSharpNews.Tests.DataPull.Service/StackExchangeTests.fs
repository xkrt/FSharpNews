module FSharpNews.Tests.DataPull.Service.StackExchangeTests

open System
open NUnit.Framework
open Suave.Types
open Suave.Http
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

[<SetUp>]
let Setup() = do Storage.deleteAll()

// todo: test wrong json

[<Test>]
let ``One question on SO and one on Programmers => two activities in storage``() =
    let handler (r: HttpRequest) =
        match (r.query)?site with
        | Some "stackoverflow" -> OK TestData.StackExchange.soJson
        | Some "programmers" -> OK TestData.StackExchange.progJson
        | Some "codereview" -> OK TestData.StackExchange.reviewJson
        | Some "codegolf" -> OK TestData.StackExchange.golfJson
        | x -> failwithf "Wrong 'site' query item=%A" x

    use se = StackExchangeApi.runServer (GET >>= url StackExchangeApi.path >>== handler)
    use tw = TwitterApi.runEmpty()
    use nu = NuGetApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    let activities = Storage.getAllActivities()
    activities
    |> List.map fst
    |> Collection.assertEquiv [ TestData.StackExchange.soActivity
                                TestData.StackExchange.progActivity
                                TestData.StackExchange.reviewActivity
                                TestData.StackExchange.golfActivity ]

    activities
    |> List.map snd
    |> List.iter (assertEqualDateWithin DateTime.UtcNow (TimeSpan.FromSeconds(15.)))
