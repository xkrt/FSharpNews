namespace FSharpNews.Web.Frontend

open System
open System.Net.Http
open System.Web
open System.Web.Http
open System.Web.Mvc
open System.Web.Routing
open System.Web.Optimization
open FSharpNews.Utils

type BundleConfig() =
    static member RegisterBundles (bundles:BundleCollection) =
        bundles.UseCdn <- true
        // libs
        bundles.Add(ScriptBundle("~/bundles/js/jquery", "//cdnjs.cloudflare.com/ajax/libs/jquery/2.1.0/jquery.min.js")
                        .Include [| "~/Scripts/jquery-2.1.0.js" |])
        bundles.Add(ScriptBundle("~/bundles/js/knockout", "//cdnjs.cloudflare.com/ajax/libs/knockout/3.0.0/knockout-min.js")
                        .Include [| "~/Scripts/knockout-3.0.0.js" |])
        bundles.Add(ScriptBundle("~/bundles/js/moment", "//cdnjs.cloudflare.com/ajax/libs/moment.js/2.5.1/moment.min.js")
                        .Include [| "~/Scripts/moment.min.js" |])
        bundles.Add(StyleBundle("~/Content/css/bootstrap", "//cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/3.1.0/css/bootstrap.min.css")
                        .Include [| "~/Content/bootstrap.css" |])
        // fsharpnews
        bundles.Add(ScriptBundle("~/bundles/js/fsharpnews").Include(
                        "~/Scripts/News.js",
                        "~/Scripts/knockout.extensions.js"))
        bundles.Add(StyleBundle("~/Content/css/fsharpnews").Include [| "~/Content/Site.css" |])

type Route = { controller: string
               action: string
               id: UrlParameter }

type HttpRoute = { controller: string
                   id: RouteParameter }

type Global() =
    inherit System.Web.HttpApplication()

    let log = Logger.create "Program"

    static member RegisterWebApi(config: HttpConfiguration) =
        config.MapHttpAttributeRoutes()
        config.Routes.MapHttpRoute(
            "DefaultApi",
            "api/{controller}/{action}"
        ) |> ignore

    static member RegisterFilters(filters: GlobalFilterCollection) =
        filters.Add(new HandleErrorAttribute())

    static member RegisterRoutes(routes:RouteCollection) =
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.MapRoute(
            "Default",
            "{controller}/{action}/{id}",
            { controller = "Home"; action = "Index"; id = UrlParameter.Optional }
        ) |> ignore

    member x.Application_Start() =
        log.Info "Application start"
        do AppDomain.CurrentDomain.UnhandledException.Add UnhandledExceptionLogger.handle

        AreaRegistration.RegisterAllAreas()
        GlobalConfiguration.Configure(Action<_> Global.RegisterWebApi)
        Global.RegisterFilters(GlobalFilters.Filters)
        Global.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles BundleTable.Bundles

    member x.Application_End() =
        log.Info "Application end"
