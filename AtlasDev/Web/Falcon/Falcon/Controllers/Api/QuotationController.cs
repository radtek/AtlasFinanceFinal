using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Falcon.Gyrkin.Library.Security.Attributes;

namespace Falcon.Controllers.Api
{
  public sealed class QuotationController : AppApiController
  {
    public QuotationController() { }

    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> AcceptQuotation(int acceptQuotation, long accountId, long quotationId)
    {
      try
      {
        await Services.WebServiceClient.Account_QuotationAcceptAsync(accountId, quotationId);
        return Request.CreateResponse(HttpStatusCode.OK, new { });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to Accept quotation.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> RejectQuotation(int rejectQuotation, long accountId, long quotationId)
    {
      try
      {
        await Services.WebServiceClient.Account_QuotationRejectAsync(accountId, quotationId);
        return Request.CreateResponse(HttpStatusCode.OK, new { });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to Reject Quotation.");
      }
    }
  }
}