using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using BrockAllen.MembershipReboot.Nh.Repository;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Models;
using NHibernate;

namespace Falcon.Controllers.Api
{
  //[Authorize(Roles = "Administrator")]
  public sealed class RoleController : AppApiController
  {
    private readonly AuthenticationService<NhUserAccount> authenticationService;
    private readonly ISession session;
    private readonly UserAccountService<NhUserAccount> userAccountService;
    private readonly IRepository<NhUserClaim> _userClaimRepo;

    public RoleController()
    {
    }
    public RoleController(AuthenticationService<NhUserAccount> authenticationService,
            ISession session,
            UserAccountService<NhUserAccount> userAccountService, IRepository<NhUserClaim> userClaimRepo)
    {

      this.authenticationService = authenticationService;
      this.session = session;
      this.userAccountService = userAccountService;
      _userClaimRepo = userClaimRepo;
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetWebRoles()
    {
      var roles = Services.WebServiceClient.Operations_GetWebRoles();

      return Request.CreateResponse(HttpStatusCode.OK, new { roles });
    }


    // POST api/<controller>
    [HttpPost]
    public bool Save(UserModel.SaveRoleModel model)
    {
      try
      {
        var user = userAccountService.GetByID(model.UserId);

        foreach (var role in model.Roles)
        {
          if (user.Claims.Count(p => p.Value == role.RoleName) == 0)
          {
            userAccountService.AddClaim(model.UserId, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role.RoleName);
          }
        }

        var rolesToDelete =
          user.Claims.Where(r => !model.Roles.Select(a => a.RoleName).Contains(r.Value)).Select(r => r.Value).ToList();
        foreach (var roleToDelete in rolesToDelete)
        {
          userAccountService.RemoveClaim(model.UserId, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", roleToDelete);

          var delete = roleToDelete;
          var claim = _userClaimRepo.FindBy(c => c.Account.ID == user.ID && c.Value == delete);
          if (claim != null)
          {
            _userClaimRepo.Delete(claim);
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return true;
    }
  }
}