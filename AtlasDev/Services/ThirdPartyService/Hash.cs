using MassTransit;
using MassTransit.Log4NetIntegration;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using log4net;


namespace Atlas.ThirdParty.Service
{
  /// <summary>
  /// Generates a hash/checksum
  /// </summary>
  public static class Hash
  {
    public static string Generate(string username, string password)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}{1}{2}", username, password, Guid.NewGuid().ToString()));
      byte[] buffer = new SHA256Managed().ComputeHash(bytes);
      string str = string.Empty;

      foreach (byte num in buffer)
        str = str + string.Format("{0:x2}", num);
      
      return str;
    }

    public static string Generate(long personId, string cellNo, long controlId)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(string.Format("{0}{1}{2}{3}", personId, cellNo, controlId, DateTime.Now.Ticks));
      byte[] buffer = new SHA256Managed().ComputeHash(bytes);
      string str = string.Empty;

      foreach (byte num in buffer)
        str = str + string.Format("{0:x2}", num);

      return str;
    }

    public static string Generate(List<string> @params)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(@params.ToArray()));
      byte[] buffer = new SHA256Managed().ComputeHash(bytes);
      string str = string.Empty;

      foreach (byte num in buffer)
        str = str + string.Format("{0:x2}", num);
      
      return str;
    }
  }

  /// <summary>
  /// Verification of checksums
  /// </summary>
  public static class Verify
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(Verify));

    /// <summary>
    /// Extrapolate fields from supplied class
    /// </summary>
    private static  List<string> GetParams<T>(T toExtrapolate)
    {
      List<string> validationParams = new List<string>();

      if (typeof(T).BaseType == typeof(Structure.BaseStructure))
      {
        var props = typeof(T).GetProperties().OrderBy(t => t.Name);
        foreach (var prop in props)
        {
          var value = prop.GetValue(toExtrapolate, null);
          if (value == null)
            validationParams.Add(string.Empty);
          else
            validationParams.Add(value.ToString());
        }
      }
      else
        validationParams.Add(toExtrapolate.ToString());

      return validationParams;
    }
    
    /// <summary>
    /// Generate checksum based on supplied class
    /// </summary>
    public static string GenerateCheckSum<T>(T toGenerate)
    {
      return Hash.Generate(GetParams<T>(toGenerate));
    }

    /// <summary>
    /// Verify checksum on supplied class and checksum
    /// </summary>
    public static bool VerifyCheckSum<T>(string checkSum, T toVerify, string callingMethod = "")
    {
      string verificationCheckSum = Hash.Generate(GetParams(toVerify));

      if (checkSum == verificationCheckSum)
        return true;
      else
      {
        _log.Info(string.Format("[{0}] - Invalid checksum expected {1}, got {2}", callingMethod, verificationCheckSum, checkSum));
        return true;
      }
    }
  }
}
