using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Filters;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using Atlas.Online.Web.Extensions.Flash;
using Atlas.Online.Web.Helpers;
using System.IO;

namespace Atlas.Online.Web.Controllers
{
  [InitializeSimpleMembership]
  public abstract class AppController : Controller
  {
    private SharedServices services = null;
    private FlashStorage _flash = null;

    public SharedServices Services
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

    public Client CurrentClient
    {
      get { return Services.CurrentClient; }
    }

    public string ActionName
    {
      get
      {
        return ControllerContext.RouteData.Values["action"] as string;
      }
    }

    public FlashStorage Flash
    {
      get
      {
        if (_flash != null)
        {
          return _flash;
        }

        return _flash = new FlashStorage(TempData);
      }
    }

    public string RenderRazorViewToString(string viewName, object model = null)
    {
      ViewData.Model = model;
      using (var sw = new StringWriter())
      {
        var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
        var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
        viewResult.View.Render(viewContext, sw);
        viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
        return sw.GetStringBuilder().ToString();
      }
    }
  }
}
