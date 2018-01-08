using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Falcon.Controllers.Api
{
  public sealed class UsernameController : AppApiController
  {
    //public UserManager<ApplicationUser> UserManager { get; private set; }

    //public UsernameController()
    //  : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //{
    //}

    public UsernameController(/*UserManager<ApplicationUser> userManager*/)
    {
      //UserManager = userManager;
    }


    [HttpPost]
    public HttpResponseMessage Verify([FromBody]dynamic value)
    {
    //  try
      //{
      //  string username = value.value;
      //  var aspnetUser = Services.UnitOfwork.Query<AspNetUsers>().FirstOrDefault(p => p.UserName == username);

      //  if (aspnetUser != null)
      //    return Request.CreateResponse(HttpStatusCode.OK, new { isValid = true, Value = username });
      //  else
      //    return Request.CreateResponse(HttpStatusCode.OK, new { isValid = false, Value = username });
      //}
      //catch (Exception)
      //{
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "An error has occurred during username discovery.");
      //}
    }
  }
}