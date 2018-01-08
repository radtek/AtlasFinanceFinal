using System;

using Atlas.Common.Utils;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class ASSUtils
  {
    internal static string ASSEncrypt(string data)
    {
      return ClipperCrypto.ASSCommsEncrypt(data, "ASS1.0BYCL");
    }


    /// <summary>
    /// Converts string '1 of 3' to its X and Y values, i.e. '1 of 3' => 'xVal' = 1, 'yVal' = 3
    /// </summary>
    /// <param name="value">String with pattern: 'x of y'</param>
    /// <param name="xVal">Resultant x value (0-36)</param>
    /// <param name="yVal">Resultant y value (0-36)</param>
    /// <returns>true if successfully parsed, false if does meet formatting criteria</returns>
    internal static bool GetXOfY(string value, out Int16 xVal, out Int16 yVal)
    {
      xVal = 0;
      yVal = 0;

      var words = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      if (words.Length == 3)
      {
        var result = Int16.TryParse(words[0], out xVal) && Int16.TryParse(words[2], out yVal);
        return result;
      }

      return false;
    }

  }
}
