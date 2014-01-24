namespace FSharpNews.Utils

module Seq =
   let tryHead (source : seq<_>) =
        use e = source.GetEnumerator()
        if e.MoveNext()
        then Some(e.Current)
        else None

module List =
    let zipWith f list1 = List.zip list1 (List.map f list1)

    let exactlyOne list =
        match list with
        | [] -> invalidArg "list" "The list is empty."
        | [x] -> x
        | _ -> invalidArg "list" "The list contains more than one element."
