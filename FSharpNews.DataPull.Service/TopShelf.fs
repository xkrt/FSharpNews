module FSharpNews.DataPull.Service.TopShelf

open System
open Topshelf
open Topshelf.HostConfigurators
open Topshelf.Runtime

let configureTopShelf f =
    HostFactory.Run(new Action<_>(f))

let addCommandLineDefinition str action (conf : HostConfigurator) =
    conf.AddCommandLineDefinition(str, new Action<_>(action))

let dependsOn (conf : HostConfigurator) name =
    conf.DependsOn name |> ignore

let dependsOnMongoDB (conf : HostConfigurator) =
    "MongoDB" |> dependsOn conf

let enableServiceRecovery (conf : HostConfigurator) f =
    conf.EnableServiceRecovery(new Action<_>(f)) |> ignore

let restartService delayInMinutes (recConf: ServiceRecoveryConfigurator) =
    recConf.RestartService(delayInMinutes) |> ignore

let runAsNetworkService (conf : HostConfigurator) =
    conf.RunAsNetworkService() |> ignore

let description str (conf: HostConfigurator) =
    conf.SetDescription str

let serviceName str (conf: HostConfigurator) =
    conf.SetServiceName str

let useNLog (conf: HostConfigurator) = conf.UseNLog()

let startAutomatically (conf : HostConfigurator) =
    conf.StartAutomatically() |> ignore

let service (conf : HostConfigurator) (fac : (unit -> 'a)) =
    let service' = conf.Service : Func<_> -> HostConfigurator
    service' (new Func<_>(fac)) |> ignore

let serviceControl (start : HostControl -> bool) (stop : HostControl -> bool) =
    { new ServiceControl with
        member x.Start hc = start hc
        member x.Stop hc = stop hc }
