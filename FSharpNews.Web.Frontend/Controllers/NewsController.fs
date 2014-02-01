namespace FSharpNews.Web.Frontend.Controllers

open System
open System.Web.Http
open Newtonsoft.Json
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Web.Frontend.Models

type NewsController() =
    inherit ApiController()

    member this.Get(addedFromDate: int64) : IHttpActionResult =
        let activityViewModels =
            addedFromDate
            |> DateTime.unixOffsetToUtcDate
            |> Storage.getActivitiesAddedSince
            |> Seq.map ActivityViewModel.Create
        this.Json(activityViewModels) :> _
