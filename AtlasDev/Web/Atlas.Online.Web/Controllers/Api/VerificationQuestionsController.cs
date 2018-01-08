using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Models;
using Atlas.Online.Web.Models.Steps;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Security;
using Atlas.Online.Web.WebService;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Controllers.Api
{
  [Authorize]
  [ValidateHttpAntiForgeryToken]
  public class VerificationQuestionsController : AppApiController
  {
    [HttpGet]
    public HttpResponseMessage Get(long? id)
    {
      if (!id.HasValue)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id parameter required.");
      }

      var application = CurrentClient.CurrentApplication;
      if (application == null || application.ApplicationId != id.Value || application.IsFinal)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid id.");
      }

      var result = Services.WebServiceClient.VER_CheckStatus(id.Value, CurrentClient.ClientId);
      if (result.Success||ConfigurationManager.AppSettings["skipXdsQuestions"] =="true")
      {
        if (application == null)
        {
          return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ErrorMessages.Application_NotFound);
        }

        application.Step = Convert.ToInt32(ApplicationStep.QuoteAcceptance);
        application.Save();
        ((UnitOfWork)application.Session).CommitChanges();

        return RedirectToAction("QuoteAcceptance", "Application", new { id = application.ApplicationId });
      } 
      else if (result.Iteration >= 2)
      {
        return RedirectToAction("Declined", "Application", new { id = application.ApplicationId });
      }

      return Request.CreateResponse(HttpStatusCode.OK, Services.WebServiceClient.VER_GetQuestions(CurrentClient.ClientId));
    }

    [HttpPost]
    public HttpResponseMessage Post(long? id, [FromBody]IEnumerable<Questions> responses)
    {
      if (!id.HasValue)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id parameter required.");
      }

      var application = CurrentClient.CurrentApplication;

      if (CurrentClient == null || application == null || application.ApplicationId != id.Value || application.IsFinal) 
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid id.");
      }
      
      if (application == null)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ErrorMessages.Application_NotFound);
      }

      var result = Services.WebServiceClient.VER_SubmitQuestions(id.Value, CurrentClient.ClientId, responses.ToList());
      if (result.Success)
      {
        application.Step = (int)ApplicationStep.QuoteAcceptance;
        application.Save();
        ((UnitOfWork)application.Session).CommitChanges();

        return RedirectToAction("QuoteAcceptance", "Application", new { id = application.ApplicationId });
      } 
      else if (result.Iteration >= 2) 
      {
        return RedirectToAction("Declined", "Application", new { id = application.ApplicationId });
      }

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        Success = false
      });
    }
  }
}
