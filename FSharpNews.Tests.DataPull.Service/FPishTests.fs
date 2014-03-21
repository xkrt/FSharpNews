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
    use fs = FPishFeed.runServer (GET >>= url FPishFeed.path >>= OK TestData.FPish.xml)
    do ServiceApplication.startAndSleep FPish

    Storage.getAllActivities()
    |> List.map fst
    |> List.exactlyOne
    |> assertEqual TestData.FPish.activity
