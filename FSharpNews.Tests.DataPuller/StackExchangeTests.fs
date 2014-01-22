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

let runServer name route =
    let config = { default_config with bindings = [HttpBinding.Create(HTTP, System.Net.IPAddress.Any.ToString(), FakeApi.serverPort)] }
    let routes = choose [log_format >> dprintf "---> %s: %s" name >> never
                         route]
    Async.Start (async { web_server config routes })

let emptyTwitterStream (req: HttpRequest) =
        let writeln = async_writeln req.connection
        let writeChunk (content: string) = async {
            let size = content.Length.ToString("X")
            do! writeln size
            do! writeln content
        }
        let writeBlankLine() = writeChunk "\r\n"
        let sleep sec = Async.Sleep(sec * 1000)
        let writeEmptyInfinite req = async {
            do! writeln ""
            while true do
                do! writeBlankLine()
                do! sleep 5
        }
        fun req -> (async {
            req.response.Headers.Add("Content-Type", "application/json")
            req.response.Headers.Add("Transfer-Encoding", "chunked")
            do! response_f 200 "OK" writeEmptyInfinite req
        } |> succeed)

[<SetUp>]
let Setup() =
    do Storage.deleteAll()

[<Test>]
let ``One question on SO and one on Programers => two activities in storage``() =
    let handler (r: HttpRequest) =
        match (r.query)?site with
        | Some "stackoverflow" -> OK TestData.SOQuestion.json
        | Some "programmers" -> OK TestData.ProgQuestion.json
        | x -> failwithf "Wrong 'site' query item=%A" x

    do runServer "StackExchange" (GET >>= url FakeApi.sePath >>== handler)
    do runServer "Twitter" (POST >>= url FakeApi.twitterPath >>== emptyTwitterStream)
    use puller = DataPullerApp.start()
    sleep 10

    let activities = Storage.getAllActivities()
    activities
    |> List.map fst
    |> Collection.assertEquiv ([ TestData.SOQuestion.activity; TestData.ProgQuestion.activity ])

    activities
    |> List.map snd
    |> List.iter (fun addedTime -> Assert.That(addedTime, Is.EqualTo(DateTime.UtcNow).Within(TimeSpan.FromSeconds(30.)), "added time"))
