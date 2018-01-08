using System;
using System.Configuration;
using System.IO;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;


namespace ASSServer.WCF.Implementation.DataFileChunk
{
  public static class GetFileSize_Impl
  {
    public static long Execute(ILogging log, IConfigSettings config, SourceRequest sourceRequest, string fileName)
    {
      var methodName = "GetFileSize";

      #region Check params
      /*ASS_BranchServerDTO server;      
      string errorMessage;
      if (!Utils.Checks.VerifyBranchServerRequest(sourceRequest, out server, out errorMessage))
      {
        log.Error(methodName, new Exception(errorMessage));
        return -1;
      }
      */
      if (!FileUtils.FileNameValid(fileName))
      {
        log.Error(new ArgumentException(string.Format("File missing: '{0}'", fileName), "fileName"), methodName);
        return -1;
      }
      #endregion

      var fullPath = Path.Combine(config.GetCustomSetting("", "DataSyncPath", false), fileName);
      if (File.Exists(fullPath))
      {
        var info = new FileInfo(fullPath);
        return info.Length;
      }
      else
      {
        log.Warning(new FileNotFoundException(string.Format("Requested file: '{0}', which does not exist locally", fullPath)), methodName);
        return -1;
      }
    }
  }
}
