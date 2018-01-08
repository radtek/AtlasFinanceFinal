using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Atlas.Common.Extensions;
using WebMatrix.WebData;
using Atlas.Online.Web.Filters;
using Atlas.Online.Web.Models;
using Atlas.Online.Web.Extensions.Flash;
using Atlas.Online.Web.Security;
using Atlas.Online.Data.Models.Definitions;
using DevExpress.Xpo;
using Atlas.Online.Web.Mailers;
using Atlas.Online.Web.Common.Helpers;
using Atlas.Online.Web.Resources;
using Atlas.Enumerators;
using Atlas.Online.Web.Validations;

namespace Atlas.Online.Web.Controllers
{
  [Authorize]
  [InitializeSimpleMembership]
  public class AccountController : AppController
  {
    //
    // GET: /Account/Login

    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
      if (User.Identity.IsAuthenticated)
      {
        return RedirectToLocal(returnUrl);
      }

      ViewBag.ReturnUrl = returnUrl;
      return View();
    }

    //
    // POST: /Account/Login

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginModel model, string returnUrl)
    {
      if (ModelState.IsValid && WebSecurity.Login(model.Email, model.Password, persistCookie: model.RememberMe))
      {
        return RedirectToAction("Index", "MyAccount");
      }

      ModelState.AddModelError("", "The user name or password provided is incorrect.");
      return View(model);
    }

    //
    // POST: /Account/LogOff

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
      WebSecurity.Logout();

      return RedirectToAction("Index", "Home");
    }

    //
    // GET: /Account/Register

    [AllowAnonymous]
    [AllowApiActions(typeof(Api.AccountsController))]
    public ActionResult Register()
    {
      if (User.Identity.IsAuthenticated)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { notice = "You are already logged in." });
      }

      return View();
    }

    //
    // POST: /Account/Register

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Register(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        // This is because we are using XPO and EntityFramework
        // This is defintely a hack to simulate db transaction behaviour 
        bool beginTransaction = false;
        try
        {
          // Attempt to register the user
          WebSecurity.CreateUserAndAccount(model.Email, model.Password);
          WebSecurity.Login(model.Email, model.Password);

          // Once we get here, if anything goes wrong we want to "rollback" (delete) user created 
          // by WebSecurity.
          beginTransaction = true;

          using (var uow = XpoHelper.GetNewUnitOfWork())
          {
            // Add Client
            var client = new Client(uow)
            {
              Firstname = model.FirstName,
              Surname = model.Surname,
              UserId = WebSecurity.GetUserId(model.Email),
            };

            // Add Contacts
            client.Contacts.Add(new Contact(uow)
            {
              ContactType = new XPQuery<ContactType>(uow).FirstOrDefault(c => c.ContactTypeId == Convert.ToInt32(General.ContactType.CellNo)),
              Value = UniqueCellAttribute.NormaliseCell(model.Cell)
            });

            client.Contacts.Add(new Contact(uow)
            {
              ContactType = new XPQuery<ContactType>(uow).FirstOrDefault(c => c.ContactTypeId == Convert.ToInt32(General.ContactType.Email)),
              Value = model.Email
            });

            client.Save();

            uow.CommitChanges();

            new AccountMailer().NewRegistration(model.Email, client, model.Password);
          }

          return RedirectToAction("OTP", "Account");
        }
        catch (MembershipCreateUserException e)
        {
          // "Rollback" :/
          if (beginTransaction)
          {
            WebSecurity.Logout();
            Membership.DeleteUser(model.Email, true);
          }

          ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    [AllowAnonymous]
    public ActionResult ForgotPassword()
    {
      return View();
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ForgotPassword(ForgotPasswordModel model)
    {
      if (ModelState.IsValid)
      {
        // Find the user id from the email address
        int userId = WebSecurity.GetUserId(model.Email);
        if (userId < 0)
        {
          ModelState.AddModelError("UserNotFound", Resources.ErrorMessages.ForgottenPassword_UserNotFound);
          return View();
        }

        // Find the client
        Client client = new XPQuery<Client>(Services.XpoUnitOfWork).FirstOrDefault(c => c.UserId == userId);
        // Check ID number
        
        if (client == null) //|| !client.IDNumber.Equals(model.IDNumber.Trim(), StringComparison.OrdinalIgnoreCase))
        {
          ModelState.AddModelError("UserNotFound", Resources.ErrorMessages.ForgottenPassword_UserNotFound);
          return View();
        }
        
        // Token which expires after 12 hours
        var tokenUrl = Url.Action("ResetPassword", "Account", new
        { 
          key = WebSecurity.GeneratePasswordResetToken(model.Email, 60 * 12),
          hash = MD5.Digest(userId.ToString())
        }, Request.Url.Scheme);
        
        new AccountMailer().ForgotPassword(model.Email, client, tokenUrl);

        return RedirectToAction("Index", "Message", new { t = MessageBoxModel.MessageType.PasswordReset_Sent });
      }

      return View(model);
    }

    [AllowAnonymous]
    public ActionResult ResetPassword(string key, string hash)
    {
      if (key == null || hash == null)
      {
        return RedirectToAction("Index", "Home");
      }

      UsersContext db = new UsersContext();
      var membership = db.WebPagesMembership.FirstOrDefault(x => x.PasswordVerificationToken == key);

      if (membership == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { error = "Invalid password reset token." });
      }

      // Check hash
      if (hash != MD5.Digest(membership.UserId.ToString()))
      {
        return RedirectToAction("Index", "Home").WithFlash(new { error = "Invalid password reset hash." });
      }

      ViewBag.token = key;

      return View(new ResetPasswordModel() { Token = key });
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ResetPassword(ResetPasswordModel model)
    {
      if (ModelState.IsValid)
      {
        if (!WebSecurity.ResetPassword(model.Token, model.Password))
        {
          ModelState.AddModelError("PasswordResetToken", Resources.ErrorMessages.PasswordReset_ResetError);
          return View();
        }

        return RedirectToAction("Index", "Message", new
        {
          t = MessageBoxModel.MessageType.PasswordReset_Success
        });
      }

      return View(model);
    }

    [HttpGet]
    [Authorize]
    [AllowApiActions(typeof(Api.OtpController))]
    public ActionResult OTP(string next = "")
    {
      if (CurrentClient == null)
      {
        return RedirectToAction("Index", "Home");
      }

      if (CurrentClient.OTPVerified)
      {
        // OTP already verified, redirect to application process.
        return RedirectToAction("Index", "Application");
      }

      Contact contact = CurrentClient.Contacts.FirstOrDefault(o => o.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt());
      if (contact == null)
      {
        return RedirectToAction("Index", "MyAccount").WithFlash(new { error = "You do not have a mobile phone number associated with your account." });
      }

      ViewBag.CellNo = contact.Value;

      if (!String.IsNullOrEmpty(next) && Url.IsLocalUrl(next))
      {
        ViewBag.NextUrl = next;
      }
      else
      {
        ViewBag.NextUrl = Url.Action("Index", "Application");
      }

      return View();
    }

    #region Helpers
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

    public enum ManageMessageId
    {
      ChangePasswordSuccess,
      SetPasswordSuccess,
      RemoveLoginSuccess,
    }

    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "That email address is already in use. Please enter a different email address or log in to your existing account.";

        case MembershipCreateStatus.DuplicateEmail:
          return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
    #endregion
  }
}
