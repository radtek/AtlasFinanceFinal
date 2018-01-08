using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using NHibernate;

using Falcon.Areas.User.Models;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;
using Falcon.Models;
using Falcon.Services;


namespace Falcon.Controllers.Api
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR })]
  public sealed class UserController : AppApiController
  {
    private readonly AuthenticationService<NhUserAccount> authenticationService;
    private readonly ISession session;
    private readonly UserAccountService<NhUserAccount> userAccountService;


    public UserController(AuthenticationService<NhUserAccount> authenticationService,
            ISession session,
            UserAccountService<NhUserAccount> userAccountService)
    {

      this.authenticationService = authenticationService;
      this.session = session;
      this.userAccountService = userAccountService;
    }

    public UserController()
    {

    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetPermissions(long personId, bool cacheRoles)
    {
      var roles = Services.WebServiceClient.Operations_GetPermissions(personId, cacheRoles);

      return Request.CreateResponse(HttpStatusCode.OK, new { roles });
    }


    [HttpGet]
    public HttpResponseMessage GetAccessibleCompanies(long getAccessibleCompaniesPersonId)
    {
      // get the companies that the user can access

      return Request.CreateResponse(HttpStatusCode.OK, new { });
    }


    [HttpGet]
    public HttpResponseMessage GetAccessibleHosts(long getAccessibleHostsPersonId)
    {
      var hosts = Services.WebServiceClient.Host_GetAccessible(getAccessibleHostsPersonId);

      return Request.CreateResponse(HttpStatusCode.OK, new { hosts });
    }


    [HttpPost]
    public async Task<HttpResponseMessage> SaveUser(SaveUserModel model)
    {
      var user = Guid.Empty;
      using (var tx = session.BeginTransaction())
      {
        try
        {
          // create web account
          var account = userAccountService.CreateAccount(model.Username, model.Password, model.Email);
          tx.Commit();
          user = account.ID;
        }
        catch (Exception ex)
        {
          return Request.CreateResponse(HttpStatusCode.InternalServerError , new { ex });
        }
      }
      using (var tx = session.BeginTransaction())
      {
        // set active.
        userAccountService.SetIsLoginAllowed(user, true);
        var account = userAccountService.GetByID(user);
        userAccountService.SetConfirmedEmail(user, account.Email);
        tx.Commit();

        // link to core.

        await WebApiClient.LinkUser(model.PersonId, user);
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { user });
    }

    //[HttpPost]
    //public HttpResponseMessage DeleteUser(DeleteUserModel model)
    //{
    //  using (var tx = this.session.BeginTransaction())
    //  {
    //    userAccountService.DeleteAccount(model.UserId);
    //    tx.Commit();
    //  }

    //  return Request.CreateResponse(HttpStatusCode.OK, "Saved");
    //}


    [HttpGet]
    public async Task<HttpResponseMessage> GetWebUsers()
    {
      var webUsers = new List<UserModel.WebUser>();
      try
      {
        var users = new List<NhUserAccount>();
        //var users = userAccountService.GetNotVerifiedAccounts(null);
        //var users = userAccountService.GetAll();
        //userAccountService.get

        foreach (var user in users)
        {
          var u = new UserModel.WebUser();

          var checkLink = await WebApiClient.CheckIfUserLinked(user.ID);

          if (checkLink != null)
          {
            u.Linked = true;
            u.PersonId = Convert.ToInt64(checkLink["PersonId"]);
          }
          u.Created = user.Created;
          u.Email = user.Email;
          u.Id = user.ID;
          u.LastLogin = user.LastLogin;
          u.UserName = user.Username;
          u.Roles = new List<UserModel.WebRole>();

          var claims = user.ClaimsCollection.ToList();
          foreach (var claim in claims)
          {
            var r = new UserModel.WebRole() { ClaimSignature = claim.Type, RoleName = claim.Value };
            if (!u.Roles.Contains(r))
              u.Roles.Add(r);
          }

          webUsers.Add(u);
        }


        var _roles = RoleType.GetClaims();
        var roles = new List<UserModel.WebRole>();
        foreach (var role in _roles)
        {
          var r = new UserModel.WebRole() { RoleName = role.Item1, ClaimSignature = role.Item2 };
          roles.Add(r);
        }

        return Request.CreateResponse(HttpStatusCode.OK, new { Roles = roles, WebUsers = webUsers });
      }
      catch (Exception ex)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
      }
    }

    [HttpGet]
    public HttpResponseMessage GetWebUsersByGuid(Guid UserId)
    {
      var falconUsers = new List<UserModel.WebUser>();
      try
      {
        var usersAll = userAccountService.GetByID(UserId);
        var u1 = new UserModel.WebUser()
            {
              UserName = usersAll.Username,
              Email = usersAll.Email,
              LastLogin = usersAll.LastLogin,
              Created = usersAll.Created,
              Id = UserId
            };

        falconUsers.Add(u1);
        return Request.CreateResponse(HttpStatusCode.OK, new { FalconUsers = falconUsers });
      }
      catch (Exception ex)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
      }
    }
  }
}