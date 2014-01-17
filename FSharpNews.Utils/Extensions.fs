[<AutoOpen>]
module FSharpNews.Utils.Extensions

open System

type System.String with
    member this.IsNullOrWs() = String.IsNullOrWhiteSpace(this)

    member this.IfNullOrWs(replacement: string) =
       if String.IsNullOrWhiteSpace(this)
       then replacement
       else this

module DateTime =
    let unixToUtcDate (unix: int) =
        let startEpoch = DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
        startEpoch.AddSeconds(float(unix))

    let toUnix (dateTime: DateTime) =
        let startEpoch = DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
        dateTime.Subtract(startEpoch).TotalSeconds |> int
