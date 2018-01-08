using System.Web.Mvc;

namespace Falcon.Areas.Payout.Controllers
{
  public class DashboardController : Controller
  {
    //
    // GET: /Payout/Dashboard/
    public ActionResult Index()
    {
      return View();
    }

    //
    // GET: /Payout/Dashboard/Details/5
    public ActionResult Details(int id)
    {
      return View();
    }
  }
}