using System;
using System.Globalization;


namespace Atlas.Data.DBF.Utils
{
  /// <summary>
  /// dBASE utilities
  /// </summary>
  public static class DBFUtils
  {
    /// <summary>
    /// Converts an object to a valid DBF value string
    /// </summary>
    /// <param name="p">The object to convert</param>
    /// <param name="decimals">Number of decimals</param>
    /// <returns>string representation of object, which conforms to DBF on-disk format</returns>
    public static string DBFStringyfy(object p, byte decimals)
    {
      if (p == null || p == DBNull.Value)
      {
        return "";
      }
      if (p is string)
      {
        var val = p as string;
        return val;
      }
      else if (p is DateTime)
      {
        var val = p as DateTime?;
        return string.Format("{0:yyyy-MM-dd}", val);
      }
      else if (p is decimal)
      {
        var val = p as decimal?;
        return val.Value.ToString(string.Format("0.{0}", new string('0', decimals)), CultureInfo.InvariantCulture);
      }
      else if (p is float)
      {
        var val = p as float?;
        return val.Value.ToString(string.Format("0.{0}", new string('0', decimals)), CultureInfo.InvariantCulture);
      }
      else if (p is bool)
      {
        var val = p as bool?;
        return val.Value ? "true" : "false";
      }
      else
      {
        throw new Exception(string.Format("Unknown data type {0}", p.GetType().ToString()));
      }
    }

  }
}
