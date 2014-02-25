namespace FSharpNews.Utils

open System
open NLog

type Logger (name: string) =
    let log = LogManager.GetLogger name

    member x.Debug fmt = Printf.ksprintf log.Debug fmt
    member x.Info fmt = Printf.ksprintf log.Info fmt
    member x.Warn fmt = Printf.ksprintf log.Warn fmt
    member x.Error fmt = Printf.ksprintf log.Error fmt
    member x.Fatal fmt = Printf.ksprintf log.Fatal fmt

    static member create name = new Logger(name)
    static member create<'a> () = new Logger(typeof<'a>.Name)
    static member create (ty: System.Type) = new Logger(ty.Name)
