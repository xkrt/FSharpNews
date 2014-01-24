namespace FSharpNews.Web.Frontend.Controllers

open System
open System.Web.Mvc
open Newtonsoft.Json
open FSharpNews.Data

type HomeController() =
    inherit Controller()

    member this.Index () =
        let json =
            Storage.getTopActivitiesByCreation 100
            |> Seq.map ActivityViewModel.Create
            |> JsonConvert.SerializeObject
        this.View(json :> obj)
