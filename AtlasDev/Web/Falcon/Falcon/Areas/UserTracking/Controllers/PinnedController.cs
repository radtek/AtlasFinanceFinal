using System.Security.Claims;
using System.Web.Mvc;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;

namespace Falcon.Areas.UserTracking.Controllers
{
   [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR, RoleType.USER_TRACKING_PINNED })]
  public class PinnedController : Controller
  {
    // GET: UserTracking/Pinned
    public ActionResult Index()
    {
      return View();
    }
  }
}