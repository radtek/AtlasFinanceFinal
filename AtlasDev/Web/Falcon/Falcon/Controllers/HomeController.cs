using System.Web;
using System.Web.Mvc;
using Falcon.Base;
using Falcon.Gyrkin.Library.Attributes;

namespace Falcon.Controllers
{
  [AllowAnonymous]
  [Compress]
  public sealed class HomeController : AppController
  {
    //public UserManager<ApplicationUser> UserManager { get; private set; }

    //public HomeController()
    //  : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //{
    //  this.Init(); 
    //}

    public HomeController(/*UserManager<ApplicationUser> userManager*/) : base()
    {
      //UserManager = userManager;
    }


    public ActionResult Index()
    {
      var httpContext = HttpContext.ApplicationInstance.Context;

      if (!User.Identity.IsAuthenticated)
      {

#if (DEBUG)
        return View("~/Areas/User/Views/Account/Login.cshtml");
#else 
        return RedirectToAction("Login", "Account", new { area = "User" });
#endif
      }

      return View();
    
//#if (DEBUG)
//      return View("~/Areas/Dashboard/Views/Analytics/Index.cshtml");
//#else 
//        return RedirectToAction("Index", "Analytics", new { area = "Dashboard" });
//#endif

    }
  }
}