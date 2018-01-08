using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures.AVS;
using Falcon.Gyrkin.Controllers.Api.Models.Avs;
using Newtonsoft.Json;
using Serilog;

namespace Falcon.Gyrkin.Controllers.Api
{
  public class AvsController : ApiController
  {
    private readonly Lazy<IAvsRepository> _avsRepository;
    private readonly ILogger _logger;

    public AvsController(Lazy<IAvsRepository> avsRepository, ILogger logger)
    {
      _avsRepository = avsRepository;
      _logger = logger;
    }

    public async Task<IHttpActionResult> Transactions([FromBody] AvsTransactionQueryModel model)
    {
      try
      {
        _logger.Information("Transaction  - Calling");
        var result = await _avsRepository.Value.GetCachedTransactions(model.BranchId, model.StartDate, model.EndDate, model.TransactionId, model.IdNumber, model.BankId);
        return Ok(new { Statistics = result.Item1, Transactions = result.Item2 });
      }
      catch (Exception ex)
      {
        _logger.Fatal(ex, "Avs - Transactions");
        var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
       {
         Content = new StringContent(string.Format("Stack Trace: {0} , Inner: {1}", ex.StackTrace, ex.InnerException)),
         ReasonPhrase = "Interal Exception"
       };
        throw new HttpResponseException(resp);
      }
    }

    [HttpGet]
    public HttpResponseMessage GetStaticData()
    {
      try
      {
        return Request.CreateResponse(HttpStatusCode.OK, new { banks = _avsRepository.Value.GetSupportedBanks(), services = _avsRepository.Value.GetActiveServices() });
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    public HttpResponseMessage Resend([FromBody]AvsResendModel model)
    {
      try
      {
        _logger.Information("Resend  - Calling");
        return Request.CreateResponse(HttpStatusCode.OK, _avsRepository.Value.Resend(model.TransactionId, model.ServiceId));
      }
      catch (Exception ex)
      {
        _logger.Fatal(ex, "Avs - Resend");
        var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = new StringContent(string.Format("Stack Trace: {0} , Inner: {1}", ex.StackTrace, ex.InnerException)),
          ReasonPhrase = "Interal Exception"
        };
        throw new HttpResponseException(resp);
      }
    }

    public HttpResponseMessage UpdateServiceSettings([FromBody]AVSUpdateServiceSettingsModel model)
    {
      try
      {
        var services = JsonConvert.DeserializeObject<List<AvsService>>(model.ServicesString);
        var serviceBanks = JsonConvert.DeserializeObject<List<AvsServiceBank>>(model.ServiceBanksString);

        var serviceSchedules = new Dictionary<IAvsService, List<IAvsServiceBank>>();
        foreach (var service in services)
        {
          //IAvsService service && serviceBank;
          var iServiceBanks = new List<IAvsServiceBank>();
          iServiceBanks.AddRange(serviceBanks.Where(s => s.ServiceId == service.ServiceId));
          serviceSchedules.Add(service, iServiceBanks);
        }
        _logger.Information("Resend  - Calling");
        _avsRepository.Value.UpdateServiceSchedule(serviceSchedules);
        return Request.CreateResponse(HttpStatusCode.OK);
      }
      catch (Exception ex)
      {
        _logger.Fatal(ex, "Avs - Resend");
        var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = new StringContent(string.Format("Stack Trace: {0} , Inner: {1}", ex.StackTrace, ex.InnerException)),
          ReasonPhrase = "Interal Exception"
        };
        throw new HttpResponseException(resp);
      }
    }
  }
}