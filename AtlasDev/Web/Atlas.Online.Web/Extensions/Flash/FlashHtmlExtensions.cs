using Atlas.Online.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Atlas.Online.Web.Extensions.Flash
{

  public static class FlashHtmlExtensions
  {
    private static readonly string[] alertTypes = new string[] { "notice", "error", "warning", "success", "info" };

    public static string Flash(FlashStorage flash, string key, string tagName = "div", bool encoded = true, string @class = "")
    {
      var message = flash.Messages.FirstOrDefault(x => x.Key.Equals(key));
      if (message.Key == null)
      {
        return String.Empty;
      }

      return message.Value;
    }

    public static IHtmlString Flash(this HtmlHelper instance, string key, string tagName = "div", bool encoded = true, string @class = "")
    {
      return new MvcHtmlString(Flash(((AppController)instance.ViewContext.Controller).Flash, key, tagName, encoded, @class));
    }

    public static bool HasAlerts(this HtmlHelper instance)
    {
      var flash = ((AppController)instance.ViewContext.Controller).Flash;
      return flash.Messages.Any(x => alertTypes.Any(y => y.Equals(x.Key, StringComparison.OrdinalIgnoreCase)));
    }

    public static IHtmlString Alerts(this HtmlHelper instance, string tagName = "div", bool encoded = true, string classes = "", bool closeButton = false)
    { 
      // Filter out any that are not alerts
      var flash = ((AppController)instance.ViewContext.Controller).Flash;
      var messages = flash.Messages.Where(x => alertTypes.Any(y => y.Equals(x.Key, StringComparison.OrdinalIgnoreCase)));

      return instance.Raw(MessagesToHtml(
        messages, 
        @class: "alert alert-{0} " + classes, 
        tagName: tagName, 
        encoded: encoded, 
        closeButton: closeButton
      ));
    }

    private static string MessagesToHtml(IEnumerable<KeyValuePair<string, string>> messages, string tagName = "div", bool encoded = true, string @class = "{0}", bool closeButton = false)
    {
      var elements = messages.ToList().Select(pair =>
      {
        TagBuilder root = new TagBuilder(tagName);
        root.AddCssClass(String.Format(@class, pair.Key.ToLower()));
        root.SetInnerText(pair.Value);        

        if (closeButton)
        {
          TagBuilder button = new TagBuilder("button");
          button.AddCssClass("close");
          button.Attributes.Add("data-dismiss", "alert");
          button.SetInnerText("x");

          root.InnerHtml += button.ToString();
        }

        return root;
      });

      return string.Join(Environment.NewLine, elements.Select(e => e.ToString()));
    }
  }
}