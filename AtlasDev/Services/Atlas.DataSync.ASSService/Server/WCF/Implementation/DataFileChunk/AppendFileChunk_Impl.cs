using System;
using System.IO;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;


namespace ASSServer.WCF.Implementation.DataFileChunk
{
  public static class AppendFileChunk_Impl
  {
    public static bool Execute(ILogging log, IConfigSettings config, SourceRequest sourceRequest, string fileName, byte[] buffer)
    {
      var methodName = "AppendFileChunk";     

      #region Check parameters
      //ASS_BranchServerDTO server;
      //string errorMessage;
      //if (!Utils.Checks.CheckSourceRequest(sourceRequest, out server, out errorMessage))
      //{
      //  log.Error(methodName, new Exception(errorMessage));
      //  return false;
      //}
      if (!FileUtils.FileNameValid(fileName))
      {
        log.Error(new ArgumentException(string.Format("Invalid value: '{0}'", fileName), "fileName"), methodName);
        return false;
      }

      if (buffer == null || buffer.Length == 0)
      {
        log.Error(new ArgumentException(string.Format("Empty/zero length value: '{0}'", fileName), "buffer"), methodName);
        return false;
      }

      var fullPath = Path.Combine(config.GetCustomSetting("", "DataSyncPath", false), fileName);
      if (File.Exists(fullPath))
      {
        var fileInfo = new FileInfo(fullPath);
        if (fileInfo.Length > 500 * 1024 * 1024)
        {
          log.Error(new ArgumentException(string.Format("File cannot exceed 500MB: '{0}'", fileName), "buffer"), methodName);
          return false;
        }
      }

      #endregion

      try
      {
        using (var file = File.Open(fullPath, FileMode.Append, FileAccess.Write, FileShare.None))
        {
          file.Write(buffer, 0, buffer.Length);
        }
        return true;
      }
      catch (Exception err)
      {        
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        return false;
      }
    }
  }
}
