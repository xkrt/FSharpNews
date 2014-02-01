namespace FSharpNews.Tests.Frontend

open System.Diagnostics
open NUnit.Framework
open canopy
open FSharpNews.Tests.Core

[<SetUpFixture>]
type TestsSetupper() =
    let killChromeDriver() =
        Process.GetProcessesByName("chromedriver")
        |> Array.iter (fun p -> p.Kill())

    [<SetUp>]
    member x.Setup() =
        do killChromeDriver()
        canopy.configuration.elementTimeout <- 5.
        canopy.configuration.chromeDir <- Environment.executingAssemblyDirPath()
        do start chrome
        do pin Left

    [<TearDown>]
    member x.Teardown() =
        do quit chrome
        do killChromeDriver()
