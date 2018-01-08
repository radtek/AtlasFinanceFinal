using System.Net;
using System.Net.Http;
using System.Web.Http;
using Falcon.Gyrkin.Library.Security.Attributes;

namespace Falcon.Controllers.Api
{
  public class ForgotModel
  {
    public string Username { get; set; }
    public string IdNo { get; set; }
    public string CellNo { get; set; }
  }

  [AllowAnonymous]
  public sealed class ForgotController : AppApiController
  {
    //public UserManager<ApplicationUser> UserManager { get; private set; }

    //public ForgotController()
    //  : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //{
    //}


    public ForgotController(/*UserManager<ApplicationUser> userManager*/)
    {
      //UserManager = userManager;
    }


    [HttpPost]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage Post(ForgotModel value)
    {
      if (string.IsNullOrEmpty(value.IdNo))
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Identity No. is required");

      if (string.IsNullOrEmpty(value.CellNo))
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Cell No. is required");

      if (string.IsNullOrEmpty(value.Username))
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Username is required");

      var key = Services.WebServiceClient.Person_VerifyIsValid(value.IdNo, value.CellNo);

      if (key == null)
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Password reset failed, field missmatch.");

      //var user = Services.UnitOfwork.Query<AspNetUsers>().FirstOrDefault(p => p.UserName == value.Username);

      //if (user == null)
      //  return Request.CreateResponse(HttpStatusCode.BadRequest, "Username does not exist in the datastore.");

      //if (user.PersonId != key)
      //  return Request.CreateResponse(HttpStatusCode.BadRequest, "Sequence of inputs do not match.");

      var result = Services.WebServiceClient.OTP_Send(value.CellNo);

      var hash = Services.WebServiceClient.OTP_StoreToHash(string.Format("{0}:{1}:{2}", result.m_Item1,result.m_Item2, value.Username), value.CellNo);

      return Request.CreateResponse(HttpStatusCode.OK, new { _ = hash });
    }
  }
}