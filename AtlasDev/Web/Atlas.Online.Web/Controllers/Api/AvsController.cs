using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Common.Mappers;
using Atlas.Online.Web.Helpers;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Security;
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
  public class AvsController : AppApiController
  {
    private BankDetail _currentBankDetails = null;

    public BankDetail CurrentBankDetails
    {
      get
      {
        if (_currentBankDetails == null)
        {
          if (CurrentClient == null) {
            return null;
          }

          _currentBankDetails = CurrentClient.BankDetails.FirstOrDefault(
            x => x.IsEnabled
          );
        }

        return _currentBankDetails;
      }
    }

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

    [HttpGet]
    public HttpResponseMessage Status()
    {
      if (CurrentBankDetails == null)
      {
        return Request.CreateResponse(HttpStatusCode.OK, new
        {
          Status = false
        });
      }

      if (CurrentBankDetails.IsVerified && CurrentBankDetails.IsActive)
      {
        return Request.CreateResponse(HttpStatusCode.OK, new
        {
          Status = AVS.Result.Passed
        });
      }

      long? tid = CurrentBankDetails.TransactionId;

      AVS.Result? status = null;

      if (tid.HasValue)
      {
        status = Services.WebServiceClient.AVS_GetResponse(CurrentClient.PersonId, tid.Value);
      }

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        Status = status
      });
    }

    [HttpPost]
    public HttpResponseMessage Submit([FromBody]JObject request)
    {
      var application = CurrentApplication;
      var applicationId = request.Value<long?>("ApplicationId");

      if (application == null || application.ApplicationId != applicationId.Value)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ErrorMessages.Application_NotFound);
      }

      // Save bank details
      BankDetailDto details = JMapper.Map<BankDetailDto>(request["data"]);
      if (!Validation.IsValidObject(details))
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ErrorMessages.Validation_FailedObject);
      }

      // If new bank details were created for this application then do another AVS request
      if (details.SaveApplication(ref application))
      {
        // Submit AVS request
        Services.WebServiceClient.AVS_Submit(application.Client.ClientId);
      }

      return Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
