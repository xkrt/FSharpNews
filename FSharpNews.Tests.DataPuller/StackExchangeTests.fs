module FSharpNews.Tests.DataPuller.StackExchangeTests

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

// todo: test wrong json

[<Test>]
let ``One question on SO and one on Programmers => two activities in storage``() =
    let handler (r: HttpRequest) =
        match (r.query)?site with
        | Some "stackoverflow" -> OK TestData.StackExchange.soJson
        | Some "programmers" -> OK TestData.StackExchange.progJson
        | x -> failwithf "Wrong 'site' query item=%A" x

    do StackExchangeApi.runServer (GET >>= url StackExchangeApi.path >>== handler)
    do TwitterApi.runServer (POST >>= url TwitterApi.path >>== TwitterApi.handleWithEmptyInfinite)

    use puller = DataPullerApp.start()
    sleep 10

    let activities = Storage.getAllActivities()
    activities
    |> List.map fst
    |> Collection.assertEquiv ([ TestData.StackExchange.soActivity; TestData.StackExchange.progActivity ])

    activities
    |> List.map snd
    |> List.iter (fun addedTime -> Assert.That(addedTime, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(30.)), "added time"))
