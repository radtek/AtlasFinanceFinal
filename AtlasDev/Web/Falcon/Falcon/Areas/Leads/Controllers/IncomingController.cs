using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Leads.Controllers
{
  [Authorize]
  public class IncomingController : AppController
  {
    public IncomingController()
    {
    }

    public ActionResult Index()
    {

      return View();
    }

  }
}