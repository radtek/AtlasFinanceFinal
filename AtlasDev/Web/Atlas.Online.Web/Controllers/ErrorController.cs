using Atlas.Online.Web.Models;
using Atlas.Online.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Controllers
{
  public class ErrorController : AppController
  {
    public ActionResult Index()
    {
      var obj = new MessageBoxModel();

      var parts = ErrorMessages.General_Error.Split(new char[] { '|' }, 2);
      if (parts.Length > 0)
      {
        obj.Heading = parts[0];
      }

      if (parts.Length == 2)
      {
        obj.HtmlMessage = new MvcHtmlString(parts[1]);
      }

      obj.HeadingClasses = "fc-beta fs-epsilon";

      obj.Title = "Server error";
      obj.Icon = MessageBoxModel.IconType.Cross;
      obj.ButtonRight = new MessageBoxModel.Button() { Text = "Back Home", Url = Url.Action("Index", "Home") };

      return View("MessageBox", obj);
    }

    //
    // GET: /HttpError/
    public ActionResult NotFound()
    {
      Response.StatusCode = 404;

      var obj = new MessageBoxModel();

      var parts = ErrorMessages.General_404.Split(new char[] { '|' }, 2);
      if (parts.Length > 0)
      {
        obj.Heading = parts[0];
      }

      if (parts.Length == 2)
      {
        obj.HtmlMessage = new MvcHtmlString(parts[1]);
      }

      obj.HeadingClasses = "fc-beta fs-epsilon";

      obj.Title = "404 - Page not found";
      obj.Icon = MessageBoxModel.IconType.Cross;
      obj.ButtonRight = new MessageBoxModel.Button() { Text = "Back Home", Url = Url.Action("Index", "Home") };

      return View("MessageBox", obj);
    }
  }
}
