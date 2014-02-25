module FSharpNews.Utils.UnhandledExceptionLogger

open System
open FSharpNews.Utils

let handle (e: UnhandledExceptionEventArgs) =
    let log = Logger.create "Unhandled"
    if (e.ExceptionObject :? Exception)
    then log.Fatal "Domain unhandled exception of type %s occured (%s)" (e.GetType().Name) (e.ExceptionObject.ToString())
    else log.Fatal "Unhandled non-CLR exception occured (%s)" (e.ExceptionObject.ToString())
