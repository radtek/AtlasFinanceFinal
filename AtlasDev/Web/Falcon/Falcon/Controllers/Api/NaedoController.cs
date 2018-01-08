
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Service;
using Newtonsoft.Json;

namespace Falcon.Controllers.Api
{
 [AllowAnonymous]
  // [ValidateHttpAntiForgeryToken]
  public sealed class NaedoController : AppApiController
  {

    //[HttpGet]
    //public HttpResponseMessage GetControl(General.Host? host, long? branchId, DateTime? startDate, DateTime? endDate, bool controlOnly)
    //{
    //  var response = Services.WebServiceClient.Naedo_GetDebitOrders(host, branchId, startDate, endDate, false);

    //  return Request.CreateResponse(HttpStatusCode.OK, new { response });
    //}

    // [HttpPost]
    ////[WebApiAntiForgeryToken]
    //public HttpResponseMessage CancelAdditionalDebitOrder(long controlId, long transactionId)
    //{
    //  var response = Services.WebServiceClient.Naedo_CancelAdditionalDebitOrder(controlId, transactionId);

    //  return Request.CreateResponse(HttpStatusCode.OK, new { response });
    //}

    [HttpPost]
    public HttpResponseMessage StopDebitOrder(long controlId, bool cancelTransactions)
    {
      var response = Services.WebServiceClient.Naedo_StopDebitOrder(controlId, cancelTransactions);

      return Request.CreateResponse(HttpStatusCode.OK, new { response });
    }


    [HttpPost]
    public HttpResponseMessage UnbatchRejectedTransactions(int unbatchRejectedTransactions, long batchId)
    {
      var successfullyUnbatched = Services.WebServiceClient.Naedo_UnbatchRejectedTransactions(batchId);

      return Request.CreateResponse(HttpStatusCode.OK, new { successfullyUnbatched });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetServiceSchedules(int getServiceSchedules)
    {
      var serviceScheduleBanks = Services.WebServiceClient.Naedo_GetServiceSchedules();

      return Request.CreateResponse(HttpStatusCode.OK, new { services = serviceScheduleBanks.Keys.ToList(), serviceBanks = serviceScheduleBanks.SelectMany(t => t.Value).ToList() });
    }


    [HttpPost]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage SaveServiceSettings(string servicesString, string serviceBanksString)
    {
      var services = JsonConvert.DeserializeObject<List<NaedoService>>(servicesString);
      var serviceBanks = JsonConvert.DeserializeObject<List<NaedoServiceBank>>(serviceBanksString);

      var serviceSchedules = new Dictionary<NaedoService, List<NaedoServiceBank>>();
      foreach (var service in services)
      {
        serviceSchedules.Add(service, serviceBanks.Where(s => s.ServiceId == service.ServiceId).ToList());
      }

      Services.WebServiceClient.Naedo_UpdateServicesSchedules(serviceSchedules);

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }
  }
}