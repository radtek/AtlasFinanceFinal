using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Leads.Controllers
{
  [Authorize]
  public class ProcessingController : AppController
  {
    public ProcessingController()
    {
    }

    public ActionResult Index()
    {

      return View();
    }

  }
}