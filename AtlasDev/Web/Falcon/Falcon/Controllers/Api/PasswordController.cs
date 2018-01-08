using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Falcon.Controllers.Api
{
  public sealed class PasswordController : AppApiController
  {

    public PasswordController()
    {
    }

    [HttpPost]
    public async Task<HttpResponseMessage> Verify([FromBody]dynamic value)
    {
      //try
      //{
      //  string result = Services.WebServiceClient.Person_ValidatePasswordReset(value["hash"].ToString());

      //  // locate user based on stored value
      //  if (!string.IsNullOrEmpty(result.ToString()))
      //  {

      //    ApplicationDbContext context = new ApplicationDbContext();
      //    UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(context);
      //    UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(store);
      //    ApplicationUser user = UserManager.FindByName(result);
      //    String newPassword = UserManager.PasswordHasher.HashPassword(value["password"].ToString());
      //    ApplicationUser dbUser = await store.FindByIdAsync(user.Id);
      //    await store.SetPasswordHashAsync(dbUser, newPassword);
      //    dbUser.IsPasswordReset = true;
      //    await store.UpdateAsync(dbUser);

      //    return Request.CreateResponse(HttpStatusCode.OK, new { isValid = true, Value = "Password Reset", URL = "/" });
      //  }

      //  return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Password reset token has expired.");
      //}
      //catch (Exception)
      //{
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "An error has occurred during username discovery.");
      //}
    }
  }
}