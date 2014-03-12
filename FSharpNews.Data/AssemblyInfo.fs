module internal AssemblyInfo

open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

// Version information
[<assembly: AssemblyVersion("1.0.0")>]
[<assembly: AssemblyFileVersion("1.0.0")>]
[<assembly: AssemblyInformationalVersion("1.0.0")>]

// Assembly information
[<assembly: AssemblyTitle("FSharpNews.Data")>]
[<assembly: AssemblyDescription("FSharpNews.Data")>]
[<assembly: Guid("146dcc98-7149-4f78-b0c2-ed2779064108")>]
[<assembly: AssemblyCopyright("Copyright © Pavel Martynov 2014")>]
[<assembly: ComVisible(false)>]

// Only allow types derived from System.Exception to be thrown --
// any other types should be automatically wrapped.
[<assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)>]

#if DEBUG
[<assembly: InternalsVisibleTo("FSharpNews.Tests.DataPull.Service")>]
[<assembly: InternalsVisibleTo("FSharpNews.Tests.Frontend")>]
#endif

do ()
