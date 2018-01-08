using System.Web.Mvc;

namespace Falcon.Areas.Campaign
{
  public class CampaignAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Campaign";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "Campaign",
          "Campaign/{controller}/Manager/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}