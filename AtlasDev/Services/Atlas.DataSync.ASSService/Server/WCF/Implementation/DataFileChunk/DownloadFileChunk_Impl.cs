using System;
using System.IO;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;


namespace ASSServer.WCF.Implementation.DataFileChunk
{
  public static class DownloadFileChunk_Impl
  {
    internal static byte[] Execute(ILogging log, IConfigSettings config, SourceRequest sourceRequest, string fileName, long offset, int chunkSize)
    {
      var methodName = "DownloadFileChunk";

      #region Check parameters
      //ASS_BranchServerDTO server;
      //string errorMessage;
      // if (!Utils.Checks.CheckSourceRequest(sourceRequest, out server, out errorMessage))
      // {
      //   log.Error(methodName, new Exception(errorMessage));
      //   return null;
      // }
      if (!FileUtils.FileNameValid(fileName))
      {
        log.Error(new ArgumentException(string.Format("Invalid value: '{0}'", fileName), "fileName"), methodName);
        return null;
      }

      if (chunkSize > 500 * 1024)
      {
        log.Error(new ArgumentException(string.Format("Parameter too large: '{0}'", chunkSize), "chunkSize"), methodName);
        return null;
      }

      var fullPath = Path.Combine(config.GetCustomSetting("", "DataSyncPath", false), fileName);
      if (!File.Exists(fullPath))
      {
        log.Error(new ArgumentException(string.Format("File missing: '{0}'", fileName), "fileName"), methodName);
        return null;
      }

      var fileInfo = new FileInfo(fullPath);
      if (fileInfo.Length < offset)
      {
        log.Error(new ArgumentException(string.Format("Invalid offset (exceeds file length): File length: '{0}', offset requested: '{1}'", fileInfo.Length, offset), "offset"), methodName);
        return null;
      }
      #endregion

      byte[] result = null;
      try
      {
        using (var file = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          result = new byte[chunkSize];
          file.Seek(offset, SeekOrigin.Begin);
          var bytesRead = file.Read(result, 0, chunkSize);
          if (bytesRead != chunkSize)
          {
            Array.Resize(ref result, bytesRead);
          }
        }

        return result;
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        return null;
      }
    }
  }
}
