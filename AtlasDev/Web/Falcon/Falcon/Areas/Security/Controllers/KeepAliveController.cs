using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Security.Controllers
{
  [Authorize]
  public class KeepAliveController : AppController
  {
    public KeepAliveController()     
    {
    }

    public ActionResult Index()    
    {      
      return View();
    }    
  }
}