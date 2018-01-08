using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Owin.Security;

using Newtonsoft.Json;

using Falcon.Gyrkin.Library.Common;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Service;


namespace Falcon.Controllers.Api
{
  public sealed class AccountController : AppApiController
  {
   // public UserManager<ApplicationUser> UserManager { get; private set; }

    //public AccountController()
    //  : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //{
    //}

    public AccountController(/*UserManager<ApplicationUser> userManager*/)
    {
      //UserManager = userManager;
    }

    [HttpGet]
    public HttpResponseMessage OTP(int OTP)
    {
      //var user = UserManager.FindByName(User.Identity.Name);
      //if(user != null)
      //{
      //  var otp = new XPQuery<OTP>(Services.UnitOfwork).FirstOrDefault(p => p.Code == OTP && p.User == new Guid(user.Id));

      //  if (otp == null)
      //    return Request.CreateResponse(HttpStatusCode.BadRequest, "Please try again");
      //}

      return Request.CreateResponse(HttpStatusCode.OK, "Verified");
    }

    [HttpPost]
    public async Task<HttpResponseMessage> ResendOTP(Guid user)
    {
      //var userObj = await UserManager.FindByIdAsync(user.ToString());

      //if (userObj != null)
      //{

      //  // ********** ADDD TO SERVICE !@#!:#K!L@#:!K@#L!:@#!
      //  //var result = new OTPSender().Send(user, userObj.CellNo);

      //  //if (result != null)
      //  //{
      //  //  var otpCode = new OTP(Services.UnitOfwork);
      //  //  otpCode.User = user;
      //  //  otpCode.Code = result.Item1;
      //  //  otpCode.Hash = result.Item2;
      //  //  otpCode.CreateDate = DateTime.Now;
      //  //  Services.UnitOfwork.CommitChanges();

      //    return Request.CreateResponse(HttpStatusCode.OK, "Resent");
      //  }
      //  else
      //  {
      //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "OTP Resend Error");
      //  }
      
      return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "User not found");
    }

    [HttpPost]
    [AllowAnonymous]
    [WebApiAntiForgeryToken]
    //[Throttle(Name = "LoginThrottle", Message = "You must wait {0} seconds before accessing this url again.", Seconds = 20)]
    public async Task<HttpResponseMessage> Login(string username, string password, string returnUrl, bool? remember) //LoginViewModel model, string returnUrl
    {
      //try
      //{
      //  var user = await UserManager.FindAsync(username, password);
      //  if (user != null)
      //  {
      //    Person personData = await Services.WebServiceClient.Operations_LocateByPersonIdAsync((long)user.PersonId);
      //    personData.UserId = user.Id;

      //    SetUpInitialCookies(personData);

      //    await SignInAsync(user, remember != null ? (bool)remember : false);

      //    return Request.CreateResponse(HttpStatusCode.OK, new { returnUrl = returnUrl, id = user.Id });
      //  }
      //  else
      //  {
      //    return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Login Failed");
      //  }
      //}
      //catch (Exception ex)
      //{
      //  return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
      //}
      return Request.CreateResponse(HttpStatusCode.OK, "Registered");
    }


    [HttpPost]
    [AllowAnonymous]
    [WebApiAntiForgeryToken]
    //[Throttle(Name = "LoginThrottle", Message = "You must wait {0} seconds before accessing this url again.", Seconds = 20)]
    public async Task<HttpResponseMessage> Register(string username, string password, string cellNo, string idNo)
    {
      //Person personData = null;

      //personData = Services.WebServiceClient.Operations_LocatebyId(idNo);

      //if (personData == null)
      //  return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Unable to locate person");

      //var user = new ApplicationUser() { UserName = username };
      //var result = await UserManager.CreateAsync(user, password);

      //if (result.Succeeded)
      //{
      //  var Db = new ApplicationDbContext();
      //  user.PersonId = personData.PersonId;
      //  user.IsAssociated = true;
      //  user.CellNo = cellNo;
      //  Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
      //  await Db.SaveChangesAsync();

      //  personData.UserId = user.Id;

      //  // ********** ADDD TO SERVICE !@#!:#K!L@#:!K@#L!:@#!
      //  //var otpResult = new OTPSender().Send(new Guid(user.Id), cellNo);

      //  //if (result != null)
      //  //{
      //  //  var otpCode = new OTP(Services.UnitOfwork);
      //  //  otpCode.User = new Guid(user.Id);
      //  //  otpCode.Code = otpResult.Item1;
      //  //  otpCode.Hash = otpResult.Item2;
      //  //  otpCode.CreateDate = DateTime.Now;
      //  //  Services.UnitOfwork.CommitChanges();
      //  //}

        return Request.CreateResponse(HttpStatusCode.OK, "Registered");
      //}
      //else
      //{
      //  return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Unable to register");
      //}
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage LocateById(string idNo)
    {
      //Person personData = null;
      //Services.WebServiceClient.Using(cli =>
      //{
      //  personData = cli.Operations_LocatebyId(idNo);
      //});

      //if (personData == null)
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Person not found.");
      //else
      //  return Request.CreateResponse(HttpStatusCode.Found, new { personData });
    }

    public HttpResponseMessage LogOff()
    {
      CookieHelper.CleanCookies(HttpContext.Current);

      AuthenticationManager.SignOut();      
      return Request.CreateResponse(HttpStatusCode.OK, "Logged Out");
    }

    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager
    {
      get
      {
        return HttpContext.Current.GetOwinContext().Authentication;
      }
    }

    //private async Task SignInAsync(ApplicationUser user, bool isPersistent)
    //{
    //  AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
    //  var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
    //  AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
    //}

    private void SetUpInitialCookies(Person data)
    {
      var jsonSerial = JsonConvert.SerializeObject(data);

      if (string.IsNullOrEmpty(CookieHelper.SecureGetCookieValue(HttpContext.Current, CookieConst.FALCON_PERSON_DATA)))
        CookieHelper.SecureStore(HttpContext.Current, CookieConst.FALCON_PERSON_DATA, jsonSerial, DateTime.Now.AddDays(+30), (string v) => { });
    }
  }
}