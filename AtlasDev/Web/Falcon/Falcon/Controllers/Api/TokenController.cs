using System.Net.Http;
using System.Web.Http;

namespace Falcon.Controllers.Api
{
  public sealed class TokenController : AppApiController
  {

    public sealed class TokenModal
    {
      public string RequestId { get; set; }
      public string UserId { get; set; }
      public string Module { get; set; }
      public string ModuleAction { get; set; }
    }


    [HttpPost]
    public HttpResponseMessage Post([FromBody]TokenModal value)
    {


      return null;
    }
  }
}