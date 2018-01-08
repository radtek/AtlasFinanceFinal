using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DevExpress.Xpo;
using Quartz;

using Atlas.Domain.Model.Evolution;
using Atlas.Domain.Model;
using Atlas.Evolution.Server.Code.Batch;
using Atlas.Common.Interface;


namespace Atlas.Evolution.Server.QuartzTasks
{
  /// <summary>
  /// Create daily batch
  /// </summary>
  internal class CreateDailyBatch : IJob
  {
    public CreateDailyBatch(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      var methodName = $"{nameof(CreateDailyBatch)}.{nameof(Execute)}";
      var snapshotDate = DateTime.Now.Hour < 18 ? DateTime.Today.Subtract(TimeSpan.FromDays(2)) : DateTime.Today.Subtract(TimeSpan.FromDays(1));
      Run(snapshotDate);

      //var snapshotDate = DateTime.Today.Subtract(TimeSpan.FromDays(2));
      //while (snapshotDate < DateTime.Today.Subtract(TimeSpan.FromDays(1)))
      //{                  
      //  Run(snapshotDate);
      //  snapshotDate = snapshotDate.AddDays(1);
      //}      
    }


    private void Run(DateTime snapshotDate)
    {
      var methodName = $"{nameof(CreateDailyBatch)}.{nameof(Run)}";
      _log.Information("[{MethodName}]- Starting for {snapshotDate}", methodName, snapshotDate);
      // Read configs
      var createFinalFiles = _config.GetCustomSettingBool("", "CreateFinalFiles", false, false); //CreateFinalEvolutionFiles;
      var srn = _config.GetCustomSetting("", "SRN", false); // AppConfig.SRN;

      var batchId = DbCreateDailyBatch.CreateBatch(_log, _config, snapshotDate, createFinalFiles, srn);
      if (batchId > 0)
      {
        using (var uow = new UnitOfWork())
        {
          var branches = uow.Query<BRN_Branch>().OrderBy(s => s.LegacyBranchNum).ToList();
          var batch = uow.Query<EVO_UploadBatch>().First(s => s.UploadBatchId == batchId);

          #region Create batch files per branch
          var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
          Directory.CreateDirectory(tempPath);
          var tempFilenames = new List<string>();
          foreach (var branch in branches.Select(s => s.LegacyBranchNum.PadLeft(3, '0')))
          {
            var tempFile = FileCreateDaily.CreateFile(_log, _config, srn, snapshotDate, branch, tempPath, batchId);
            if (!string.IsNullOrEmpty(tempFile))
            {
              tempFilenames.Add(tempFile);
            }
          }
          #endregion

          #region Merge files and add to mongo
          if (tempFilenames.Any())
          {
            // Rename files to required format, with sequence           
            var batchFilename = string.Format("{0}_ALL_{1}702_D_{2:yyyyMMdd}_1_1.txt", srn, createFinalFiles ? "L" : "T", snapshotDate);
            batch.StorageSystemRef = BatchUtils.MergeFilesAndAddToMongo(_log, batchFilename, tempPath, batch.UploadBatchId);
            batch.StorageFileName = Path.GetFileName(batchFilename);
            uow.CommitChanges();
          }
          #endregion
        }

        _log.Information("[{MethodName}]- Completed", methodName);
      }
    }

    private readonly ILogging _log;
    private readonly IConfigSettings _config;
  }

}
