using System.Net.Http;
using System.Web.Http;


namespace Falcon.Controllers.Api
{
  public sealed class AuthenticateRequestController : AppApiController
  {

    public sealed class AuthenticateModal
    {
      public string RequestId { get; set; }
      public string Token { get; set; }
      public string RequestedUser { get; set; }
      public string Module { get; set; }
      public string ModuleAction { get; set; }
    }


    [HttpPost]
    public HttpResponseMessage Verify([FromBody]AuthenticateModal value)
    {


      return null;
    }
  }
}