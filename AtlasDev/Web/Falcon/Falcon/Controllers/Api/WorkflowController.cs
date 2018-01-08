using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Atlas.Enumerators;

namespace Falcon.Controllers.Api
{
  public sealed class WorkflowController : AppApiController
  {
    // GET api/<controller>
    [HttpGet]
    //[WebApiAntiForgeryToken]
    public HttpResponseMessage GetWorkflow(int hostId, long? branchId, string accountNo, DateTime? startDate, DateTime? endDate)
    {
      var response = Services.WebServiceClient.Workflow_Get(hostId > 0 ? (General.Host)hostId : General.Host.Falcon, branchId, accountNo, startDate, endDate);

      return Request.CreateResponse(HttpStatusCode.OK, new { processes = response });
    }


    [HttpGet]
    public HttpResponseMessage GetProcessSteps(long processJobId)
    {
      var response = Services.WebServiceClient.Workflow_GetProcessSteps(processJobId);

      return Request.CreateResponse(HttpStatusCode.OK, new { processes = response });
    }


    [HttpPost]
    public HttpResponseMessage RedirectAccountToProcessStep(long processStepJobAccountId, long userId)
    {
      Services.WebServiceClient.Workflow_RedirectAccountToProcessStep(processStepJobAccountId, userId);

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }
  }
}