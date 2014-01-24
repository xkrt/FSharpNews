module FSharpNews.Tests.Core.ExtensionsTests

open System
open NUnit.Framework
open FSharpNews.Utils

[<Test>]
let ``DateTime truncate``() =
    let fmt (dt: DateTime) = dt.ToString("HH:mm:ss.fffffff")
    let dt = DateTime.Parse("09:10:35.7654321")

    dt |> fmt |> assertEqual "09:10:35.7654321"
    dt.Truncate(TimeSpan.FromMilliseconds(1.)) |> fmt |> assertEqual "09:10:35.7650000"
    dt.Truncate(TimeSpan.FromSeconds(1.)) |> fmt |> assertEqual "09:10:35.0000000"
    dt.Truncate(TimeSpan.FromMinutes(1.)) |> fmt |> assertEqual "09:10:00.0000000"


