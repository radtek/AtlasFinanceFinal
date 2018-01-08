using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Atlas.Common.Extensions;
using Atlas.Enumerators;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Service;
using Account = Atlas.Enumerators.Account;


namespace Falcon.Controllers.Api
{
  public sealed class LoanController : AppApiController
  {
    private enum Fn
    {
      [Description("override")]
      AUTHENTICATION_OVERRIDE,
      [Description("reset")]
      AUTHENTICATION_RESET
    }
    public LoanController()
    {
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    [Authorize]
    public async Task<HttpResponseMessage> Search(string query)
    {
      if (string.IsNullOrEmpty(query))
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Search parameter cannot be null.");

      List<Gyrkin.Library.Service.Account> result = null;
      try
      {
        result = await Services.WebServiceClient.Account_SearchAsync(query);
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { SearchResults = result });
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    [Authorize]
    public async Task<HttpResponseMessage> Get(long? id)
    {
      if (!id.HasValue)
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Account id cannot be null.");

      AccountDetail accountDetail = null;
      try
      {
        accountDetail = await Services.WebServiceClient.Account_GetDetailAsync((long)id);
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { Result = accountDetail });
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage UpdateAccountStatus(long accountId, long userId, Account.AccountStatus newStatus, Account.AccountStatusReason? reason, Account.AccountStatusSubReason? subReason)
    {
      try
      {
        Services.WebServiceClient.Account_UpdateStatus(accountId, userId, newStatus, reason, subReason);
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to update account status.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> OverrideFraudResult(long fId, long fUid, string reason)
    {
      try
      {
        return Request.CreateResponse(HttpStatusCode.OK, new { overriden = await Services.WebServiceClient.Fraud_OverrideResultAsync(fId, fUid, reason) });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to override the fraud result.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> PostNote(long accountId, long personId, string note)
    {
      try
      {
        await Services.WebServiceClient.Account_AddNoteAsync(accountId, personId, note, null);
        return Request.CreateResponse(HttpStatusCode.OK, new { });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to post note.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> AuthenticationResetOverride(long authenticationId, string reason, string fn, long oId)
    {
      try
      {
        if (fn == Fn.AUTHENTICATION_OVERRIDE.ToStringEnum())
        {
          var overrideResult = await Services.WebServiceClient.Authentication_OverrideResultAsync(authenticationId, oId, reason);
          return Request.CreateResponse(HttpStatusCode.OK, new { Fn = fn, Result = overrideResult });
        }
        else
        {
          var resetResult = await Services.WebServiceClient.Authentication_ResetAttemptsAsync(authenticationId, oId, reason);
          return Request.CreateResponse(HttpStatusCode.OK, new { Fn = fn, Result = resetResult });
        }
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, string.Format("Unable to {0} the authentication result.", fn));
      }
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    [Authorize]
    public async Task<HttpResponseMessage> GetAffordabilityCategories(int getAffordabiltiyCategories)
    {
      List<AffordabilityCategory> categories = null;
      try
      {
        categories = await Services.WebServiceClient.Account_Affordability_GetCategoriesAsync(General.Host.Atlas_Online);
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { categories });
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> AffordabilityAcceptOption(long accountId, int affordabilityOptionId)
    {
      try
      {
        await Services.WebServiceClient.Account_Affordability_AcceptOptionAsync(accountId, affordabilityOptionId);
        return Request.CreateResponse(HttpStatusCode.OK, new { });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to Add item to Account Affordability.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> AffordabilityAddItem(long accountId, int affordabilityCategoryId, decimal amount, long personId)
    {
      try
      {
        var affordabilityItem = await Services.WebServiceClient.Account_Affordability_AddItemAsync(accountId, affordabilityCategoryId, amount, personId);
        return Request.CreateResponse(HttpStatusCode.OK, new { affordabilityItem });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to Add item to Account Affordability.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> AffordabilityDeleteItem(long accountId, long affordabilityId, long personId)
    {
      try
      {
        var affordabilityItem = await Services.WebServiceClient.Account_Affordability_DeleteItemAsync(accountId, affordabilityId, personId);
        return Request.CreateResponse(HttpStatusCode.OK, new { affordabilityItem });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to Delete item from Account Affordability.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> AddressAdd(long personId, long userPersonId, int addressTypeId, int provinceId, string line1, string line2, string line3, string line4, string postalCode)
    {
      try
      {
        var address = await Services.WebServiceClient.Person_AddAddressAsync(personId, userPersonId, (General.AddressType)addressTypeId,
          (General.Province)provinceId, line1, line2, line3, line4, postalCode);
        return Request.CreateResponse(HttpStatusCode.OK, new { address });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to Add Address to Person.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> AddressDisable(long personId, long addressId)
    {
      try
      {
        var address = await Services.WebServiceClient.Person_DisableAddressAsync(personId, addressId);
        return Request.CreateResponse(HttpStatusCode.OK, new { address });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to disable Address.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> ContactAdd(long personId, int contactTypeId, string value)
    {
      try
      {
        var contact = await Services.WebServiceClient.Person_AddContactAsync(personId, (General.ContactType)contactTypeId, value);
        return Request.CreateResponse(HttpStatusCode.OK, new { contact });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to Add Contact to Person.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> ContactDisable(long personId, long contactId)
    {
      try
      {
        var contact = await Services.WebServiceClient.Person_DisableContactAsync(personId, contactId);
        return Request.CreateResponse(HttpStatusCode.OK, new { contact });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to disable Contact.");
      }
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    [Authorize]
    public async Task<HttpResponseMessage> GetRelationTypes(int getRelationTypes)
    {
      IEnumerable<dynamic> relationTypes = null;
      try
      {
        await Task.Run(() =>
        {
          relationTypes = EnumUtil.GetValues<General.RelationType>().Where(a => a != General.RelationType.NotSet).Select(a => new { RelationTypeId = (int)a, Description = a.ToStringEnum() });
        });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { relationTypes });
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> CreateRelationship(int addRelation, long personId, long userPersonId, string firstname, string lastname, string cellNo, int relationTypeId)
    {
      try
      {
        var relation = await Services.WebServiceClient.Person_NewRelationAsync(personId, userPersonId, firstname, lastname, cellNo, (General.RelationType)relationTypeId);
        return Request.CreateResponse(HttpStatusCode.OK, new { relation });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to disable Contact.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> UpdateRelationship(int updateRelation, long personId, long relationPersonId, string firstname, string lastname, string cellNo, int relationTypeId)
    {
      try
      {
        var relation = await Services.WebServiceClient.Person_UpdateRelationAsync(personId, relationPersonId, firstname, lastname, cellNo, (General.RelationType)relationTypeId);
        return Request.CreateResponse(HttpStatusCode.OK, new { relation });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to disable Contact.");
      }
    }


    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public async Task<HttpResponseMessage> AdjustAccount(long accountId, string loanAmount, int period)
    {
      try
      {
        await Services.WebServiceClient.Account_AdjustLoanAsync(accountId, decimal.Parse(loanAmount), period);
        return Request.CreateResponse(HttpStatusCode.OK, new { });
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to disable Contact.");
      }
    }
  }
}