using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Admin.Controllers
{
  [Authorize]
  public class BranchController : AppController
  {
    public ActionResult Index()
    {
      return View();
    }
  }
}