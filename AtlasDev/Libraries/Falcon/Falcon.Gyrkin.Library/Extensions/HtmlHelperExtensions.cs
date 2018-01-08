using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using Falcon.Gyrkin.Library.Common;

namespace Falcon.Gyrkin.Library.Extensions
{
  public static class HtmlHelperExtension
  {
    public static MvcHtmlString Json<T>(this HtmlHelper instance, T data)
    {
      return MvcHtmlString.Create(JsonNet.Serialize<T>(data, Newtonsoft.Json.TypeNameHandling.None));
    }

    public static string GetVersion(this HtmlHelper helper)
    {
      Assembly assembly = Assembly.GetExecutingAssembly();
      FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
      string version = fvi.FileVersion;
      return String.Format(string.Format("v {0}", fvi.FileVersion));
    }

    public static bool IsDebug(this HtmlHelper htmlHelper, HttpContext context)
    {
      if (context.Request.IsLocal)
        return true;
      else
        return false;
    }

    public static bool HasRole(this HtmlHelper htmlHelper, string claim, HttpContext context)
    {
      return ClaimsPrincipal.Current.HasClaim(ClaimTypes.Role, claim);
    }

    public static bool HasRole(this HtmlHelper htmlHelper, HttpContext context, params string[] claims)
    {
      List<bool> _claims = new List<bool>();

      foreach (var claim in claims)
        _claims.Add(ClaimsPrincipal.Current.HasClaim(ClaimTypes.Role, claim));

      return _claims.Contains(true);
    }

    public static string VersionStatic(this UrlHelper self, string contentPath)
    {
      string versionedContentPath = contentPath + "?v=" + Assembly.GetAssembly(typeof(HtmlHelperExtension)).GetName().Version.ToString().Replace(".","");
      return self.Content(versionedContentPath);
    }
  }
}