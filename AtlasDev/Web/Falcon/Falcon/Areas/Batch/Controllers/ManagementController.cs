using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Batch.Controllers
{
  [Authorize]
  public sealed class ManagementController : AppController
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