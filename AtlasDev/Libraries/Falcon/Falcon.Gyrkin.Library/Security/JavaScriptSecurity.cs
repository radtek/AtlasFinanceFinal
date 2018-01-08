using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;

namespace Falcon.Gyrkin.Library.Security
{
  public static class JavascriptSecurity
  {
    private static readonly string[] _purposes = new string[] { "System.Web.Helpers.AntiXsrf.AntiForgeryToken.v1" };

    internal static string Protect(string unprotectedData)
    {
      return HttpServerUtility.UrlTokenEncode(MachineKey.Protect(Encoding.UTF8.GetBytes(unprotectedData), _purposes));
    }

    internal static string Unprotect(string protectedData)
    {
      return Encoding.UTF8.GetString(MachineKey.Unprotect(HttpServerUtility.UrlTokenDecode(protectedData)));
    }

    public static void ValidateToken(string actionName, string token)
    {
      var rvt = Unprotect(token);

      if (rvt == null)
        throw new InvalidOperationException("Cannot read XSRF token");

      var parts = rvt.Split(':');
      if (parts.Length != 3)
        throw new InvalidOperationException("Malformed XSRF token");

      var actions = parts[0];
      var cookieToken = parts[1];
      var formToken = parts[2];

      // Check that this action is allowed by the token
      if (!actions.Split(';').Any(s => s.Equals(actionName)))
        throw new InvalidOperationException("Action not allowed");

      AntiForgery.Validate(cookieToken, formToken);
    }

    internal static string GenerateToken(IEnumerable<string> actions, string oldCookieToken = null)
    {
      string cookieToken, formToken;
      AntiForgery.GetTokens(oldCookieToken, out cookieToken, out formToken);

      var actionStr = String.Join(";", actions);
      return Protect(String.Format("{0}:{1}:{2}", actionStr, cookieToken, formToken));
    }
  }
}