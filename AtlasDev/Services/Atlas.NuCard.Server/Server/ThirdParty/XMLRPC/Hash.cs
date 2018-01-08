/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-04-19- Skeleton created
 * 
 *     2014-08-14- Simplified from params to string, as each class formats the message appropriately
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

#endregion


namespace Atlas.ThirdParty.XMLRPC.Utils
{
  public class Hash
  {
    /// <summary>
    /// Builds a HMAC-SHA1 hex hash for provided parameters (supports only string, int, boolean and DateTime parameter types)
    /// </summary>
    /// <param name="key">HMAC-SHA1 key for hash</param>
    /// <param name="args">The value to hash</param>
    /// <returns>Hex HMA-SHA1 hex string</returns>
    /// <author>Keith Blows</author>
    /// <lastrevision>2014-08-14</lastrevision>
    public static string GetHMACSHA1(string key, string value)
    {
      #region Calculate the hex HMAC-SHA1 of data
      byte[] keyBytes = Encoding.ASCII.GetBytes(key);
      using (var hmac = new HMACSHA1(keyBytes))
      {
        var data = Encoding.ASCII.GetBytes(value);
        var hashValue = hmac.ComputeHash(data);

        var hexHash = new StringBuilder(hashValue.Length * 2);
        foreach (var ch in hashValue)
        {
          hexHash.AppendFormat("{0:x2}", ch);
        }

        return hexHash.ToString();
      }
      #endregion
    }
  }
}
