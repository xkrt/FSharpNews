module FSharpNews.Tests.DataPull.Service.GistsTests

open System
open NUnit.Framework
open Suave.Types
open Suave.Http
open FSharpNews.Data
open FSharpNews.Utils
open FSharpNews.Tests.Core

[<SetUp>]
let Setup() = do Storage.deleteAll()

let runGistsWith json = GitHubApi.runServer (GET >>= url GitHubApi.gistsPath
                                                 >>= (set_mime_type "application/json"
                                                 >> OK json))

[<Test>]
let ``One F# gist returned by api => one activity in storage``() =
    use gs = runGistsWith TestData.Gist.fsharpJson
    do ServiceApplication.startAndSleep Gists

    Storage.getAllActivities()
    |> List.map fst
    |> List.exactlyOne
    |> function Gist g -> g | x -> failwithf "Expected Gist, but was %O" (x.GetType())
    |> assertEqual TestData.Gist.gist

[<Test>]
let ``Non-F# gists filtered``() =
    use gs = runGistsWith TestData.Gist.nonFsharpJson
    do ServiceApplication.startAndSleep Gists
    Storage.getAllActivities() |> assertEqual []

[<Test>]
let ``Gists with non-english description filtered``() =
    use gs = runGistsWith TestData.Gist.japanFsharpJson
    do ServiceApplication.startAndSleep Gists
    Storage.getAllActivities() |> assertEqual []

[<Test>]
let ``Gist with empty description saved with null description``() =
    use gs = runGistsWith TestData.Gist.emptyDescription
    do ServiceApplication.startAndSleep Gists
    
    Storage.getAllActivities()
    |> List.map fst
    |> List.exactlyOne
    |> function Gist g -> g | x -> failwithf "Expected Gist, but was %O" (x.GetType())
    |> (fun g -> g.Description |> assertEqual None)
