using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Security;
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
  public class AffordabilityController : AppApiController
  {
    [HttpPost]
    public HttpResponseMessage Accept(long? id)
    {
      var affordability = Affordability.GetFirstBy(Services.XpoUnitOfWork, x => x.Application.ApplicationId == id && x.Application.Client.UserId == WebSecurity.CurrentUserId);
      if (affordability == null)
      {
        throw new InvalidOperationException("Affordability not found.");
      }

      // Accept affordability
      Services.WebServiceClient.APP_AcceptAffordability(affordability.Application.ApplicationId);

      return RedirectToAction("Verification", "Application", new { id = id });
    }

    [HttpPost]
    public HttpResponseMessage Reject(long? id)
    {
      //var affordability = Affordability.GetFirstBy(Services.XpoUnitOfWork, x => x.Application.ApplicationId == id && x.Application.Client.UserId == WebSecurity.CurrentUserId);
      //if (affordability == null)
      //{
      //  throw new InvalidOperationException("Affordability not found.");
      //}

      // TODO: Any logic for rejecting an option?

      return RedirectToAction("Index", "MyAccount");
    }
  }
}
