namespace FSharpNews.Web.Frontend

open System
open System.Net.Http
open System.Web
open System.Web.Http
open System.Web.Mvc
open System.Web.Routing
open System.Web.Optimization

// todo handle unhandled exceptions
// todo setup logging

type BundleConfig() =
    static member RegisterBundles (bundles:BundleCollection) =
        bundles.Add(ScriptBundle("~/bundles/libs").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/knockout-3.0.0.js",
                        "~/Scripts/moment.min.js"))
        bundles.Add(ScriptBundle("~/bundles/news").Include(
                        "~/Scripts/News.js",
                        "~/Scripts/knockout.extensions.js"))
        bundles.Add(StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/Site.css"))

type Route = { controller: string
               action: string
               id: UrlParameter }

type HttpRoute = { controller: string
                   id: RouteParameter }

type Global() =
    inherit System.Web.HttpApplication()

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
        AreaRegistration.RegisterAllAreas()
        GlobalConfiguration.Configure(Action<_> Global.RegisterWebApi)
        Global.RegisterFilters(GlobalFilters.Filters)
        Global.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles BundleTable.Bundles
