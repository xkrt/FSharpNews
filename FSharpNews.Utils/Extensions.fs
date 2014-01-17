﻿[<AutoOpen>]
module FSharpNews.Utils.Extensions

open System

type System.String with
    member this.IsNullOrWs() = String.IsNullOrWhiteSpace(this)

    member this.IfNullOrWs(replacement: string) =
       if String.IsNullOrWhiteSpace(this)
       then replacement
       else this

module DateTime =
    let private startEpoch = DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)

    let unixToUtcDate (unix: int) = startEpoch.AddSeconds(float(unix))
    let toUnix (dateTime: DateTime) = dateTime.Subtract(startEpoch).TotalSeconds |> int

    let unixOffsetToUtcDate (unixOffset: int64) = startEpoch.AddMilliseconds(float(unixOffset))
    let toUnixOffset (dateTime: DateTime) = dateTime.Subtract(startEpoch).TotalMilliseconds |> int64
