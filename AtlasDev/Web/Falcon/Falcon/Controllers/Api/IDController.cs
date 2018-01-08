using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Falcon.Controllers.Api
{
  public sealed class IDController : AppApiController
  {
    //public UserManager<ApplicationUser> UserManager { get; private set; }

    //public IDController()
    //  : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //{
    //}

    public IDController(/*UserManager<ApplicationUser> userManager*/)
    {
    //  UserManager = userManager;
    }


    [HttpPost]
    public HttpResponseMessage Verify([FromBody]dynamic value)
    {
      string IdNo = value.value;

      var found = Services.WebServiceClient.Person_LocateByIdentityNo(IdNo);

      return Request.CreateResponse(HttpStatusCode.OK, new { isValid = found, Value = IdNo });
    }
  }
}