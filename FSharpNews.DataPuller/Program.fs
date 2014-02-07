module FSharpNews.DataPuller.Program

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

let private waitForCancelKey() =
    let event = new AutoResetEvent(false)
    Console.CancelKeyPress.Add(fun args ->
        do log.Info "Ctrl-C pressed, cancel"
        args.Cancel <- true
        do event.Set() |> ignore)
    do event.WaitOne() |> ignore

let private repeatForever (interval: TimeSpan) fn =
    async { let rec loop () =
                try fn()
                with
                | e -> do log.Error "%O" e
                Thread.Sleep(interval)
                loop()
            loop() }

let private stackExchange config =
    let fetchNewQuestions site =
        let lastQuestionTime = Storage.getTimeOfLastQuestion site
        let startDate = lastQuestionTime.AddSeconds(1.)
        let activitiesWithRaws = StackExchange.fetch config site startDate
        do log.Info "Fetched questions for %A: %d" site activitiesWithRaws.Length // todo move to module
        do Storage.saveAll activitiesWithRaws
    let repeat = repeatForever (TimeSpan.FromMinutes(5.))
    repeat (fun () -> StackExchange.allSites |> List.iter fetchNewQuestions)

let private twitter config =
    async { Twitter.listenStream config Storage.save }

let private nuget config =
    let fetchSince = NuGet.fetch config
    let fetchNewPackages () =
        let lastPackagePublishedDate = Storage.getTimeOfLastPackage()
        let pkgsWithRaw = fetchSince lastPackagePublishedDate
        do log.Info "Fetched packages: %d" pkgsWithRaw.Length              // todo move to module
        do Storage.saveAll pkgsWithRaw
    repeatForever (TimeSpan.FromMinutes(5.)) fetchNewPackages

[<EntryPoint>]
let main argv =
    do log.Info "Started"
    let seConfig, twiConfig, nuConfig = Configuration.get argv
    [stackExchange seConfig; twitter twiConfig; nuget nuConfig]
    |> Async.Parallel
    |> Async.Ignore
    |> Async.Start
    waitForCancelKey()
    0
