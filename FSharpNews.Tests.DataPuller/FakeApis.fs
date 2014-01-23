namespace FSharpNews.Tests.DataPuller

open Suave.Http
open Suave.Types
open Suave.Web
open Suave.Socket
open FSharpNews.Tests.Core

module WebServer =
    let run name port route =
        let binding = HttpBinding.Create(HTTP, System.Net.IPAddress.Any.ToString(), port)
        let config = { default_config with bindings = [binding] }
        let pipeline = choose [log_format >> dprintfn "---> %s: %s" name >> never
                               route]
        Async.Start (async { web_server config pipeline })

module StackExchangeApi =
    let private port = 4141
    let path = "/2.1/questions"
    let baseUrl = sprintf "http://%s:%d" Environment.machine port
    let runServer = WebServer.run "StackExchange" port

module TwitterApi =
    let private port = 4142

    let path = "/1.1/statuses/filter.json"
    let baseUrl = sprintf "http://%s:%d/1.1" Environment.machine port

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

    let writeEmptyInfinite (req: HttpRequest) = async {
        while true do
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
        async {
            do! writeHeaderBodyDelimeter req
            do! writeEmptyInfinite req })
