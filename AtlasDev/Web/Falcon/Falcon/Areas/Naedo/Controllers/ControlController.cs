using System.Security.Claims;
using System.Web.Mvc;
using Falcon.Base;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;

namespace Falcon.Areas.Naedo.Controllers
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR})]
  public class ControlController : AppController
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}