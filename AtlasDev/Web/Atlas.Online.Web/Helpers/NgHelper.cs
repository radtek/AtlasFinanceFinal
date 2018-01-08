using Atlas.Online.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Atlas.Online.Web.Extensions.Ng;

namespace Atlas.Online.Web.Helpers
{
  public static class Ng
  {
    public static MvcHtmlString App(WebViewPage<dynamic> view) 
    {
      object ngApp = view.ViewBag.ngApp;

      return MvcHtmlString.Create((ngApp != null) ?
        String.Format("id='ng-app'  ng-app='{0}'", ngApp) :
        String.Empty
      );
    }

    public static MvcHtmlString Slider(string model = null, string min = null, string max = null, IDictionary<string, object> attributes = null)
    {
      TagBuilder div = new TagBuilder("div");
      
      div.MergeAttribute("atl-slider-bar", null);

      if (!String.IsNullOrEmpty(min))
      {
        div.MergeAttribute("min", min);
      }

      if (!String.IsNullOrEmpty(max))
      {
        div.MergeAttribute("max", max);
      }

      if (attributes != null)
      {
        div.MergeAttributes(attributes);
      }

      if (!String.IsNullOrEmpty(model))
      {
        div.MergeAttribute("ng-model", model);
      }

      return div.ToMvcHtmlString();
    }

  }
}