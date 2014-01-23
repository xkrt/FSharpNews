namespace FSharpNews.Tests.Core

open System

module Environment =
    let machine = Environment.MachineName.ToLower()

    let executingAssemblyDirPath () =
        let codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase
        let uri = UriBuilder(codeBase)
        let path = Uri.UnescapeDataString(uri.Path)
        IO.Path.GetDirectoryName(path)

[<AutoOpen>]
module Utils =
    do Diagnostics.Debug.AutoFlush <- true
    let dprintfn fmt = Printf.ksprintf Diagnostics.Debug.Print fmt

    let sleep secs = Threading.Thread.Sleep(secs * 1000)
