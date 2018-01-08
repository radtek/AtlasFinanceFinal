using System.Security.Claims;
using System.Web.Mvc;
using Falcon.Common;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;

namespace Falcon.Areas.Reports.Controllers
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR, RoleType.CI_REPORT })]
  public class CIReportController : Controller
  {
    // GET: Dashboard/CIReport
    public ActionResult Index()
    {
      ViewBag.PersonId = new UserCommon().GetPersonId();
      return View();
    }

    public ActionResult NewCiReport()
    {
      ViewBag.PersonId = new UserCommon().GetPersonId();
      return View();
    }
  }
}