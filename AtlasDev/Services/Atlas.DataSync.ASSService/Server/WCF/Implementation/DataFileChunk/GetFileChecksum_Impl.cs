using System;
using System.IO;
using System.Security.Cryptography;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;


namespace ASSServer.WCF.Implementation.DataFileChunk
{
  internal static class GetFileChecksum_Impl
  {
    internal static string Execute(ILogging log, IConfigSettings config, SourceRequest sourceRequest, string fileName, string method)
    {
      var methodName = "GetFileChecksum_Impl.Execute";

      #region Check params      
      if (!FileUtils.FileNameValid(fileName))
      {
        log.Error(new ArgumentException(string.Format("Invalid value: '{0}'", fileName), "fileName"), methodName);
        return null;
      }

      var fullPath = Path.Combine(config.GetCustomSetting("", "DataSyncPath", false), fileName);
      if (!File.Exists(fullPath))
      {
        log.Error(new ArgumentException(string.Format("File missing: '{0}'", fileName), "fileName"), methodName);
        return null;
      }
      #endregion

      switch (method.ToUpper())
      {
        case "SHA-256":
        case "SHA256":
          var hashValue = GetSHA256Checksum(fullPath);
          log.Information("{File} {Checksum}", fullPath, hashValue);
          return hashValue;

        default:
          log.Error("Unknown hash type requested {HashType}", method);
          return null;
      }
    }


    /// <summary>
    /// SHA256 hexadecimal hash value for a file
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private static string GetSHA256Checksum(string file)
    {
      using (FileStream stream = File.OpenRead(file))
      {
        using (var sha = new SHA256Managed())
        {
          byte[] checksum = sha.ComputeHash(stream);
          return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }
      }
    }
        
  }
}
