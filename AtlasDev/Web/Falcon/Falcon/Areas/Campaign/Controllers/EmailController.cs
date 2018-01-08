using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Campaign.Controllers
{
  [Authorize]
  public class EmailController : AppController
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}