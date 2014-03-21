module FSharpNews.Tests.DataPull.Service.GroupsTests

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
let ``One topic in atom feed => one activity in storage``() =
    use gr = GroupsFeed.runServer (GET >>= url GroupsFeed.path >>= OK TestData.Groups.xml)
    do ServiceApplication.startAndSleep Groups

    Storage.getAllActivities()
    |> List.map fst
    |> List.exactlyOne
    |> assertEqual TestData.Groups.activity
