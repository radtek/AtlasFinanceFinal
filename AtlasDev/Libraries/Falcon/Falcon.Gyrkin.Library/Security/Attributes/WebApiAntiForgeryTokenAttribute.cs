
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Falcon.Gyrkin.Library.Security.Attributes
{
  public class WebApiAntiForgeryTokenAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      HttpRequestMessage request = actionContext.Request;

      try
      {
        if (IsAjaxRequest(request))
        {
          Validate(actionContext);
        }
        else
        {
          AntiForgery.Validate();
        }
      }
      catch (Exception)
      {
        actionContext.Response = new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.Forbidden,
          RequestMessage = actionContext.ControllerContext.Request
        };
      }
      base.OnActionExecuting(actionContext);
    }

    private bool IsAjaxRequest(HttpRequestMessage request)
    {
      IEnumerable<string> xRequestedWithHeaders;
      if (request.Headers.TryGetValues("X-Requested-With", out xRequestedWithHeaders))
      {
        string headerValue = xRequestedWithHeaders.FirstOrDefault();
        if (!String.IsNullOrEmpty(headerValue))
        {
          return String.Equals(headerValue, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
        }
      }
      return false;
    }


    /// <summary>
    /// Validate token
    /// </summary>
    private void Validate(HttpActionContext context)
    {
      string cookieToken = "";
      string formToken = "";

      IEnumerable<string> tokenHeaders;
      if (context.Request.Headers.TryGetValues("forge_token", out tokenHeaders))
      {
        string[] tokens = tokenHeaders.First().Split(':');
        if (tokens.Length == 2)
        {
          cookieToken = tokens[0].Trim();
          formToken = tokens[1].Trim();
        }
      }
      AntiForgery.Validate(cookieToken, formToken);
    }
  }
}