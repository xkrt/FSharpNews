module FSharpNews.Tests.DataPuller.NuGetTests

open System
open System.Threading
open NUnit.Framework
open Suave.Types
open Suave.Socket
open Suave.Http
open Suave.Web
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

[<SetUp>]
let Setup() = do Storage.deleteAll()

[<Test>]
let ``One package returned by api => one activity in storage``() =
    do NuGetApi.runServer (GET >>= url NuGetApi.path
                               >>= (set_header "Content-Type" "application/atom+xml;type=feed;charset=utf-8"
                                    >> OK TestData.NuGet.xml))
    do TwitterApi.runEmpty()
    do StackExchangeApi.runEmpty()

    use puller = DataPullerApp.start()
    sleep 10

    let activities = Storage.getAllActivities()
    activities
    |> List.map fst
    |> Collection.assertEquiv ([ TestData.NuGet.activity ])

    activities
    |> List.map snd
    |> List.iter (fun addedTime -> Assert.That(addedTime, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(15.)), "added time"))
