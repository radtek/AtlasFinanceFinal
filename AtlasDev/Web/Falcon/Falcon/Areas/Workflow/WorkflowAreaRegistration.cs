using System.Web.Mvc;

namespace Falcon.Areas.Workflow
{
  public class WorkflowAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Workflow";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "AccountManagement",
          "Workflow/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}