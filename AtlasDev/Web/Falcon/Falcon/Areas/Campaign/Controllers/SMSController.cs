using System.Security.Claims;
using System.Web.Mvc;
using Falcon.Base;
using Falcon.Gyrkin.Library.Attributes;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;

namespace Falcon.Areas.Campaign.Controllers
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR, RoleType.CAMPAIGN_MANAGER, RoleType.CAMPAIGN_MANAGER_SMS })]
  [Compress]
  public class SMSController : AppController
  {  
    public ActionResult Index()
    {
      return View();
    }
  }
}