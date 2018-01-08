/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Base64 Helper functions 
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-09-03 - Initial revision
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.IO;


namespace Atlas.Common.Utils
{
  public static class Base64
  {
    public static string Encode(string fileName)
    {
      byte[] filebytes= null;

      using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      {
        filebytes = new byte[fileStream.Length];
        fileStream.Read(filebytes, 0, Convert.ToInt32(fileStream.Length));
      }
      return Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
    }


    public static string EncodeString(byte[] compressedStr)
    {
      return Convert.ToBase64String(compressedStr, Base64FormattingOptions.InsertLineBreaks);
    }


    public static byte[] DecodeString(string encodedString)
    {
      return Convert.FromBase64String(encodedString);
    }


    public static string Base64Encode(string plainText)
    {
      var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
      return Convert.ToBase64String(plainTextBytes);
    }


    public static string Base64Decode(string base64EncodedData)
    {
      var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
      return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

  }
}
