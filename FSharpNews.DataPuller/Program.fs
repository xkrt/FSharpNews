open System
open System.Threading
open FSharpNews.Data
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

let private stackExchange =
    let fetchNewQuestions site =
        let lastQuestionTime = Storage.getTimeOfLastQuestion site
        let activitiesWithRaws = StackExchange.fetch site (lastQuestionTime.AddSeconds(1.))
        log.Info "Fetched questions for %A: %d" site activitiesWithRaws.Length
        Storage.saveAll activitiesWithRaws
    let repeat = repeatForever (TimeSpan.FromMinutes(5.))
    repeat (fun () -> [Stackoverflow; Programmers] |> List.iter fetchNewQuestions)

let private twitter =
    async { Twitter.listenStream Storage.save }

[<EntryPoint>]
let main argv =
    log.Info "Started"
    [stackExchange; twitter]
    |> Async.Parallel
    |> Async.Ignore
    |> Async.Start
    waitForCancel()
    0
