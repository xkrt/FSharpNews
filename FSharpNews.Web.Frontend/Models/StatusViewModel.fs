namespace FSharpNews.Web.Frontend.Models

open System
open System.Web.Http
open FSharpNews.Data
open FSharpNews.Utils

type SourceStat = { Name: string
                    DayCount: int
                    WeekCount: int
                    MonthCount: int
                    OverallCount: int }

type StatusViewModel = { SourcesStats: SourceStat list }
