using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Batch.Controllers
{
  [Authorize]
  public sealed class JobController : AppController
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