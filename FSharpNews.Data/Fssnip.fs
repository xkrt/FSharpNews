module FSharpNews.Data.Fssnip

open System
open System.IO
open FSharp.Data
open FSharpx
open FSharpNews.Utils

type Configuration = { Url: string }

let private log = Logger.create "FsSnip"

let private parseId url = (Uri url).AbsolutePath.TrimStart('/').Trim()
let private toDateTime (publishedAgo: string) =
    let double m = m.GroupValues |> List.head |> Double.Parse
    let span = match publishedAgo.Trim() with
               | Regex.Interpreted.Match "(\\d+) secs ago" m -> m |> double |> TimeSpan.FromSeconds
               | Regex.Interpreted.Match "(\\d+) mins ago" m -> m |> double |> TimeSpan.FromMinutes
               | Regex.Interpreted.Match "(\\d+) hours ago" m -> m |> double |> TimeSpan.FromHours
               | Regex.Interpreted.Match "yesterday" _ -> TimeSpan.FromDays(1.)
               | Regex.Interpreted.Match "(\\d+) days ago" m -> m |> double |> TimeSpan.FromDays
               | Regex.Interpreted.Match "(\\d+) months ago" m -> m |> double |> (*) 30. |> TimeSpan.FromDays
               | Regex.Interpreted.Match "(\\d+) years ago" m -> m |> double |> (*) 365. |> TimeSpan.FromDays
               | s -> failwithf "Cant parse: %s" s
    DateTime.UtcNow.Subtract(span)

let fetch config =
    let api = new ApiaryProvider<"fssnip">(config.Url)
    let snips = api.Snippet.List() |> Array.toList
    do log.Info "Fetched snippets: %d" snips.Length
    snips
    |> List.map (fun s -> FsSnippet { Id = parseId s.Link
                                      Title = s.Title
                                      Author = s.Author
                                      PublishedDate = toDateTime s.Published
                                      Url = s.Link }
                          , s.JsonValue.ToString() )
