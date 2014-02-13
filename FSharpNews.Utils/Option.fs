[<AutoOpen>]
module FSharpNews.Utils.Option

open System

let inline fill defaultValue (value: 'T option) =
    defaultArg value defaultValue

let fromNullable (n: _ Nullable) =
    if n.HasValue
        then Some n.Value
        else None
let toNullable =
    function
    | None -> Nullable()
    | Some x -> Nullable(x)

let (|Null|_|) (x: _ Nullable) =
   if x.HasValue
       then None
       else Some()

let (|Value|_|) = fromNullable
