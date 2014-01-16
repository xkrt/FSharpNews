namespace FSharpNews.Utils

module Seq =
   let tryHead (source : seq<_>) =
        use e = source.GetEnumerator()
        if e.MoveNext()
        then Some(e.Current)
        else None

module List =
    let zipWith f list1 = List.zip list1 (List.map f list1)
