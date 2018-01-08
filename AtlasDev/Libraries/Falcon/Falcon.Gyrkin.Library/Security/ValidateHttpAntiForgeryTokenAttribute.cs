using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace Falcon.Security
{
  /// <summary>
  /// Validates Anti-Forgery CSRF tokens for Web API
  /// </summary>
  /// <remarks>
  /// See MVC 4 SPA template
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class ValidateHttpAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
  {
    public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
    {
      HttpRequestMessage request = actionContext.Request;

      try
      {
        var actionName = String.Join(".", new string[] {
          actionContext.ControllerContext.ControllerDescriptor.ControllerName,
          actionContext.ActionDescriptor.ActionName
        });

        if (IsAjaxRequest(request))
        {
          ValidateRequestHeader(request, actionName);
        }
        else
        {
          AntiForgery.Validate();
        }
      }
      catch (Exception ex)
      {
        //Elmah.ErrorLog.GetDefault(HttpContext.Current).Log(new Elmah.Error(ex));

        actionContext.Response = new HttpResponseMessage
        {
          StatusCode = HttpStatusCode.Forbidden,
          RequestMessage = actionContext.ControllerContext.Request
        };
        return FromResult(actionContext.Response);
      }
      return continuation();
    }

    private Task<HttpResponseMessage> FromResult(HttpResponseMessage result)
    {
      var source = new TaskCompletionSource<HttpResponseMessage>();
      source.SetResult(result);
      return source.Task;
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

    private void ValidateRequestHeader(HttpRequestMessage request, string actionName)
    {
      var headers = request.Headers;

      IEnumerable<string> xXsrfHeaders;

      if (headers.TryGetValues("X-XSRF-Token", out xXsrfHeaders))
      {
        JavascriptSecurity.ValidateToken(actionName, xXsrfHeaders.FirstOrDefault());
      }
      else
      {
        var headerBuilder = new StringBuilder();

        headerBuilder.AppendLine("Missing X-XSRF-Token HTTP header:");

        foreach (var header in headers)
        {
          headerBuilder.AppendFormat("- [{0}] = {1}", header.Key, header.Value);
          headerBuilder.AppendLine();
        }

        throw new InvalidOperationException(headerBuilder.ToString());
      }
    }

    //public void OnAuthorization(AuthorizationContext filterContext)
    //{
    //  throw new NotImplementedException();
    //}
  }
}