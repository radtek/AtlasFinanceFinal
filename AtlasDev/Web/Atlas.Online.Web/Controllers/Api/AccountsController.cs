using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Models;
using Atlas.Online.Web.Security;
using Atlas.Online.Web.Validations;
using DevExpress.Xpo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Security;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Controllers.Api
{
  [AllowAnonymous]
  [ValidateHttpAntiForgeryToken]
  public class AccountsController : AppApiController
  {
    [HttpGet]
    public HttpResponseMessage EmailInUse(string email)
    {
      if (string.IsNullOrEmpty(email))
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Required 'email' parameter missing.");
      }

      using (UsersContext db = new UsersContext())
      {
        var exists = db.UserProfiles.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && (WebSecurity.CurrentUserId < 0 || u.UserId != WebSecurity.CurrentUserId));
        bool result = !exists;

        return Request.CreateResponse(HttpStatusCode.OK, new
          {
            Valid = result
          });
      }
    }

    [HttpGet]
    public HttpResponseMessage CellInUse(string cell)
    {
      if (string.IsNullOrEmpty(cell))
      {
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Required 'cell' parameter missing.");
      }

      // Normalise 
      cell = UniqueCellAttribute.NormaliseCell(cell);

      bool result = !new XPQuery<Client>(Services.XpoUnitOfWork).Any(
          c =>
            c.Contacts.Any(
              p =>
                p.ContactType.ContactTypeId == Convert.ToInt32(Enumerators.General.ContactType.CellNo) &&
                p.Value == cell) &&
            (WebSecurity.CurrentUserId < 0 || c.UserId == WebSecurity.CurrentUserId));

      return Request.CreateResponse(HttpStatusCode.OK, new
      {
        Valid = result
      });
    }   
  }
}
