using System;
using System.Web;
using System.Web.Security;

namespace AutoFac.Web.Core.Authentication
{
  public class DefaultFormsAuthentication : IFormsAuthentication
  {
    /// <summary>
    /// Set Authentication Cookie
    /// </summary>
    /// <param name="userName">Username to save</param>
    /// <param name="persistent">Persist on exit</param>
    public void SetAuthCookie(string userName, bool persistent)
    {
      FormsAuthentication.SetAuthCookie(userName, persistent);
    }

    /// <summary>
    /// Clears all cookies for this particular session.
    /// </summary>
    public void Signout()
    {
      FormsAuthentication.SignOut();
    }

    /// <summary>
    /// Set authencation cookie
    /// </summary>
    /// <param name="httpContext">HTTP Base Context</param>
    /// <param name="authenticationTicket">Predetermine authentication ticket</param>
    public void SetAuthCookie(HttpContextBase httpContext, FormsAuthenticationTicket authenticationTicket)
    {
      var encryptedTicket = FormsAuthentication.Encrypt(authenticationTicket);
      httpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) { Expires = CalculateCookieExpirationDate() });
    }

    /// <summary>
    /// Set authentication cookie
    /// </summary>
    /// <param name="httpContext">HTTP Context</param>
    /// <param name="authenticationTicket">Predetermined authentication ticket</param>
    public void SetAuthCookie(HttpContext httpContext, FormsAuthenticationTicket authenticationTicket)
    {
      var encryptedTicket = FormsAuthentication.Encrypt(authenticationTicket);
      httpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket) { Expires = CalculateCookieExpirationDate() });
    }

    /// <summary>
    /// Calculate the expiration of the cookie
    /// </summary>
    /// <returns>Calculated DateTime object</returns>
    private static DateTime CalculateCookieExpirationDate()
    {
      return DateTime.Now.Add(FormsAuthentication.Timeout);
    }

    /// <summary>
    /// Decrypt the authentication ticket
    /// </summary>
    /// <param name="encryptedTicket">Encrypted authentication ticket.</param>
    /// <returns></returns>
    public FormsAuthenticationTicket Decrypt(string encryptedTicket)
    {
      return FormsAuthentication.Decrypt(encryptedTicket);
    }
  }
}