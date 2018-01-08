using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Falcon.Gyrkin.Library.Security.Attributes;

namespace Falcon.Controllers.Api
{
  public sealed class PayoutController : AppApiController
  {
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetTransactions(long? branchId, DateTime? startRangeActionDate, DateTime? endRangeActionDate, long? payoutId, string idNumber, int? bankId)
    {
      var payouts = Services.WebServiceClient.Payout_GetTransactions(branchId, startRangeActionDate, endRangeActionDate, payoutId, idNumber, bankId);
      return Request.CreateResponse(HttpStatusCode.OK, new { transactions = payouts.Item2, statistics = payouts.Item1 });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetBanks(int getBanks)
    {
      var banks = Services.WebServiceClient.Payout_GetBanks();

      return Request.CreateResponse(HttpStatusCode.OK, new { banks });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetAlerts(int getAlerts)
    {
      var alerts = Services.WebServiceClient.Payout_GetAlerts();

      return Request.CreateResponse(HttpStatusCode.OK, new { alerts });
    }


    [HttpPost]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage PlaceOnHold(long payoutToHold)
    {
      Services.WebServiceClient.Payout_PlaceOnHold(payoutToHold);

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }


    [HttpPost]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage RemoveHoldFromPayout(long payoutToRemoveHold)
    {
      Services.WebServiceClient.Payout_RemoveFromHold(payoutToRemoveHold);

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }
  }
}