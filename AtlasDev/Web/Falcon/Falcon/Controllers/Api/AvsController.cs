using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Falcon.Gyrkin.Library.Attributes;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Service;
using Newtonsoft.Json;


namespace Falcon.Controllers.Api
{
  [Authorize]
  public sealed class AvsController : AppApiController
  {
    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    [Compress]
    [ETagMvc]
    public HttpResponseMessage GetTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId)
    {
      var response = Services.WebServiceClient.AVS_GetTransactions(branchId, startDate, endDate, transactionId, idNumber, bankId);

       return Request.CreateResponse(HttpStatusCode.OK, new { transactions = response.Item2, statistics = response.Item1 });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    [Compress]
    [ETagMvc]
    public HttpResponseMessage GetBanks(int getBanks)
    {
      var banks = Services.WebServiceClient.AVS_GetBanks();

      return Request.CreateResponse(HttpStatusCode.OK, new { banks });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    [Compress]
    [ETagMvc]
    public HttpResponseMessage GetServiceSchedules(int getServiceSchedules)
    {
      var serviceScheduleBanks = Services.WebServiceClient.AVS_GetServiceSchedules();

      return Request.CreateResponse(HttpStatusCode.OK, new { services = serviceScheduleBanks.Keys.ToList(), serviceBanks = serviceScheduleBanks.SelectMany(t => t.Value).ToList() });
    }


    [HttpPost]
    [WebApiAntiForgeryToken]
    [Compress]
    [ETagMvc]
    public HttpResponseMessage Resend(long resendTransactionId)
    {
      Services.WebServiceClient.AVS_ResendTransactions(new List<long>() { resendTransactionId });

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }


    [HttpPost]
    [WebApiAntiForgeryToken]
    [Compress]
    [ETagMvc]
    public HttpResponseMessage Cancel(long cancelTransactionId)
    {
      Services.WebServiceClient.AVS_CancelTransactions(new List<long>() { cancelTransactionId });

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }


    [HttpPost]
    [WebApiAntiForgeryToken]
    [Compress]
    [ETagMvc]
    public HttpResponseMessage SaveServiceSettings(string servicesString, string serviceBanksString)
    {
      var services = JsonConvert.DeserializeObject<List<AvsService>>(servicesString);
      var serviceBanks = JsonConvert.DeserializeObject<List<AvsServiceBank>>(serviceBanksString);

      var serviceSchedules = new Dictionary<AvsService, List<AvsServiceBank>>();
      foreach (var service in services)
      {
        serviceSchedules.Add(service, serviceBanks.Where(s => s.ServiceId == service.ServiceId).ToList());
      }

      Services.WebServiceClient.AVS_UpdateServicesSchedules(serviceSchedules);

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }
  }
}