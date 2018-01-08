using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.FingerPrint.Controllers
{
  [Authorize]
  public sealed class VerifyController : AppController
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}