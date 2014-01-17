namespace FSharpNews.Web.Frontend.Controllers
open System
open System.Web.Http
open Newtonsoft.Json
open FSharpNews.Data
open FSharpNews.Utils

type NewsController() =
    inherit ApiController()

    member this.Get(fromDate: int64) : IHttpActionResult =
        let activityViewModels =
            Storage.getActivities (DateTime.unixOffsetToUtcDate fromDate)
            |> Seq.map ActivityViewModel.Create
        this.Json(activityViewModels) :> _
