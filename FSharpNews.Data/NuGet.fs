module FSharpNews.Data.NuGet

open System
open Microsoft.FSharp.Data.TypeProviders
open FSharpNews.Utils

type private NuGet = ODataService<"https://www.nuget.org/api/v2">
let private context = NuGet.GetDataContext()

let fetch sinceDateExclusive =
    let rec loop result toSkip =
        let batchSize = 40
        let batch =
            query { for pkg in context.Packages do
                    sortByDescending pkg.Published
                    where (pkg.Tags.Contains("F#"))
                    where (pkg.Published > sinceDateExclusive)
                    skip toSkip
                    take batchSize
                    select pkg }
            |> Seq.toList
        let result' = batch @ result
        match batch.Length with
        | x when x = batchSize -> loop result' (toSkip + batchSize)
        | _ -> result'

    let pkgs = loop [] 0
    pkgs
    |> List.map (fun p -> NugetPackage { Id = p.Id
                                         Version = p.NormalizedVersion
                                         Url = p.GalleryDetailsUrl
                                         PublishedDate = DateTime(p.Published.Ticks, DateTimeKind.Utc) }
                          , Serializer.toJson p)
