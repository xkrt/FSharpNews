module FSharpNews.Data.NuGet

open System
open Microsoft.FSharp.Data.TypeProviders
open FSharpNews.Utils

let private log = Logger.create "StackExchange"

type Configuration = { Url: string }
type private NuGet = ODataService<"https://www.nuget.org/api/v2">

let fetch config sinceDateExclusive =
    let context = NuGet.GetDataContext(Uri config.Url)
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
    do log.Info "Fetched packages: %d" pkgs.Length
    pkgs |> List.map (fun p -> NugetPackage { Id = p.Id
                                              Version = p.NormalizedVersion
                                              Url = p.GalleryDetailsUrl
                                              PublishedDate = DateTime(p.Published.Ticks, DateTimeKind.Utc) }
                               , Serializer.toJson p)
