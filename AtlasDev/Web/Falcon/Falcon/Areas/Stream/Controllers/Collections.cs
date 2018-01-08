using System.Security.Claims;
using System.Web.Mvc;
using Falcon.Base;
using Falcon.Gyrkin.Library.Attributes;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;

namespace Falcon.Areas.Stream.Controllers
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR, RoleType.STREAM_COLLECTIONS})]
  [Compress]
  public class CollectionsController : AppController
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}