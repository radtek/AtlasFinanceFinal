using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;
using Quartz;
using Serilog;

using Atlas.Domain.Model;
using Atlas.ThirdParty.CompuScan.Batch;


namespace Atlas.Credit.Engine.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class CompuScanErrorTask : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<CompuScanErrorTask>();

    public void Execute(IJobExecutionContext context)
    {
      _log.Information("Job CompuScanErrorTask is executing...");

      List<long> _batchId = new List<long>();

      using (var uow = new UnitOfWork())
      {
        var batchSubmission = new XPQuery<BUR_BatchSubmission>(uow).Where(p => p.ErrorMessage == "Particular Job missing").ToList();

        if (batchSubmission.Count > 0)
        {
          _batchId.AddRange(batchSubmission.Select(p => p.Batch.BatchId));

          foreach(var bs in batchSubmission)
          {
            var items = new XPQuery<BUR_BatchSubmissionItem>(uow).Where(p => p.SubmissionBatch.BatchSubmissionId == bs.BatchSubmissionId);
            
            // Remove all items.
            foreach (var i in items)
              i.Delete();

            // Remove the submission batch
            bs.Delete();
          }
        }

        // Force wipe
        uow.PurgeDeletedObjects();

        uow.CommitChanges();

        var batches = new XPQuery<BUR_Batch>(uow).Where(p => _batchId.Contains(p.BatchId));

        foreach(var b in batches)
        {
          b.DeliveryDate = null;
          b.Locked = false;
          b.Save();
        }

        uow.PurgeDeletedObjects();

        uow.CommitChanges();

        BatchServletImpl batchImpl = new BatchServletImpl();
        batchImpl.DeliverBatch(true);
      }
      _log.Information("CompuScanErrorTask execution.");
    }
  }
}
