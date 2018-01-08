using System.Net.Http;
using System.Web.Http;


namespace Falcon.Controllers.Api
{
  // [AllowAnonymous]
  // [ValidateHttpAntiForgeryToken]
  [Authorize]
  public sealed class AccountVerificationAnalyticsResourceController : AppApiController
  {
    public struct Total
    {
      public int Count { get; set; }

      public Total(int t) : this()
      {
        Count = t;
      }
    }
    [HttpGet]
    public HttpResponseMessage GetTransactionTotal()
    {
      return null;
      //return Request.CreateResponse(HttpStatusCode.OK, new Total(new XPQuery<AVS_Transaction>(Services.UnitOfwork).Count(p => p.CreateDate.Date >= DateTime.Now.Date)));
    }
  }
}