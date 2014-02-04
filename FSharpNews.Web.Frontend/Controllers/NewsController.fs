namespace FSharpNews.Web.Frontend.Controllers

open System
open System.Web.Http
open Newtonsoft.Json
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Web.Frontend.Models

type NewsController() =
    inherit ApiController()

    [<HttpGet>]
    [<ActionName("since")>]
    member this.AddedSince(time: int64) : IHttpActionResult =
        let activityViewModels =
            time
            |> DateTime.unixOffsetToUtcDate
            |> Storage.getActivitiesAddedSince
            |> Seq.map ActivityViewModel.Create
        this.Json(activityViewModels) :> _

    [<HttpGet>]
    [<ActionName("earlier")>]
    member this.AddedEarlier(time: int64) : IHttpActionResult =
        let activityViewModels =
            time
            |> DateTime.unixOffsetToUtcDate
            |> Storage.getActivitiesAddedEarlier 100
            |> Seq.map ActivityViewModel.Create
        this.Json(activityViewModels) :> _
