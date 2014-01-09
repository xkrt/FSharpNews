open System
open System.Threading
open FSharpNews.Data
open FSharpNews.DataProviders
open FSharpNews.Utils

[<Literal>]
let repeatInterval = 5.0

do Logger.configure()
let private log = Logger.create "Program"

do AppDomain.CurrentDomain.UnhandledException.Add(fun e ->
    let log = Logger.create "Unhandled"
    if (e.ExceptionObject :? Exception)
    then log.Error "Domain unhandled exception of type %s occured (%s)" (e.GetType().Name) (e.ExceptionObject.ToString())
    else log.Error "Unhandled non-CLR exception occured (%s)" (e.ExceptionObject.ToString()))

let private waitForCancel () =
    printfn "To cancel press Ctrl+C"
    let event = new AutoResetEvent(false)
    Console.CancelKeyPress.Add(fun args ->
        args.Cancel <- true
        event.Set() |> ignore)
    event.WaitOne() |> ignore

let private repeatForever fn =
    async {
        let rec loop () =
            try fn()
            with
            | e -> log.Error "Error"
            Thread.Sleep(TimeSpan.FromMinutes(repeatInterval))
            loop()
        loop()
    }

let private stackExchange () =
    let fetch site =
        let lastQuestionTime = Storage.getLastQuestionTime site
        let qs = StackExchange.fetch site (lastQuestionTime.AddSeconds(1.0))
        log.Info "Fetched questions for %A: %d" site qs.Length
        Storage.saveAll qs
    [StackExchangeSite.Stackoverflow; StackExchangeSite.Programmers] |> List.iter fetch

[<EntryPoint>]
let main argv =
    repeatForever stackExchange |> Async.Start
    waitForCancel()
    0
