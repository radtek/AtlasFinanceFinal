using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;
using NHibernate;

namespace Falcon.Areas.User.Controllers
{
  [ClaimsAuthorize(ClaimTypes.Role, new string[] { RoleType.ADMINISTRATOR })]
  public class ManagementController : Controller
  {
    private readonly AuthenticationService<NhUserAccount> authenticationService;

    private readonly ISession session;

    private readonly UserAccountService<NhUserAccount> userAccountService;

    public ManagementController(
            AuthenticationService<NhUserAccount> authenticationService,
            ISession session,
            UserAccountService<NhUserAccount> userAccountService)
    {

      this.authenticationService = authenticationService;
      this.session = session;
      this.userAccountService = userAccountService;
    }
    // GET: User/Management
    public async Task<ActionResult> Index()
    {
     
      return View();
    }

    public ActionResult Add()
    {
      return View();
    }
  }
}