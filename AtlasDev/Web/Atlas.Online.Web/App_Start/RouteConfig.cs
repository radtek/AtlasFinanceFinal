using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Atlas.Online.Web
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");      

      routes.MapRoute(
        name: "MyAccount",
        url: "MyAccount",
        defaults: new { controller = "MyAccount", action = "Index" }
      );

      routes.MapRoute(
        name: "Default",
        url: "{controller}/{action}/{id}",
        defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );

      routes.MapRoute(
        "404-PageNotFound",
        "{*url}",
        new { controller = "HttpError", action = "PageNotFound" }
      );
    }
  }
}