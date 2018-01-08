using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Falcon.Controllers.Api
{
  public sealed class CellController : AppApiController
  {
    //public UserManager<ApplicationUser> UserManager { get; private set; }

    //public CellController()
    //  : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
    //{
    //}

    public CellController(/*UserManager<ApplicationUser> userManager*/)
    {
      //UserManager = userManager;
    }


    [HttpPost]
    public HttpResponseMessage Verify([FromBody]dynamic value)
    {
      //string cellNo = value.value;
      //var cell = Services.UnitOfwork.Query<AspNetUsers>().Any(p => p.CellNo.Trim() == cellNo.Trim());

      //return Request.CreateResponse(HttpStatusCode.OK, new { isValid = cell, Value = cellNo });
      return Request.CreateResponse(HttpStatusCode.OK, "");
    }
  }
}