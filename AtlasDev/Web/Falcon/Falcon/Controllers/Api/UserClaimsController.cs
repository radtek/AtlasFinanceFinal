using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using Falcon.Models;
using Falcon.Common;
using Falcon.Gyrkin.Library.Common;


namespace Falcon.Controllers.Api
{
  [AllowAnonymous]
  public sealed class UserClaimsController : AppApiController
  {

    private readonly UserAccountService<NhUserAccount> userAccountService;

    public UserClaimsController(UserAccountService<NhUserAccount> userAccountService)
    {
      this.userAccountService = userAccountService;
    }

    [HttpGet]
    //[WebApiAntiForgeryToken]
    public HttpResponseMessage GetUserClaimsData(Guid UserId)
    {
      var user = userAccountService.GetByID(UserId);

      var claims = user.ClaimsCollection.ToList();
      var roles = new List<UserModel.WebRole>();
      foreach (var claim in claims)
      {
        var r = new UserModel.WebRole() { ClaimSignature = claim.Type, RoleName = claim.Value };
        roles.Add(r);
      }
      return Request.CreateResponse(HttpStatusCode.OK, new { Roles = roles });
    }

    [HttpGet]
    public HttpResponseMessage ChangeUserPassword(Guid UserId, string NewPassword)
    {
     Tuple<string,string> hashValue =  CookieHelper.HashPassword(NewPassword);
     userAccountService.SetPassword(UserId, NewPassword);
      return Request.CreateResponse(HttpStatusCode.OK,"Sucsess");
    }
  }
}