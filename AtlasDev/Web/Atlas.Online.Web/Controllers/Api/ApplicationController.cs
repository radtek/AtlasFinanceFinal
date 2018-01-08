using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Data.Models.DTO;
using Atlas.Online.Web.Security;
using Atlas.Online.Web.WebService;
using DevExpress.Xpo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Controllers.Api
{
  [Authorize]
  [ValidateHttpAntiForgeryToken]
  public class ApplicationController : AppApiController
  {
    // GET api/application
    public IEnumerable<ApplicationDto> Get()
    {
      var result = new XPQuery<Application>(Services.XpoUnitOfWork)
        .Where(x => x.Client.UserId == WebSecurity.CurrentUserId).Take(20).ToList();

      return AutoMapper.Mapper.Map<List<Application>, List<ApplicationDto>>(result);
    }

    // GET api/application/5
    public ApplicationDto Get(long id)
    {
      var result = new XPQuery<Application>(Services.XpoUnitOfWork)
        .FirstOrDefault(x => x.ApplicationId == id && x.Client.UserId == WebSecurity.CurrentUserId);

      return AutoMapper.Mapper.Map<Application, ApplicationDto>(result);
    }

    public ApplicationDto Current()
    {
      var result = new XPQuery<Application>(Services.XpoUnitOfWork)
        .FirstOrDefault(x => x.IsCurrent && x.Client.UserId == WebSecurity.CurrentUserId);

      return AutoMapper.Mapper.Map<Application, ApplicationDto>(result);
    }

    [HttpGet]
    public HttpResponseMessage Repayment(long? id)
    {
      if (!id.HasValue)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID parameter required.");
      }

      var application = new XPQuery<Application>(Services.XpoUnitOfWork)
        .FirstOrDefault(x => x.ApplicationId == id && x.Client.UserId == WebSecurity.CurrentUserId);

      if (application == null)
      {
        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Application not found.");
      }

      if (application.Affordability == null)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cannot settle application without affordability.");
      }

      var settlement = Services.WebServiceClient.MYC_CheckSettlement(id.Value);

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        HasSettlement = settlement != null,
        OrigRepaymentDate = application.RepaymentDate,
        RepaymentAmount = settlement != null ? settlement.Amount : application.Affordability.RepaymentAmount,
        RepaymentDate = settlement != null ? settlement.RepaymentDate : application.RepaymentDate,
      });

    }

    [HttpPost]
    public HttpResponseMessage SubmitSettlement(long? id, [FromBody]JObject data)
    {
      DateTime? newDate = data.Value<DateTime?>("newDate");

      if (!newDate.HasValue)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "newDate parameter missing.");
      }

      // TODO: Any sanity checks?
      var result = Services.WebServiceClient.APP_SubmitSettlement(new ApplicationSettlementSubmission() { 
        RepaymentDate = newDate.Value, 
        ApplicationId = id.Value 
      });

      return Request.CreateResponse(HttpStatusCode.OK, result);
    }
  }
}
