using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Falcon.Gyrkin.Library.Extensions
{
  public static class CommonExtensions
  {
    #region Internal extensions
    internal static string EvalString(this HtmlHelper instance, string key)
    {
      return Convert.ToString(instance.ViewData.Eval(key), CultureInfo.CurrentCulture);
    }

    internal static string EvalString(this HtmlHelper instance, string key, string format)
    {
      return Convert.ToString(instance.ViewData.Eval(key, format), CultureInfo.CurrentCulture);
    }

    internal static object GetModelStateValue(this HtmlHelper instance, string key, Type destinationType)
    {
      ModelState modelState;
      if (instance.ViewData.ModelState.TryGetValue(key, out modelState))
      {
        if (modelState.Value != null)
        {
          return modelState.Value.ConvertTo(destinationType, null /* culture */);
        }
      }
      return null;
    }

    internal static bool EvalBoolean(this HtmlHelper instance, string key)
    {
      return Convert.ToBoolean(instance.ViewData.Eval(key), CultureInfo.InvariantCulture);
    }

    #endregion


    #region Request Extension

    public static string BaseURL(this HttpRequestMessage request)
    {
      return request.RequestUri.AbsoluteUri.Replace(request.RequestUri.PathAndQuery, String.Empty);
    }

    #endregion
  }
}