using System.Web.Mvc;

namespace Falcon.Areas.Common.Controllers
{
  public class AuthenticateRequestController : Controller
  {
    // GET: Common/AuthenticateRequest
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Authenticate()
    {

      return View();
    }
  }
}