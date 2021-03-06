﻿namespace FSharpNews.Tests.DataPull.Service

open System
open Suave.Http
open Suave.Types
open Suave.Web
open Suave.Socket
open FSharpNews.Tests.Core

type Source =
    | StackExchange
    | Twitter
    | NuGet
    | FsSnip
    | FPish
    | Gists
    | Repos
    | Groups

module WebServer =
    let run name port route =
        let binding = HttpBinding.Create(HTTP, System.Net.IPAddress.Any.ToString(), port)
        let errorhandler (ex : Exception) msg (request : HttpRequest) =
            async { dprintfn "WebServer error: %s\n%O\nWhile processing request: %A" msg ex request }
        let cts = new Threading.CancellationTokenSource()
        let config = { default_config with bindings = [binding]
                                           error_handler = errorhandler
                                           ct = cts.Token }
        let pipeline = choose [log_format >> dprintfn "---> %s: %s" name >> never
                               route
                               failwithf "Unexpected request to %s: %A" name]
        Async.Start (async { web_server config pipeline })
        { new IDisposable
          with member this.Dispose() = cts.Cancel() }

module StackExchangeApi =
    let private port = 4141
    let path = "/2.1/questions"
    let baseUrl = sprintf "http://%s:%d" Environment.machine port
    let runServer = WebServer.run "StackExchange" port
    let runEmpty () = runServer (OK TestData.StackExchange.emptyJson)

module TwitterApi =
    let private port = 4142

    let streamPath = "/1.1/statuses/filter.json"
    let searchPath = "/1.1/search/tweets.json"
    let baseUrl = sprintf "http://%s:%d/1.1" Environment.machine port
    let baseSearchUrl = sprintf "http://%s:%d/1.1/search" Environment.machine port

    let runServer = WebServer.run "Twitter" port

    let private writeln str req = async_writeln req.connection str
    let writeMessage req (msg: string) =
        async { let size = (msg.Length + 2).ToString("X")
                do! writeln size req
                do! writeln msg req
                do! writeln "" req }
    let writeBlankLine req = writeMessage req ""
    let writeHeaderBodyDelimeter = writeln ""
    let sleep sec = Async.Sleep(sec * 1000)

    let writeEmptyInfinite (req: HttpRequest) =
        async { while true do
                do! writeBlankLine req
                do! sleep 5 }

    let handle bodyWriter (req: HttpRequest) =
        fun req ->
            let asyncHandler = async {
                req.response.Headers.Add("Content-Type", "application/json")
                req.response.Headers.Add("Transfer-Encoding", "chunked")
                do! response_f 200 "OK" bodyWriter req }
            succeed asyncHandler

    let handleWithEmptyInfinite = handle (fun req ->
        async { do! writeHeaderBodyDelimeter req
                do! writeEmptyInfinite req })

    let runEmptyStream() = runServer (POST >>= url streamPath >>== handleWithEmptyInfinite)

module NuGetApi =
    let private port = 4143

    let path = "/api/v2/Packages()"
    let baseUrl = sprintf "http://%s:%d/api/v2" Environment.machine port

    let runServer = WebServer.run "NuGet" port
    let runEmpty() = runServer (OK TestData.NuGet.emptyXml)

module FsSnipApi =
    let private port = 4144

    let path = "/1/snippet"
    let baseUrl = sprintf "http://%s:%d" Environment.machine port

    let runServer = WebServer.run "FsSnip" port
    let runEmpty() = runServer (OK TestData.FsSnip.emptyJson)

module FPishFeed =
    let private port = 4145

    let path = "/atom/topics/tag/1/f~23"
    let baseUrl = sprintf "http://%s:%d" Environment.machine port

    let runServer = WebServer.run "FPish" port
    let runEmpty() = runServer (OK TestData.FPish.emptyXml)

module GitHubApi =
    let private port = 4146

    let gistsPath = "/api/v3/gists/public"
    let repoPath = "/api/v3/search/repositories"
    let baseUrl = sprintf "http://%s:%d" Environment.machine port

    let runServer = WebServer.run "GitHub" port
    let runEmpty() = runServer (OK TestData.Gist.emptyJson)

module GroupsFeed =
    let private port = 4147

    let path = "/forum/feed/fsharp-opensource/topics/atom.xml"
    let baseUrl = sprintf "http://%s:%d" Environment.machine port

    let runServer = WebServer.run "Groups" port
