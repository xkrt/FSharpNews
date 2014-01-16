[<AutoOpen>]
module FSharpNews.Utils.Extensions

open System

type System.String with
    member this.IsNullOrWs() = String.IsNullOrWhiteSpace(this)

    member this.IfNullOrWs(replacement: string) =
       if String.IsNullOrWhiteSpace(this)
       then replacement
       else this
