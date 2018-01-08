using System;
using System.Threading.Tasks;

using Atlas.DataSync.WCF.Interface;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;


namespace ASSServer.WCF.Implementation.DataFileRequest
{
  public static class StartGetBranchPSQL_Impl
  {
    public static ProcessStatus Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest)
    {
      var methodName = "StartGetBranchPSQL";
    
      try
      {
        #region Check parameters
        ASS_BranchServer_Cached server;        
        string errorMessage;
        if (!ASSServer.WCF.Implementation.Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
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
        #endregion

        var transactionId = Guid.NewGuid().ToString("N");

        // Start task to dump branch PSQL
        Task.Factory.StartNew(() =>
          Utils.PSQL.CreateBranchPSQLDump.Execute(log, cache, config, legacyBranchNum, transactionId),
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
