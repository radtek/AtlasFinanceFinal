using System.Security.Claims;
using System.Web.Mvc;
using Falcon.Common;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;

namespace Falcon.Areas.Reports.Controllers
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR, RoleType.STREAM_REPORT })]
  public class StreamController : Controller
  {
    // GET: Reports/Stream
    public ActionResult Index()
    {
      ViewBag.PersonId = new UserCommon().GetPersonId();
      return View();
    }

    public ActionResult Detail()
    {
      ViewBag.PersonId = new UserCommon().GetPersonId();
      return View();
    }
  }
}