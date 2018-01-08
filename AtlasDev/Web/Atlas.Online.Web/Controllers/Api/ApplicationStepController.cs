using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Configuration;
using Atlas.Online.Web.Filters;
using Atlas.Online.Web.Helpers;
using Atlas.Online.Web.Models;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Models.Steps;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Security;
using DevExpress.Xpo;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Controllers.Api
{

  [Authorize]
  [ValidateHttpAntiForgeryToken]
  public class ApplicationStepController : AppApiController
  {
    private Application _currentApplication = null;

    public Application CurrentApplication
    {
      get
      {
        if (_currentApplication == null)
        {
          _currentApplication = Application.GetForUser(
            Services.XpoUnitOfWork,
            WebSecurity.CurrentUserId,
            isCurrent: true
         );
        }

        return _currentApplication;
      }
    }

    /// <summary>
    /// Returns the last saved step of the current client
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public object Resume()
    {     
      return ApplicationStepFactory.Create(CurrentApplication);
    }

    [HttpPost]    
    public object Next([FromBody]JObject data)
    {
      ApplicationStepBase step = ApplicationStepFactory.Create(data.Value<int>("Id"), data);
      
      if (!step.IsValid())
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ErrorMessages.Application_StepValidationFail);
      }

      var application = CurrentApplication;
      int nextStep = ApplicationStepFactory.GetStepId(step.Id, ApplicationStepFactory.Direction.Forward);

      try
      {
        if (step.IsDirty)
        {
           step.Save(ref application, new HttpRequestWrapper(HttpContext.Current.Request));
        }

        // Update current step if the user has progressed
        if (nextStep > application.Step)
        {
          application.Step = nextStep;
          application.Save();
        }        
      }
      catch (Exception)
      {
        Services.XpoUnitOfWork.RollbackTransaction();
        throw;
      }

      Services.XpoUnitOfWork.CommitChanges();
      step.AfterCommit(application);

      // Load next step
      return ApplicationStepFactory.Create(nextStep, application);
    }

    public object Get(int? stepId)
    {
      if (!stepId.HasValue)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "stepId Parameter required.");
      }

      int appStep = CurrentApplication == null ? 1 : CurrentApplication.Step;
      if (stepId > appStep)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ErrorMessages.Application_CannotGetStepPastCurrent);
      }

      return ApplicationStepFactory.Create(stepId.Value, CurrentApplication, ApplicationStepFactory.Direction.None);
    }

    [HttpGet]
    public object Back(int? stepId)
    {
      if (!stepId.HasValue)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "stepId Parameter required.");
      } 
      
      int appStep = CurrentApplication == null ? (int)ApplicationStep.PersonalDetails : CurrentApplication.Step;
      if (stepId - 1 > appStep)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ErrorMessages.Application_CannotGetStepPastCurrent);
      }

      return ApplicationStepFactory.Create(stepId.Value, CurrentApplication, ApplicationStepFactory.Direction.Backward);
    }    
    
    [HttpGet]
    public HttpResponseMessage AccountCDV(General.BankName? bank, General.BankAccountType? accType, string accountNo)
    {
      if (string.IsNullOrEmpty(accountNo) || !bank.HasValue || !accType.HasValue)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Required parameters missing for CDV.");
      }

      // Normalise
      accountNo = Regex.Replace(accountNo, "[^.0-9]", String.Empty);      

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        Valid = Services.WebServiceClient.CDV_VerifyAccount(bank.Value, accType.Value, accountNo)
      });
    }

    [HttpGet]
    public HttpResponseMessage IdNumberInUse(string idNumber)
    {
      if (string.IsNullOrEmpty(idNumber))
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Required 'idNumber' parameter missing.");
      }

      // Normalise
      idNumber = Regex.Replace(idNumber, "[^.0-9]", String.Empty);

      int clientId = CurrentClient.ClientId;

      bool result = !new XPQuery<Client>(Services.XpoUnitOfWork).Any(
        // Check by idNumber excluding the current client id
        c => c.IDNumber == idNumber && c.ClientId != clientId
      );

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        Valid = result
      });
    }

    [HttpGet]
    public HttpResponseMessage Status(long applicationId)
    {     
      var application = CurrentApplication;
      if (application == null || application.ApplicationId != applicationId)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, 
          String.Format(ErrorMessages.Application_NoApplicationForUser, WebSecurity.CurrentUserId));
      }

      return Request.CreateResponse(HttpStatusCode.OK, new 
      {
        Status = application.Status
      });
    }

    [HttpGet]
    public HttpResponseMessage ProcessingRedirect(long? id)
    {
      if (!id.HasValue)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ID parameter required.");
      }

      var application = Application.GetFirstBy(Services.XpoUnitOfWork, x => x.ApplicationId == id && x.Client.UserId == WebSecurity.CurrentUserId);
      if (application == null)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
          String.Format(ErrorMessages.Application_NoApplicationForUser, WebSecurity.CurrentUserId));
      }

      // Check application status
      switch (application.Status)
      {        
        case Account.AccountStatus.Inactive:
          // Still waiting for a result...
          break;
        case Account.AccountStatus.Technical_Error:
          return RedirectToAction("TechnicalError", "Application", new { id = application.ApplicationId });
        case Account.AccountStatus.Declined:
          return RedirectToAction("Declined", "Application", new { id = application.ApplicationId });
        case Account.AccountStatus.Review:
          return RedirectToAction("Review", "Application", new { id = application.ApplicationId });
        default:
          return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
            String.Format(ErrorMessages.Application_InvalidStatus, application.Status.ToString()));
      }

      // Check AVS
      if (!application.BankDetail.IsVerified && application.Status != Account.AccountStatus.Inactive)
      {
        long? tid = application.BankDetail.TransactionId;
        AVS.Result? avsResult = tid.HasValue ? Services.WebServiceClient.AVS_GetResponse(CurrentClient.PersonId, tid.Value) : null;

        if (!avsResult.HasValue || avsResult.Value == AVS.Result.Failed)
        {
          // Set application back to confirm and verify step.
          application.Step = ApplicationStepFactory.GetStepIdByType(typeof(ConfirmVerifyStep));
          application.Save();
          Services.XpoUnitOfWork.CommitChanges();

          return RedirectToAction("Index", "Application");
        }
      }

      // Check affordability      
      if (application.Affordability != null)
      {
        if (application.Affordability.Accepted || application.Affordability.OptionType == Account.AffordabilityOptionType.RequestedOption)
        {
          return RedirectToAction("Verification", "Application", new { id = application.ApplicationId });
        }
        else
        {
          // Redirect to affordability
          return RedirectToAction("Affordability", "Application", new { id = application.ApplicationId });
        }
      }

      if (application.Step < Convert.ToInt32(ApplicationStep.Verify))
      {
        return RedirectToAction("Index", "Application");
      }

      if (application.CurrentStep == ApplicationStep.QuoteAcceptance)
      {
        return RedirectToAction("QuoteAcceptance", "Application", new { id = application.ApplicationId });
      }

      // We dont know where to redirect yet.
      return Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
