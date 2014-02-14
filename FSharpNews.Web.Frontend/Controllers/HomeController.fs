namespace FSharpNews.Web.Frontend.Controllers

open System
open System.Web.Mvc
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Web.Frontend.Models

type HomeController() =
    inherit Controller()

#if DEBUG
    let requestNewsPeriod = 5 // secs
#else
    let requestNewsPeriod = 60 // secs
#endif

    member this.Index () =
        let batchSize = 100
        let news = Storage.getTopActivitiesByCreation batchSize
                   |> List.map ActivityViewModel.Create
        let config = PageConfiguration(news, requestNewsPeriod, batchSize)
        let json = Serializer.toJson config
        this.View(json :> obj)
