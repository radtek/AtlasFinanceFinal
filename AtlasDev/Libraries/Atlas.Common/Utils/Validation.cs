/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Basic validation routines
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;


namespace Atlas.Common.Utils
{

  /// <summary>
  /// Common validation routines
  /// </summary>
  public static class Validation
  {
    // The general consensus is that the best way to validate an e-mail is to use the 
    // .NET MailAddress class (supports 'DisplayName' in address, whereas a RegEx does not...)
    public static bool IsValidEmail(string eMailAddress)
    {
      // Do very basic e-mail validation
      if (string.IsNullOrEmpty(eMailAddress) || eMailAddress.IndexOf("@") == -1 || eMailAddress.Length < 3)
        return false;

      eMailAddress = eMailAddress.Trim();
      try
      {
        var address = new System.Net.Mail.MailAddress(eMailAddress);
        return !string.IsNullOrEmpty(address.Address);
      }
      catch (FormatException)
      {
        return false;
      }
    }
    

    public static bool IsBlank(string inStr, string caption, out string errorMsg)
    {
      errorMsg = string.Empty;
      if (inStr.Trim() != string.Empty)
        return false;

      errorMsg = string.Format("Please specify a {0}.", caption);
      return true;
    }


    public static bool IsNumeric(string inStr)
    {
      double dResult;
      return double.TryParse(inStr, System.Globalization.NumberStyles.Any, null, out dResult);
    }

  }
}
