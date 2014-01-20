[<AutoOpen>]
module FSharpNews.Tests.Acceptance.TypedAssertions

open NUnit.Framework

/// Asserts that two values are equal.
let inline assertEqual<'T when 'T : equality> (expected : 'T) (actual : 'T) =
    Assert.AreEqual (expected, actual)
