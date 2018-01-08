using System.Security.Claims;
using System.Web.Mvc;
using Falcon.Base;
using Falcon.Gyrkin.Library.Attributes;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;

namespace Falcon.Areas.Avs.Controllers
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR, RoleType.AVS_ADMIN })]
  [Compress]
  public class AdminController : AppController
  {
    //
    // GET: /Avs/Admin/
    public ActionResult Index()
    {
      return View();
    }
  }
}