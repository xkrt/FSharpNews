namespace FSharpNews.Utils

open log4net
open log4net.Config

type Logger (name: string) =
    let log = log4net.LogManager.GetLogger name

    member x.Debug fmt = Printf.ksprintf log.Debug fmt
    member x.Info fmt = Printf.ksprintf log.Info fmt
    member x.Warn fmt = Printf.ksprintf log.Warn fmt
    member x.Error fmt = Printf.ksprintf log.Error fmt

    static member configure () = XmlConfigurator.Configure() |> ignore
    static member create name = new Logger(name)
    static member create<'a> () = new Logger(typeof<'a>.Name)
    static member create (ty: System.Type) = new Logger(ty.Name)
