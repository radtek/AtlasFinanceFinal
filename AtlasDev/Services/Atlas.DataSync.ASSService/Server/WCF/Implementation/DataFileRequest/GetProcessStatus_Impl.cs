using System;
using System.IO;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;


namespace ASSServer.WCF.Implementation.DataFileRequest
{ 
  public static class GetProcessStatus_Impl
  {
    public static ProcessStatus Execute(ILogging log, SourceRequest sourceRequest, string transactionId)
    {
      var methodName = "GetProcessStatus";
      
      try
      {
        string errorMessage;
        #region Check parameters
        /*//  This slows down the operation too much... removed for the moment
          
        ASS_BranchServerDTO server;
        string errorMessage;
        if (!Utils.Checks.VerifyBranchServerRequest(sourceRequest, out server, out errorMessage))
        {
          log.Error("GetProcessStatus", new Exception(errorMessage));
          return new ProcessStatus() { ErrorMessage = errorMessage, Status = ProcessStatus.CurrentStatus.Failed };
        }
        if (server.Branch == null)
        {
          var error = new ArgumentNullException("LegacyBranchNum");
          log.Error("GetProcessStatus", error);
          return new ProcessStatus() { ErrorMessage = error.Message, Status = ProcessStatus.CurrentStatus.Failed };
        }*/

        Guid guid;
        if (string.IsNullOrWhiteSpace(transactionId) || !Guid.TryParse(transactionId, out guid))
        {
          var error = new ArgumentNullException("transactionId");
          log.Error(error, "{MethodName}- {@Request}", methodName, sourceRequest);
          return new ProcessStatus() { ErrorMessage = error.Message, Status = ProcessStatus.CurrentStatus.Failed };
        }
        #endregion

        ProcessTracking.CurrentStatus status;
        string fileName;
        if (ProcessTracking.GetTransactionState(transactionId, out status, out errorMessage, out fileName))
        {
          if (!string.IsNullOrEmpty(fileName))
          {
            fileName = Path.GetFileName(fileName);
          }
          return new ProcessStatus() { TransactionId = transactionId, Status = WCFStatusUtils.StatusToWCFStatus(status), ErrorMessage = errorMessage, Filename = fileName };
        }
        else
        {
          return new ProcessStatus() { TransactionId = transactionId, Status = ProcessStatus.CurrentStatus.Failed, ErrorMessage = "Transaction could not be located" };
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        return new ProcessStatus() { ErrorMessage = err.Message, Status = ProcessStatus.CurrentStatus.NotSet };
      }
    }
  }
}
