namespace FSharpNews.Web.Frontend.Controllers

open System
open System.Web.Mvc
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Web.Frontend.Models

type StatusController() =
    inherit Controller()

    member this.Index () =
        let now = DateTime.UtcNow
        let activityTypes = Enum.GetValues(typeof<Storage.ActivityType>)
                            |> Seq.cast<Storage.ActivityType>
                            |> Seq.toList
        let model =
            { SourcesStats = activityTypes |> List.map (fun at -> { Name = Enum.GetName(typeof<Storage.ActivityType>, at)
                                                                    DayCount = Storage.getCountAddedSince at (now.AddDays -1.)
                                                                    WeekCount = Storage.getCountAddedSince at (now.AddDays -7.)
                                                                    MonthCount = Storage.getCountAddedSince at (now.AddMonths -1)
                                                                    OverallCount = Storage.getCount at }) }
        this.View(model)
