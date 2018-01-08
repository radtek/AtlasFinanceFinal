using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Falcon.Areas.Security.Models;
using Falcon.Base;
using Microsoft.Owin.Security;

namespace Falcon.Areas.Security.Controllers
{
  [Authorize]
  public class LockOutController : AppController
  {
    //public UserManager<ApplicationUser> UserManager { get; private set; }

    public LockOutController(/*UserManager<ApplicationUser> userManager*/)
    {
    //  UserManager = userManager;
    }

    //public LockOutController()
    //  : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //{
    //}

    public ActionResult Index()    
    {
      //if (Request.IsAuthenticated)
      //{
      //  ViewBag.UserName = User.Identity.Name;

      //  var Db = new ApplicationDbContext();
      //  var user = Db.Users.First(u => u.UserName == User.Identity.Name);
      //  if (!user.LockoutEnabled)
      //  {
      //    user.LockoutEnabled = true;
      //    Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
      //    Db.SaveChangesAsync();
      //  }
      //}
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Unlock(LockOutViewModel model, string returnUrl)
    {
      //if (ModelState.IsValid)
      //{
      //  var user = await UserManager.FindAsync(model.UserName, model.Password);
      //  if (user != null)
      //  {
      //    //await SignInAsync(user, model.RememberMe);

      //    if (string.IsNullOrEmpty(returnUrl))
      //      return Redirect(Url.Content("~/"));

      //    return RedirectToLocal(returnUrl);
      //  }
      //  else
      //  {
      //    ModelState.AddModelError("", "Invalid username or password.");
      //  }
      //}

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction("Index", "Home");
      }
    }

    private IAuthenticationManager AuthenticationManager
    {
      get
      {
        return HttpContext.GetOwinContext().Authentication;
      }
    }


    //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
    //{
    //  AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
    //  var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
    //  AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
    //}
  }
}