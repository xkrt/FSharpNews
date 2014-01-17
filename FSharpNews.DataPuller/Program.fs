open System
open System.Configuration
open System.Threading
open FSharpNews.Data
open FSharpNews.Data.StackExchange
open FSharpNews.Utils

do Logger.configure()
let private log = Logger.create "Program"

do AppDomain.CurrentDomain.UnhandledException.Add(fun e ->
    let log = Logger.create "Unhandled"
    if (e.ExceptionObject :? Exception)
    then log.Error "Domain unhandled exception of type %s occured (%s)" (e.GetType().Name) (e.ExceptionObject.ToString())
    else log.Error "Unhandled non-CLR exception occured (%s)" (e.ExceptionObject.ToString()))

let private waitForCancel () =
    let event = new AutoResetEvent(false)
    Console.CancelKeyPress.Add(fun args ->
        args.Cancel <- true
        event.Set() |> ignore)
    event.WaitOne() |> ignore

let private repeatForever (interval: TimeSpan) fn =
    async {
        let rec loop () =
            try fn()
            with
            | e -> do log.Error "%O" e
            Thread.Sleep(interval)
            loop()
        loop()
    }

let private stackExchange config =
    let fetchNewQuestions site =
        let lastQuestionTime = Storage.getTimeOfLastQuestion site
        let startDate = lastQuestionTime.AddSeconds(1.)
        let activitiesWithRaws = StackExchange.fetch config site startDate
        do log.Info "Fetched questions for %A: %d" site activitiesWithRaws.Length
        do Storage.saveAll activitiesWithRaws
    let repeat = repeatForever (TimeSpan.FromMinutes(5.))
    repeat (fun () -> [Stackoverflow; Programmers] |> List.iter fetchNewQuestions)

let private twitter =
    async { Twitter.listenStream Storage.save }

let buildConfigs argv =
    let seApiKey = ConfigurationManager.AppSettings.["StackExchangeApiKey"]
    let seApiUrl =
        match Array.toList argv with
        | opt::stackExchangeUrl::[] when opt = "-test" -> stackExchangeUrl
        | [] -> Configuration.ConfigurationManager.AppSettings.["StackExchangeApiUrl"]
        | _ -> failwith "Wrong command line parameters"
    { ApiKey=seApiKey
      ApiUrl=seApiUrl }

[<EntryPoint>]
let main argv =
    do log.Info "Started"
    let seConfig = buildConfigs argv
    [stackExchange seConfig; twitter]
    |> Async.Parallel
    |> Async.Ignore
    |> Async.Start
    waitForCancel()
    0
