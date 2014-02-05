namespace FSharpNews.Tests.Frontend

open System.Diagnostics
open NUnit.Framework
open FSharpNews.Tests.Core

[<SetUpFixture>]
type TestsSetupper() =
    let killChromeDriver() =
        Process.GetProcessesByName("chromedriver")
        |> Array.iter (fun p -> p.Kill())

    [<SetUp>]
    member x.Setup() =
        do killChromeDriver()
        do Browser.start()

    [<TearDown>]
    member x.Teardown() =
        do Browser.close()
        do killChromeDriver()
