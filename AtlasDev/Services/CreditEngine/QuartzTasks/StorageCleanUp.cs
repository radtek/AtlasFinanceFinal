using System;
using System.Linq;

using DevExpress.Xpo;
using Quartz;
using Serilog;

using Atlas.Domain.Model;
using Atlas.ThirdParty.CompuScan.Batch;


namespace Atlas.Credit.Engine.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class StorageCleanUp : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<StorageCleanUp>();
    private static int DAY_ELAPSED = 120;
    public void Execute(IJobExecutionContext context)
    {
      _log.Information("Bureau Storage Cleanup is executing....");

      using (var uow = new UnitOfWork())
      {
        var bureauCollection = new XPQuery<BUR_Enquiry>(uow).Where(p => p.CreateDate < DateTime.Now.AddDays(DAY_ELAPSED * -1) && p.Storage.Count> 0).OrderBy(p => p.EnquiryId);

        _log.Information("Found {records} records to clean up", bureauCollection.Count().ToString());
        foreach(var bureau in bureauCollection)
        {
          _log.Information("Found enquiry {enquiry} that is {days} days old, removing storage object {object}", bureau.EnquiryId, Math.Round((DateTime.Now - (DateTime)bureau.CreateDate).TotalDays, MidpointRounding.ToEven),
            bureau.Storage.FirstOrDefault() != null ? bureau.Storage.FirstOrDefault().StorageId.ToString() : string.Empty);

          uow.Delete(bureau.Storage);
        }
        _log.Information("Purging records...");

        uow.PurgeDeletedObjects();


        _log.Information("Comitting changes...");
        uow.CommitChanges();
      }

      BatchServletImpl batchImpl = new BatchServletImpl();
      batchImpl.DeliverBatch(false);
      batchImpl = null;

      _log.Information("Bureau Storage Cleanup finished executing.");
    }
  }
}
