using System.Net;
using System.Net.Http;
using System.Web.Http;
using Falcon.Models;

namespace Falcon.Controllers.Api
{
  [AllowAnonymous]
  public sealed class AssUserOverrideController : AppApiController
  {
    // Functionality to call the Ass User bypass functionality.

    [HttpPost]
    public HttpResponseMessage AssUserOverride(AssUserOverrideModel model)
    {
      var result = Services.WebServiceClient.AssInt_AddUserOverride(model.StartDate, model.EndDate, model.UserOperatorCode,
                                                                     model.BranchNum, model.RegionalOperatorCode, model.NewLevel, model.Reason);
      return result > 0 ? Request.CreateResponse(HttpStatusCode.OK, result.ToString()) :
        Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, "Internal Server Error occurred.");
    }

  }
}