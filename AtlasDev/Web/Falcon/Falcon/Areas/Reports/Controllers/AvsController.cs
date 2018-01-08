using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Reports.Controllers
{
  [Authorize]
  public class AvsController : AppController
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Avs()
    {
      return View();
    }
  }
}