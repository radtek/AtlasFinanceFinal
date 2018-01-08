using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Security.Controllers
{
  public sealed class AccessController : AppController
  {
    public ActionResult Denied()
    {
      return View();
    }
  }
}