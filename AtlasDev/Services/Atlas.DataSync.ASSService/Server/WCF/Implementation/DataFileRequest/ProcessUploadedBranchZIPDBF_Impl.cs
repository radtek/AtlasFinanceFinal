using System;
using System.IO;
using System.Threading.Tasks;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using ASSServer.WCF.Implementation.DataFileRequest.Tasks;
using Atlas.Cache.Interfaces.Classes;


namespace ASSServer.WCF.Implementation.DataFileRequest
{ 
  public static class ProcessUploadedBranchZIPDBF_Impl
  {
    public static ProcessStatus Execute(ILogging log, ICacheServer cache, IConfigSettings config,
      SourceRequest sourceRequest, string clientTransactionId, string fileName)
    {
      var methodName = "ProcessUploadedBranchPSQL";
    
      #region Check parameters
      ASS_BranchServer_Cached server;

      string errorMessage;
      if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
      {
        log.Error(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
        return new ProcessStatus() { ErrorMessage = errorMessage, Status = ProcessStatus.CurrentStatus.Failed };
      }
      if (server.Branch == null)
      {
        var error = new ArgumentNullException("LegacyBranchNum");
        log.Error(error, "{MethodName}- {@Request}", methodName, sourceRequest);
        return new ProcessStatus() { ErrorMessage = error.Message, Status = ProcessStatus.CurrentStatus.Failed };
      }
      var branch = cache.Get<BRN_Branch_Cached>(server.Branch.Value);
      var legacyBranchNum = branch.LegacyBranchNum.PadLeft(3, '0');
      Guid guid;
      if (string.IsNullOrWhiteSpace(clientTransactionId) || !Guid.TryParse(clientTransactionId, out guid))
      {
        var error = new ArgumentNullException("clientTransactionId");
        log.Error(error, "{MethodName}- {@Request}", methodName, sourceRequest);
        return new ProcessStatus() { ErrorMessage = error.Message, Status = ProcessStatus.CurrentStatus.Failed };
      }
      var fullName = Path.Combine(config.GetCustomSetting("", "DataSyncPath", false), fileName);
      if (!File.Exists(fullName))
      {
        var error = new Exception(string.Format("Specified file cannot be located: '{0}'", fileName));
        log.Error(error, "{MethodName}- {@Request}", methodName, sourceRequest);
        return new ProcessStatus() { ErrorMessage = error.Message, Status = ProcessStatus.CurrentStatus.Failed };
      }
      #endregion

      ProcessTracking.CurrentStatus status;
      string currentFile;
      if (!ProcessTracking.GetTransactionState(clientTransactionId, out status, out errorMessage, out currentFile))
      {
        ProcessTracking.SetTransactionState(clientTransactionId, ProcessTracking.CurrentStatus.Started, null, fileName);

        // Start thread to convert the ZIPPED DBFs to local PostgreSQL tables, using 'br' + branch number as the schema name (brXX)
        Task.Factory.StartNew(() =>
          ConvertZIPDBFToPSQL.Execute(log, cache, config, server, legacyBranchNum, clientTransactionId),
          TaskCreationOptions.LongRunning);

        return new ProcessStatus() { Status = ProcessStatus.CurrentStatus.Started, TransactionId = clientTransactionId };
      }
      else
      {
        return new ProcessStatus() { Status = WCFStatusUtils.StatusToWCFStatus(status), ErrorMessage = errorMessage, Filename = fileName };
      }
    }

  }
}
