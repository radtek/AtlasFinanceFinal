using Falcon.Gyrkin.Library.Security.Claims;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Falcon.Gyrkin.Library.Security
{
  public static class PrincipalHelper
  {
    /// <summary>
    /// Checks the access.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="action">The action.</param>
    /// <param name="resourceClaims">The resource claims.</param>
    /// <returns></returns>
    public static bool CheckAccess(this IPrincipal principal, string resource, string action,
      IList<Claim> resourceClaims)
    {
      var context = new AuthorizationContext(principal as ClaimsPrincipal, resource, action);

      resourceClaims.ToList().ForEach(c => context.Resource.Add(c));
      var config = new IdentityConfiguration();
      return config.ClaimsAuthorizationManager.CheckAccess(context);
    }

    /// <summary>
    /// Checks the access.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <param name="resource">The resource.</param>
    /// <param name="action">The action.</param>
    /// <returns></returns>
    public static bool CheckAccess(this IPrincipal principal, string resource, string action)
    {
      var context = new AuthorizationContext(principal as ClaimsPrincipal, resource, action);
      var resourceClaims = context.Principal.Claims.ToList();
      resourceClaims.ToList().ForEach(c => context.Resource.Add(c));
      var config = new IdentityConfiguration();
      return config.ClaimsAuthorizationManager.CheckAccess(context);
    }

    /// <summary>
    /// Gets the full name.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns></returns>
    public static string GetFullName(this IPrincipal principal)
    {

      return String.Format("{0} {1}",
        ((ClaimsPrincipal) principal).FindFirst(System.Security.Claims.ClaimTypes.GivenName).Value,
        ((ClaimsPrincipal) principal).FindFirst(System.Security.Claims.ClaimTypes.Surname).Value);
    }

    /// <summary>
    /// Gets the first name.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns></returns>
    public static string GetFirstName(this IPrincipal principal)
    {
      return principal.GetClaimValue(System.Security.Claims.ClaimTypes.GivenName);
    }

    public static string GetEMail(this IPrincipal principal)
    {
      return principal.GetClaimValue(System.Security.Claims.ClaimTypes.Email);
    }

    public static string GetClaimValue(this IPrincipal principal, string claimType)
    {
      var claim = ((ClaimsPrincipal) principal).FindFirst(claimType);
      return claim != null ? claim.Value : string.Empty;
    }

    public static List<Department> GetDepartments(this IPrincipal principal, string role)
    {
      var claim = ((ClaimsPrincipal) principal).Claims.FirstOrDefault(c => c.Type == role);
      if (claim == null)
      {
        return new List<Department>();
      }
      var capabilityClaim = claim as ComplexClaim<CapabilityClaim>;
      if (capabilityClaim == null)
      {
        capabilityClaim = new ComplexClaim<CapabilityClaim>(role,
          JsonConvert.DeserializeObject<CapabilityClaim>(claim.Value));
      }

      var d = capabilityClaim.Value.Departments; //capabilityClaim.Value.Departments;
      return d;
    }

    /// <summary>
    /// Gets the token.
    /// </summary>
    /// <param name="principal">The principal.</param>
    /// <returns></returns>
    public static JwtSecurityToken GetToken(this IPrincipal principal)
    {
      try
      {
        var identity = ((ClaimsPrincipal) principal).Identity as ClaimsIdentity;
        var bc = identity.BootstrapContext as BootstrapContext;
        JwtSecurityToken jwt = bc.SecurityToken as JwtSecurityToken;
        if (jwt == null)
        {
          var handlers = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers;
          //var sr = new StringReader(bc.Token);
          //var xtr = new XmlTextReader(sr);
          //jwt = handlers.ReadToken(xtr) as JwtSecurityToken;
          jwt = handlers.ReadToken(bc.Token) as JwtSecurityToken;
        }
        return jwt;
      }
      catch (Exception)
      {
        return null;
      }
    }

    public static string GetTokenString(this IPrincipal principal)
    {
      var token = principal.GetToken();
      return token != null ? token.RawData : string.Empty;
    }
  }
}