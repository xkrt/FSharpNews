﻿module FSharpNews.Tests.DataPull.Service.NuGetTests

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
let ``One package returned by api => one activity in storage``() =
    use nu = NuGetApi.runServer (GET >>= url NuGetApi.path
                               >>= (set_header "Content-Type" "application/atom+xml;type=feed;charset=utf-8"
                                    >> OK TestData.NuGet.xml))
    use tw = TwitterApi.runEmpty()
    use se = StackExchangeApi.runEmpty()

    use puller = ServiceApplication.start()
    sleep 10

    let activities = Storage.getAllActivities()
    activities
    |> List.map fst
    |> Collection.assertEquiv ([ TestData.NuGet.activity ])

    activities
    |> List.map snd
    |> List.iter (assertEqualDateWithin DateTime.UtcNow (TimeSpan.FromSeconds(15.)))