using System.IO;
using System.Web.Mvc;
using Falcon.Services;

namespace Falcon.Base
{
  //[InitializeSimpleMembership]
  public abstract class AppController : Controller
  {
    private SharedServices services = null;

    public AppController()
    {
      this.Init();
    }
    public void Init()
    {
      //var userCommon = new UserCommon();
      //ViewBag.PersonId = userCommon.GetPersonId();
      //ViewBag.BranchId = userCommon.GetBranchId();
    }

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

    public string ActionName
    {
      get
      {
        return ControllerContext.RouteData.Values["action"] as string;
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