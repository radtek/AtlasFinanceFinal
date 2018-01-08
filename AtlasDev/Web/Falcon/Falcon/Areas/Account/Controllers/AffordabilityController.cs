using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Account.Controllers
{
  [Authorize]
  public sealed class AffordabilityController : AppController
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