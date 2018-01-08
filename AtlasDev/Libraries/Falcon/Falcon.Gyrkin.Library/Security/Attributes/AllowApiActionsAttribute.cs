using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Falcon.Gyrkin.Library.Security.Attributes
{
  /// <summary>
  /// Attribute used to define whether api calls are allowed.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public class AllowApiActionsAttribute : FilterAttribute, IActionFilter
  {
    private static List<string> allowedActions = null;

    public AllowApiActionsAttribute(Type controllerType, params string[] allowedMethods)
    {
      if (!controllerType.IsSubclassOf(typeof(ApiController)))
        throw new InvalidOperationException("Must specify a ApiController type as an allowed controller.");

      string baseName = controllerType.Name;

      if (!baseName.EndsWith("Controller"))
        throw new InvalidOperationException("All controller classes must end with 'Controller'.");

      // Cut off "Controller"
      baseName = baseName.Substring(0, baseName.Length - 10);

      IEnumerable<MethodInfo> methods = controllerType.GetMethods().Where(m => m.IsPublic && m.DeclaringType.Equals(controllerType));

      if (allowedMethods.Length > 0)
      {
        // Only use explicitly allowed methods
        methods = methods.Where(m =>
        {
          return allowedMethods.Contains(m.Name);
        });
      }

      AllowedActions.AddRange(methods.Select<MethodInfo, string>(m =>
      {
        return String.Format("{0}.{1}", baseName, m.Name);
      }));
    }

    public AllowApiActionsAttribute(params string[] allowedActions)
    {
      AllowedActions.AddRange(allowedActions);
    }

    public static string HttpAntiForgeryCookieName
    {
      get { return "XSRF-TOKEN"; }
    }

    protected static List<string> AllowedActions
    {
      get
      {
        if (allowedActions == null)
          return allowedActions = new List<string>();

        return allowedActions;
      }
    }

    public void OnActionExecuted(ActionExecutedContext filterContext)
    {
      var oldCookieToken = TryGetOldCookieToken(filterContext);

      var xsrfCookie = new HttpCookie(HttpAntiForgeryCookieName, JavascriptSecurity.GenerateToken(AllowedActions, oldCookieToken));

      filterContext.HttpContext.Response.SetCookie(xsrfCookie);
    }

    public void OnActionExecuting(ActionExecutingContext filterContext)
    {
      // Do nothing
    }

    private string TryGetOldCookieToken(ActionExecutedContext filterContext)
    {
      HttpRequestBase request = filterContext.HttpContext.Request;
      try
      {
        var cookie = request.Cookies[HttpAntiForgeryCookieName];
        if (cookie != null)
          return cookie.Value;
      }
      catch { }

      return null;
    }
  }
}