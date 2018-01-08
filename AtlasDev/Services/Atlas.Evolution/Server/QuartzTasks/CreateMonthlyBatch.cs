using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using DevExpress.Xpo;
using Quartz;

using Atlas.Domain.Model.Evolution;
using Atlas.Domain.Model;
using Atlas.Evolution.Server.Code.Batch;
using Atlas.Common.Interface;


namespace Atlas.Evolution.Server.QuartzTasks
{
  internal class CreateMonthlyBatch : IJob
  {
    public CreateMonthlyBatch(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      var methodName = $"{nameof(CreateMonthlyBatch)}.{nameof(Execute)}";

      _log.Information("[{MethodName}]- Starting", methodName);

      // Read configs
      var createFinalFiles = _config.GetCustomSettingBool("", "CreateFinalFiles", false, false);// AppConfig.CreateFinalEvolutionFiles;
      var srn = _config.GetCustomSetting("", "SRN", false);//  AppConfig.SRN;

      // We run for the previous month
      var collateDate = DateTime.Now;
      var lastMonth = DateTime.Today.AddMonths(-1);
      var monthStart = new DateTime(lastMonth.Year, lastMonth.Month, 1);
      var monthEnd = new DateTime(lastMonth.Year, lastMonth.Month, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month));
      _log.Information("[{MethodName}]- Processing for period: {Start:yyyy-MM-dd} - {End:yyyy-MM-dd}...", methodName, monthStart, monthEnd);

      try
      {
        // Create the batch in the DB
        var batchId = DbCreateMonthlyBatch.CreateBatch(_log, _config, createFinalFiles, collateDate, monthStart, monthEnd);

        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempPath);
        var tempFilenames = new List<string>();

        using (var uow = new UnitOfWork())
        {
          // Create batch file        
          var batch = uow.Query<EVO_UploadBatch>().First(s => s.UploadBatchId == batchId);
          var tempFile = FileCreateMonthly.CreateFile(_log, _config, srn, collateDate, monthEnd, tempPath, batchId);

          if (!string.IsNullOrEmpty(tempFile))
          {
            var batchFilename = string.Format("{0}_ALL_{1}702_M_{2:yyyyMMdd}_1_1.txt", srn, createFinalFiles ? "L" : "T", monthEnd);
            batch.StorageSystemRef = BatchUtils.MergeFilesAndAddToMongo(_log, batchFilename, tempPath, batchId);
            batch.StorageFileName = batchFilename;
            uow.CommitChanges();
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "[{MethodName}]", methodName);
      }

      _log.Information("[{MethodName}]- Completed", methodName);
    }


    private readonly ILogging _log;
    private readonly IConfigSettings _config;
  }
}
