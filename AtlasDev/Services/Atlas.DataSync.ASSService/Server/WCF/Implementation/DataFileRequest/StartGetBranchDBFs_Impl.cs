using System;
using System.Threading.Tasks;

using Atlas.DataSync.WCF.Interface;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using ASSServer.Utils.PSQL;
using Atlas.Cache.DataUtils;


namespace ASSServer.WCF.Implementation.DataFileRequest
{
  public static class StartGetBranchDBFs_Impl
  {
    public static ProcessStatus Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest)
    {
      var methodName = "StartGetBranchDBFs";
      try
      {
        #region Check parameters
        /*ASS_BranchServerDTO server;
        string errorMessage;
        if (!Utils.Checks.VerifyBranchServerRequest(sourceRequest, out server, out errorMessage))
        {          
          log.Error("StartGetBranchDBFs", new Exception(errorMessage));
          return new ProcessStatus() { ErrorMessage = errorMessage, Status = ProcessStatus.CurrentStatus.Failed };
        }
        if (server.Branch == null)
        {
          var error = new ArgumentNullException("LegacyBranchNum");
          log.Error("StartGetBranchDBFs", error);
          return new ProcessStatus() { ErrorMessage = error.Message, Status = ProcessStatus.CurrentStatus.Failed };
        }*/
        var legacyBranchNum = sourceRequest.BranchCode.PadLeft(3, '0');
        #endregion

        var transactionId = Guid.NewGuid().ToString("N");

        // Start task to create branch DBFs and zip
        Task.Factory.StartNew(() =>
          ExportBranchToZipDBF.Execute(log, cache, config, legacyBranchNum, transactionId, CacheUtils.GetCurrentDbVersion(cache).DbUpdateScriptId),
          TaskCreationOptions.LongRunning);

        ProcessTracking.SetTransactionState(transactionId, ASSServer.WCF.ProcessTracking.CurrentStatus.Started);

        return new ProcessStatus() { Status = ProcessStatus.CurrentStatus.Started, TransactionId = transactionId };
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        return new ProcessStatus() { ErrorMessage = err.Message, Status = ProcessStatus.CurrentStatus.Failed };
      }
    }
  }
}
