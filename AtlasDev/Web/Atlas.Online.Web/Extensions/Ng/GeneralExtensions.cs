using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Extensions.Ng
{
  public static class GeneralExtensions
  {
    //<div class="d-iblock" ng-show="[show]">
    //  <img src="@Url.Content("~/Assets/img/loader.gif")" alt="loading..." width="16" height="16" class="d-iblock mr-micro"> <span class="d-iblock va-m">loading...</span>
    //</div>
    public static MvcHtmlString NgLoader(this HtmlHelper instance, string text = "loading...", string ngShow = null, object htmlAttributes = null)
    {
      // <div>
      TagBuilder div = new TagBuilder("div");

      div.AddCssClass("col col-md-2-3 xsm-no-push col-push-pull push-4 mb-alpha ng-cloak");

      if (htmlAttributes != null)
      {
        div.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
      }

      if (ngShow != null)
      {
        div.MergeAttribute("ng-show", ngShow);
      }

      // <img>
      TagBuilder img = new TagBuilder("img");

      img.MergeAttribute("src", UrlHelper.GenerateContentUrl("~/Assets/img/loader.gif", instance.ViewContext.HttpContext));
      img.AddCssClass("d-iblock mr-micro");
      img.MergeAttribute("alt", text);
      img.MergeAttribute("width", "16");
      img.MergeAttribute("height", "16");

      // <span>
      TagBuilder span = new TagBuilder("span");
      span.AddCssClass("d-iblock va-m");
      span.SetInnerText(text);

      div.InnerHtml = img.ToString(TagRenderMode.SelfClosing) + span.ToString();

      return div.ToMvcHtmlString();
    }

    public static MvcHtmlString NgTooltip(this HtmlHelper instance, string title = null, string options = null, object htmlAttributes = null)
    {
      TagBuilder anchor = new TagBuilder("a");

      anchor.AddCssClass("tooltip-help va-m");

      anchor.Attributes.Add("href", "javascript:void(0);");
      anchor.Attributes.Add("title", title);
      anchor.Attributes.Add("f8-tooltip", options);

      if (htmlAttributes != null)
      {
        anchor.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
      }

      return anchor.ToMvcHtmlString(TagRenderMode.Normal);
    }
  }
}