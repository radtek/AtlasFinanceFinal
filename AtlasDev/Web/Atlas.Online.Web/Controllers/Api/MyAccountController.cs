using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Models;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Security;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Controllers.Api
{
  [Authorize]
  [ValidateHttpAntiForgeryToken]
  public class MyAccountController : AppApiController
  {
    [HttpPost]
    public HttpResponseMessage UpdateBankDetails(long clientId, [FromBody]BankDetailDto details)
    {
      if (CurrentClient == null || CurrentClient.ClientId != clientId)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid client id.");
      }

      if (!ModelState.IsValid)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bank details are invalid.");
      }

      var currentBankDetails = CurrentClient.BankDetails.FirstOrDefault(x => x.IsEnabled);
      AVS.Result? avs = null;
      if (currentBankDetails == null || !details.Equals(currentBankDetails))
      {
        if (currentBankDetails != null)
        {
          currentBankDetails.IsEnabled = false;
          currentBankDetails.Save();
        }

        var bankDetail = details.ToBankDetail(Services.XpoUnitOfWork);
        CurrentClient.BankDetails.Add(bankDetail);
        CurrentClient.Save();

          if (CurrentClient.CurrentApplication != null)
          {
              CurrentClient.CurrentApplication.BankDetail = bankDetail;
              CurrentClient.CurrentApplication.Save();
          }

          Services.XpoUnitOfWork.CommitChanges();
     

        // Submit AVS
        Services.WebServiceClient.AVS_SubmitAsync(CurrentClient.ClientId);
        avs = AVS.Result.NoResult;
      }

      if (!avs.HasValue && currentBankDetails != null && currentBankDetails.TransactionId.HasValue)
      {
        avs = Services.WebServiceClient.AVS_GetResponse(CurrentClient.PersonId, currentBankDetails.TransactionId.Value);
      }

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        AvsStatus = avs
      });
    }

    public HttpResponseMessage UpdatePersonalDetails(long clientId, [FromBody]PersonalDetailsDto details)
    {
      if (CurrentClient == null || CurrentClient.ClientId != clientId)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid client id.");
      }

      if (!ModelState.IsValid)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Personal details are invalid.");
      }

      var client = CurrentClient;
      var session = (UnitOfWork)client.Session;

      // Address
      var address = client.Addresses.FirstOrDefault(x => x.AddressType.Type == General.AddressType.Residential);
      if (address == null || address.IsDeleted)
      {
        address = new Address(session);

        client.Addresses.Add(address);
      }
      address.AddressLine1 = details.Address1;
      address.AddressLine2 = details.Address2;
      address.AddressLine3 = details.Address3;
      address.AddressLine4 = details.City;
      address.AddressType = new XPQuery<AddressType>(session).First(a => a.Type == General.AddressType.Residential);
      address.Province = new XPQuery<Province>(session).First(a => a.ProvinceId == Convert.ToInt32(details.Province));
      address.PostalCode = details.PostalCode;
      address.Client = client;
      address.Save();

      if (client.SetContact(General.ContactType.CellNo, details.CellNo) || !client.OTPVerified)
      {
        client.OTPVerified = false;
        client.Save();
        
        session.CommitChanges();

        return RedirectTo(new { 
          controller = "Account", 
          action = "OTP", 
          next = new Uri(Url.Link("MyAccount", new { controller = "MyAccount", action = "Index" })).PathAndQuery 
        });
      }

      session.CommitChanges();

      return Request.CreateResponse(HttpStatusCode.NoContent);
    }

    public HttpResponseMessage UpdateLoginDetails(long clientId, [FromBody]LoginDetailsModel details)
    {
      if (CurrentClient == null || CurrentClient.ClientId != clientId)
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid client id.");
      }

      if (!ModelState.IsValid)
      {
        // Hack: StringLength attribute dfoesn't take into account that [Required] is not set
        //       so we will just ignore the error about string length if Password is blank.
        if (ModelState.ContainsKey("details.Password"))
        {
          ModelState.Remove("details.Password");
        }

        if (!ModelState.IsValid)
        {
          return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Personal details are invalid.");
        }
      }

      var client = CurrentClient;
      var session = (UnitOfWork)client.Session;
      bool reissueToken = false;

      using (UsersContext db = new UsersContext())
      {
        var profile = db.UserProfiles.FirstOrDefault(x => x.UserId == WebSecurity.CurrentUserId);

        if (!profile.Email.Equals(details.Email, StringComparison.OrdinalIgnoreCase))
        {
          client.SetContact(General.ContactType.Email, details.Email);

          profile.Email = details.Email;
          db.Entry(profile).CurrentValues.SetValues(profile);
          db.SaveChanges();

          // Reissue authentication token
          FormsAuthentication.SetAuthCookie(profile.Email, true);
          reissueToken = true;
        }

        if (!String.IsNullOrEmpty(details.Password) && !String.IsNullOrEmpty(details.CurrentPassword))
        {
          if (!WebSecurity.ChangePassword(WebSecurity.CurrentUserName, details.CurrentPassword, details.Password))
          {
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ErrorMessages.MyAccount_PasswordChangeFailed);
          }
        }
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { reissueToken = reissueToken });
    }    
  }
}
