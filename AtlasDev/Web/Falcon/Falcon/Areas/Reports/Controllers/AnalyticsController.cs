using System.Web.Mvc;

namespace Falcon.Areas.Reports.Controllers
{
  [Authorize]
  public sealed class AnalyticsController : Controller
  {
    //
    // GET: /Dashboard/Analytics/
    public ActionResult Index()
    {
      return View();
    }
  }
}