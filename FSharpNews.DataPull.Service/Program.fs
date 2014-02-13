module FSharpNews.DataPull.Service.Program

open System
open System.Threading
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.DataPull.Service.TopShelf

do Logger.configure()
let private log = Logger.create "Program"

do AppDomain.CurrentDomain.UnhandledException.Add(fun e ->
    let log = Logger.create "Unhandled"
    if (e.ExceptionObject :? Exception)
    then log.Error "Domain unhandled exception of type %s occured (%s)" (e.GetType().Name) (e.ExceptionObject.ToString())
    else log.Error "Unhandled non-CLR exception occured (%s)" (e.ExceptionObject.ToString()))

let private repeatForever (interval: TimeSpan) fn =
    async { let rec loop () =
                try fn()
                with
                | e -> do log.Error "%O" e
                Thread.Sleep(interval)
                loop()
            loop() }

let private stackExchange (conf: Configuration.Type) =
    let fetchNewQuestions site =
        let lastQuestionTime = Storage.getTimeOfLastQuestion site
        let startDate = lastQuestionTime.AddSeconds(1.)
        let activitiesWithRaws = StackExchange.fetch conf.StackExchange site startDate
        do Storage.saveAll activitiesWithRaws
    let repeat = repeatForever (TimeSpan.FromMinutes(5.))
    repeat (fun () -> StackExchange.allSites |> List.iter fetchNewQuestions)

let private twitter (conf: Configuration.Type) =
    async { Twitter.listenStream conf.Twitter Storage.save }

let private nuget (conf: Configuration.Type) =
    let fetchSince = NuGet.fetch conf.NuGet
    let fetchNewPackages () =
        let lastPackagePublishedDate = Storage.getTimeOfLastPackage()
        let pkgsWithRaw = fetchSince lastPackagePublishedDate
        do Storage.saveAll pkgsWithRaw
    repeatForever (TimeSpan.FromMinutes(5.)) fetchNewPackages

[<EntryPoint>]
let main argv =
    do log.Info "Program started"
    let seUrl = ref None
    let twiUrl = ref None
    let nuUrl = ref None

    let startPullingData() =
        let conf = Configuration.build !seUrl !twiUrl !nuUrl
        [stackExchange conf; twitter conf; nuget conf]
        |> Async.Parallel
        |> Async.Ignore
        |> Async.Start

    let start _ = startPullingData(); true
    let stop _ = true
    let runService () = serviceControl start stop

    let configureService conf =
        conf |> serviceName "FSharpNews.DataPull.Service"
        conf |> description "FSharp News data pull service"
        conf |> runAsNetworkService
        conf |> startAutomatically
        conf |> dependsOnMongoDB
        conf |> enableServiceRecovery <| restartService 1
        conf |> useLog4Net
        conf |> addCommandLineDefinition "stackExchangeUrl" (fun url -> seUrl := Some url)
        conf |> addCommandLineDefinition "twitterUrl" (fun url -> twiUrl := Some url)
        conf |> addCommandLineDefinition "nugetUrl" (fun url -> nuUrl := Some url)

    let exitCode = configureTopShelf <| fun conf ->
        do configureService conf
        do runService |> service conf
    log.Info "Exiting, code=%A" exitCode
    int exitCode
