open System
open System.Configuration
open System.Threading
open FSharpNews.Data
open FSharpNews.Data.StackExchange
open FSharpNews.Data.Twitter
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

let private buildConfigs argv =
    let cfg = ConfigurationManager.AppSettings

    let seApiKey = cfg.["StackExchangeApiKey"]
    let twiConsumerKey = cfg.["TwitterConsumerKey"]
    let twiConsumerSecret = cfg.["TwitterConsumerSecret"]
    let twiAccessToken = cfg.["TwitterAccessToken"]
    let twiAccessTokenSecret = cfg.["TwitterAccessTokenSecret"]

    let seApiUrl, twiStreamApiUrl =
        match Array.toList argv with
        | opt::stackExchangeUrl::twitterUrl::[] when opt = "-test" -> stackExchangeUrl, twitterUrl
        | [] -> cfg.["StackExchangeApiUrl"],
                cfg.["TwitterStreamingApiUrl"]
        | _ -> failwith "Wrong command line parameters"

    let seConfig = { ApiKey = seApiKey
                     ApiUrl = seApiUrl }
    let twiConfig = { ConsumerKey = twiConsumerKey
                      ConsumerSecret = twiConsumerSecret
                      AccessToken = twiAccessToken
                      AccessTokenSecret = twiAccessTokenSecret
                      StreamApiUrl = twiStreamApiUrl }
    seConfig, twiConfig

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
        do log.Info "Fetched questions for %A: %d" site activitiesWithRaws.Length // todo move to module
        do Storage.saveAll activitiesWithRaws
    let repeat = repeatForever (TimeSpan.FromMinutes(5.))
    repeat (fun () -> [Stackoverflow; Programmers] |> List.iter fetchNewQuestions)

let private twitter config =
    async { Twitter.listenStream config Storage.save }

let private nuget () =
    let fetchNewPackages () =
        let lastPackagePublishedDate = Storage.getTimeOfLastPackage()
        let pkgsWithRaw = NuGet.fetch lastPackagePublishedDate
        do log.Info "Fetched packages: %d" pkgsWithRaw.Length              // todo move to module
        do Storage.saveAll pkgsWithRaw
    repeatForever (TimeSpan.FromMinutes(5.)) fetchNewPackages

[<EntryPoint>]
let main argv =
    do log.Info "Started"
    let seConfig, twiConfig = buildConfigs argv
    [stackExchange seConfig; twitter twiConfig; nuget()]
    |> Async.Parallel
    |> Async.Ignore
    |> Async.Start
    waitForCancel()
    0
