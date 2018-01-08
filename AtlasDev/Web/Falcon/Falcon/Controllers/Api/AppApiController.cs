using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Mvc;

using Falcon.Services;


namespace Falcon.Controllers.Api
{
  public abstract class AppApiController : ApiController
  {
    protected const string REDIRECT_HEADER = "X-Client-Redirect";

    private SharedServices services = null;


    protected SharedServices Services
    {
      get
      {
        if (services == null)
        {
          services = new SharedServices();
        }

        return services;
      }
    }


    //protected Client CurrentClient
    //{
    //  get { return Services.CurrentClient; }
    //}

    protected MediaTypeFormatter DefaultFormatter
    {
      get
      {
        return GlobalConfiguration.Configuration.Formatters.First();
      }
    }


    protected HttpResponseMessage RedirectToAction(string actionName, string controllerName, object routeAttributes = null, string routeName = "Default")
    {
      var merged = HtmlHelper.AnonymousObjectToHtmlAttributes(new { controller = controllerName, action = actionName });
      if (routeAttributes != null)
      {
        var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(routeAttributes);
        foreach (var item in attrs)
        {
          merged.Add(item.Key, item.Value);
        }
      }

      return RedirectTo(merged, routeName);
    }


    protected HttpResponseMessage RedirectTo(object routeParams, string routeName = "Default")
    {
      return RedirectTo(new Uri(Url.Link(routeName, routeParams)));
    }


    protected HttpResponseMessage RedirectTo(Uri uri)
    {
      var response = Request.CreateResponse(HttpStatusCode.NoContent);
      response.Headers.Add(REDIRECT_HEADER, uri.ToString());
      return response;
    }
  }
}
