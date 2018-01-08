using System.Collections.Generic;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Falcon.Gyrkin.Library.Security.Attributes
{
  public class ClaimsAuthorizeAttribute : AuthorizeAttribute
  {
    private string claimType;
    private string[] claimValue;

    public ClaimsAuthorizeAttribute()
    {

    }
    public ClaimsAuthorizeAttribute(string type, string[] value)
    {
      claimType = type;
      claimValue = value;
    }

    public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
    {
      var user = HttpContext.Current.User as ClaimsPrincipal;

      List<bool> _claimsCheck = new List<bool>();

      if (claimValue.Length > 1)
      {
        foreach (var cl in claimValue)
          _claimsCheck.Add(user.HasClaim(claimType, cl.Trim()));
      }
      else
        _claimsCheck.Add(user.HasClaim(claimType, claimValue[0]));

      if (_claimsCheck.Contains(true))
        base.OnAuthorization(filterContext);
      else
        HandleUnauthorizedRequest(filterContext);
    }
    protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
    {
      // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
      filterContext.Result = new RedirectToRouteResult("Security",         
                                 new RouteValueDictionary 
                                   {
                                       { "action", "Denied" },
                                       { "controller", "Access" }
                                   });
    }
  }
}