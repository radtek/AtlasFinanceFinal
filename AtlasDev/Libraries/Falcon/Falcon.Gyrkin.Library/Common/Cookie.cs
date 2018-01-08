using Atlas.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Falcon.Gyrkin.Library.Common
{
  public static class CookieHelper
  {
    /// <summary>
    /// Returns value of saved cookie
    /// </summary>
    /// <param name="controllerContext">Calling context</param>
    /// <param name="keyName">Name of cookie</param>
    /// <returns></returns>
    public static string GetCookieValue(HttpContext context, string keyName)
    {
      if (context.Request.Cookies.AllKeys.Contains(keyName))
      {
        return context.Request.Cookies[keyName].Value;
      }
      return string.Empty;
    }

    /// <summary>
    /// Returns secured value of saved cookie
    /// </summary>
    /// <param name="controllerContext">Calling context</param>
    /// <param name="keyName">Name of cookie</param>
    /// <returns></returns>
    public static string SecureGetCookieValue(HttpContext context, string keyName)
    {
      if (context.Request.Cookies.AllKeys.Contains(keyName))
      {
        if (!string.IsNullOrEmpty(context.Request.Cookies[keyName].Value))
        {
          return Cryptography.Decrypt(context.Request.Cookies[keyName].Value, "r2320938rm193f0r4irjifj8fu", "42rerwr238h4971h348923gh4981723g4");
        }
      }
      return string.Empty;
    }

    /// <summary>
    /// Store cookie
    /// </summary>
    /// <param name="controllerContext">Calling context</param>
    /// <param name="keyName">Name of cookie</param>
    /// <param name="keyValue">Values to store</param>
    /// <param name="expireDate">Expiration date of cookie</param>
    /// <returns></returns>
    public static bool Store(HttpContext context, string keyName, string keyValue, DateTime expireDate, Action<string> act)
    {
      HttpCookie cookie = new HttpCookie(keyName);

      if (!string.IsNullOrEmpty(keyValue))
      {
        cookie.Value = keyValue;
        cookie.Expires = expireDate;
      }


      act(keyValue);

      context.Response.Cookies.Add(cookie);

      return true;
    }

    /// <summary>
    /// Secure store values of cookie
    /// </summary>
    public static bool SecureStore(HttpContext context, string keyName, string keyValue, DateTime expireDate, Action<string> act)
    {
      HttpCookie cookie = new HttpCookie(keyName);

      if (!string.IsNullOrEmpty(keyValue))
      {
        cookie.Value = Cryptography.Encrypt(keyValue, "r2320938rm193f0r4irjifj8fu", "42rerwr238h4971h348923gh4981723g4"); 
        cookie.Expires = expireDate;
      }

      act(keyValue);
      context.Response.Cookies.Add(cookie);

      return true;
    }

    /// <summary>
    /// Remove cookie
    /// </summary>
    /// <param name="controllerContext">Calling context</param>
    /// <param name="keyName">Name of cookie</param>
    /// <returns></returns>
    public static bool Remove(HttpContext context, string keyName)
    {
      if (context.Request.Cookies.AllKeys.Contains(keyName))
      {
        HttpCookie cookie = context.Request.Cookies[keyName];
        cookie.Expires = DateTime.Now.AddDays(-1);
        context.Response.Cookies.Add(cookie);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Clean up all cookies
    /// </summary>
    /// <param name="context">Calling context</param>
    public static void CleanCookies(HttpContext context)
    {
      var cookieCollection = context.Request.Cookies.AllKeys.ToList();
      foreach(var cookie in cookieCollection)
      {
        if(cookie.Contains(CookieConst.FALCON_COOKIE_PREFIX))
        {
          Remove(context, cookie);
        }
      }
    }

    public static Tuple<string, string> HashPassword(string password)
    {
      Tuple<string, string> hashedValue = Cryptography.HashPassword(password);
      return hashedValue;
    }
  }
}