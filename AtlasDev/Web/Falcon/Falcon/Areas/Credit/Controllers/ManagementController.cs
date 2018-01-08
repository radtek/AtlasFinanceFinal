using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Credit.Controllers
{
  [Authorize]
  public class ManagementController : AppController
  {
    public ActionResult Index()
    {
      return View();
    }

    //[Authorize]
    //public ActionResult Avs()
    //{
    //  return View();
    //}
  }
}