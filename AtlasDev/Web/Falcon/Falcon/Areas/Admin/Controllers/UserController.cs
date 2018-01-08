using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Admin.Controllers
{
  [Authorize]
  public class UserController : AppController
  {
    //public UserManager<ApplicationUser> UserManager { get; private set; }

    public UserController(/*UserManager<ApplicationUser> userManager*/)
    {
      //UserManager = userManager;
    }

    //public UserController()
    //  : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //{
    //}

    public ActionResult Index()
    {
      return View();
    }
  }
}