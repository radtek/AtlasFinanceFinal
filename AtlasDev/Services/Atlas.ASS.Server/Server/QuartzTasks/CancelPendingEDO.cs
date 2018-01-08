using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;
using Quartz;

using Atlas.Common.Interface;
using Atlas.Domain.Model;
using Atlas.Server.WCF.Utils;


// namespaces...
namespace Atlas.WCF.QuartzTasks
{
  // public classes...
  public class CancelPendingEDO : IJob
  {
    public CancelPendingEDO(ILogging log)
    {
      _log = log;
    }


    // public methods...
    public void Execute(IJobExecutionContext context)
    {
      var methodName = $"{nameof(CancelPendingEDO)}.{nameof(Execute)}";
      _log.Information("{MethodName} starting", methodName);

      try
      {
        #region Get any EDOs, whose cancellation has not been confirmed
        var outstanding = new List<EDO>();
        using (var uow = new UnitOfWork())
        {
          outstanding = uow
            .Query<ALT_EDOContractCancel>()
            .Where(s => s.HandledOK == null && s.ContractNum != null)
            .Select(s => new EDO(s.EDOContractCancelId, s.EDOType, s.CreatedDT, s.AltechTransactionId, s.LastAttemptDT, s.ContractNum))
            .ToList();
        }
        #endregion

        if (outstanding.Any())
        {
          using (var uow = new UnitOfWork())
          {
            var prioritized = outstanding.Where(s => s.LastAttemptDT == null).ToList();                            // no cancel attempts- try them first
            prioritized.AddRange(outstanding.Except(prioritized.ToList()).ToList().OrderBy(s => s.LastAttemptDT));

            foreach (var item in prioritized)
            {
              var error = EDOUtils.CancelAllPendingEDOTransactionsFor(_log, item.EDOType, item.ContractRef, item.AltechTransactionId, null, false);
              var edoCancelRow = uow.Query<ALT_EDOContractCancel>().First(s => s.EDOContractCancelId == item.RecId);
              if (string.IsNullOrEmpty(error)) // success
              {
                edoCancelRow.CompletedDT = DateTime.Now;
                edoCancelRow.HandledOK = true;
              }
              else
              {
                edoCancelRow.LastAttemptDT = DateTime.Now;
                edoCancelRow.LastError = error;
                if (DateTime.Now.Subtract(edoCancelRow.CreatedDT).TotalDays >= 7) // we give up!
                {
                  edoCancelRow.HandledOK = false;
                }
              }
            }

            uow.CommitChanges();
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
      }

      _log.Information("{MethodName} completed");
    }



    private class EDO
    {

      // public constructors...
      public EDO(Int64 recId, string edoType, DateTime createdDT, Int64 transId, DateTime? lastAttemptDT, string contractRef)
      {
        RecId = recId;
        EDOType = edoType;
        CreatedDT = createdDT;
        AltechTransactionId = transId;
        LastAttemptDT = lastAttemptDT;
        ContractRef = contractRef;
      }

      public Int64 RecId { get; private set; }
      // public properties...
      public Int64 AltechTransactionId { get; private set; }
      public DateTime CreatedDT { get; private set; }
      public string EDOType { get; private set; }
      public DateTime? LastAttemptDT { get; private set; }
      public string ContractRef { get; private set; }
    }


    private readonly ILogging _log;

  }
}
