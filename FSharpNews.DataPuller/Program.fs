open System
open FSharpNews.Data
open FSharpNews.DataProviders
open FSharpNews.Utils

do Logger.configure()

do AppDomain.CurrentDomain.UnhandledException.Add(fun e ->
    let log = Logger.create "Unhandled"
    if (e.ExceptionObject :? Exception)
    then log.Error "Domain unhandled exception of type %s occured (%s)" (e.GetType().Name) (e.ExceptionObject.ToString())
    else log.Error "Unhandled non-CLR exception occured (%s)" (e.ExceptionObject.ToString()))

let private log = Logger.create "Program"

let fetch site =
    let qs = StackExchange.fetch site (DateTime(2014, 1, 1, 0, 0, 0, DateTimeKind.Utc))
    log.Info "Fetched questions for %A: %d" site qs.Length
    Storage.saveAll qs

[<EntryPoint>]
let main argv =
    [StackExchangeSite.Stackoverflow; StackExchangeSite.Programmers]
    |> List.iter fetch
    0
