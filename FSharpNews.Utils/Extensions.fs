[<AutoOpen>]
module FSharpNews.Utils.Extensions

open System

type String with
    member this.IsNullOrWs() = String.IsNullOrWhiteSpace(this)

    member this.IfNullOrWs(replacement: string) =
       if String.IsNullOrWhiteSpace(this)
       then replacement
       else this

    member this.EnsureEndsWith(suffix: string) =
       if this.EndsWith(suffix)
       then this
       else this + suffix

    member this.TrimToNull() =
        if this = null
        then null
        else let res = this.Trim()
             if res = "" then null else res

module Strings =
    let trim (s: string) = s.Trim()

type DateTime with
    member this.Truncate(timeSpan: TimeSpan) = this.AddTicks -(this.Ticks % timeSpan.Ticks)

module DateTime =
    let private startEpoch = DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)

    let unixToUtcDate (unix: int) = startEpoch.AddSeconds(float(unix))
    let toUnix (dateTime: DateTime) = dateTime.Subtract(startEpoch).TotalSeconds |> int

    let unixOffsetToUtcDate (unixOffset: int64) = startEpoch.AddMilliseconds(float(unixOffset))
    let toUnixOffset (dateTime: DateTime) = dateTime.Subtract(startEpoch).TotalMilliseconds |> int64
