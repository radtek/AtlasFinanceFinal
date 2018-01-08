using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using Falcon.Base;
using Falcon.Gyrkin.Library.Security;
using Falcon.Models;
using Falcon.Services;
using Microsoft.Owin.Security.Cookies;
using NHibernate;

namespace Falcon.Areas.User.Controllers
{
  [Authorize]
  public class AccountController : AppController
  {
    private readonly AuthenticationService<NhUserAccount> _authenticationService;
    private readonly ISession _session;
    private readonly UserAccountService<NhUserAccount> _userAccountService;


    public AccountController(
            AuthenticationService<NhUserAccount> authenticationService,
            ISession session,
            UserAccountService<NhUserAccount> userAccountService)
    {
      _authenticationService = authenticationService;
      _session = session;
      _userAccountService = userAccountService;

      // //Uncomment to make yourself a username
      //NhUserAccount account;
      //using (var tx = this.session.BeginTransaction())
      //{
      //  account = this.userAccountService.CreateAccount("jackh", "jackh12345", "jack@atcorp.co.za");


      //  tx.Commit();
      //}
      //using (var tx = this.session.BeginTransaction())
      //{
      //  this.userAccountService.AddClaim(account.ID, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Administrator");
      //  this.userAccountService.UpdateClaims(account.ID);
      //  tx.Commit();
      //}
    }
    
    // GET: /Account/Login
    [AllowAnonymous]
    public async Task<ActionResult> Login(LoginModel model, string returnUrl)
    {
      if (ModelState.IsValid)
      {
        NhUserAccount account;
        using (var tx = _session.GetSession(EntityMode.Poco))
        {
          var cookie = new HttpCookie("fl_user_id") { Expires = DateTime.Now.AddDays(-30) };
          HttpContext.Response.Cookies.Add(cookie);

          cookie = new HttpCookie("fl_branch_id") { Expires = DateTime.Now.AddDays(-30) };
          HttpContext.Response.Cookies.Add(cookie);

          cookie = new HttpCookie("fl_branch_name") { Expires = DateTime.Now.AddDays(-30) };
          HttpContext.Response.Cookies.Add(cookie);

          if (_userAccountService.AuthenticateWithUsernameOrEmail(model.UserName, model.Password, out account))
          {
            _authenticationService.SignIn(account, model.RememberMe);

            if (_userAccountService.IsPasswordExpired(account))
            {
              return RedirectToAction("ChangePassword", "Account");
            }
            else
            {
              // Add userId to cookie
              cookie = new HttpCookie("fl_user_id");
              cookie.Value = account.ID.ToString();
              cookie.Expires = DateTime.Now.AddDays(+30);
              HttpContext.Response.Cookies.Add(cookie);

              // check if user linked.              
              // Store some basic info in koekies.
              var userDetails = await WebApiClient.GetUserDetails(account.ID);
              if (userDetails != null)
              {
                cookie = new HttpCookie("fl_branch_id");
                cookie.Value = userDetails["Branch"]["BranchId"].ToString();
                cookie.Expires = DateTime.Now.AddDays(+30);
                HttpContext.Response.Cookies.Add(cookie);

                cookie = new HttpCookie("fl_branch_name");
                cookie.Value = userDetails["Branch"]["Name"].ToString();
                cookie.Expires = DateTime.Now.AddDays(+30);
                HttpContext.Response.Cookies.Add(cookie);

                cookie = new HttpCookie("fl_legacy_client_code");
                cookie.Value = userDetails["LegacyClientCode"].ToString();
                cookie.Expires = DateTime.Now.AddDays(+30);
                HttpContext.Response.Cookies.Add(cookie);
              }

              return
                RedirectToLocal(string.IsNullOrEmpty(returnUrl)
                  ? "/"
                  : returnUrl.ToLower().Contains("login") ? "/" : returnUrl);
            }
          }
          else
          {
            ModelState.AddModelError(string.Empty, "Invalid Username or Password");
          }
        }
      }
      return View(model);
    }


    public async Task<ActionResult> Associate(AssociateModel model)
    {
      return RedirectToLocal("/");

      if (ModelState.IsValid)
      {
        var id = User.GetClaimValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

        
        var result = await WebApiClient.LinkUser(model.IdNumber, Guid.Parse(id));
        if (result)
          return RedirectToLocal("/");
        else
        {
          ModelState.AddModelError("ERROR_LINKING_USER", "Error attempting to link user.");
        }

      }
      return View(model);
    }
   
   
    public ActionResult LogOut()
    {

      _authenticationService.SignOut();
      FormsAuthentication.SignOut();

      var cookie = new HttpCookie("fl_user_id") { Expires = DateTime.Now.AddDays(-30) };
      HttpContext.Response.Cookies.Add(cookie);

      cookie = new HttpCookie("fl_branch_id") { Expires = DateTime.Now.AddDays(-30) };
      HttpContext.Response.Cookies.Add(cookie);

      cookie = new HttpCookie("fl_branch_name") { Expires = DateTime.Now.AddDays(-30) };
      HttpContext.Response.Cookies.Add(cookie);


      return Redirect("/");
    }

    [AllowAnonymous]
    public ActionResult ForgotPassword()
    {
      return View();
    }

    public ActionResult ChangePassword()
    {
      return View();
    }

    public async Task<ActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
    {
      // validate current password
      var userCookie = HttpContext.Request.Cookies.Get("fl_user_id");
      if (userCookie != null)
      {
        try
        {
          var userId = new Guid(userCookie.Value);
          _userAccountService.ChangePassword(userId, resetPasswordModel.CurrentPassword, resetPasswordModel.NewPassword);
        }
        catch (Exception exception)
        {
          return RedirectToLocal("/");
          //return Content(exception.Message);
        }
      }
      else
      {
        //TODO: throw error or return an error back to caller
      }

      // validate new password and confirmation password

      // fire change password call to db

      // if everything is successful, redirect to home page, else return error

      return RedirectToLocal("/");
    }

    [AllowAnonymous]
    public ActionResult Password()
    {
      return View();
    }

    //
    // GET: /Account/Profile/
    public new ActionResult Profile()
    {
      return View();
    }


    public ActionResult Settings()
    {
      return View();
    }

    //
    // GET: /Account/Register
    [AllowAnonymous]
    public ActionResult Register()
    {
      return View();
    }

    [AllowAnonymous]
    public ActionResult OTP()
    {
      //ViewBag.UserId = new Guid(User.Identity.GetUserId().Trim());
      return View();
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
      return Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToLocal("/");
    }
  }
}