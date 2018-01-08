/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Implementation of Fingerprint interface
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-12-03- Basic functionality started
 *
 * -----------------------------------------------------------------------------------------------------------------  */

using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model.Biometric;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class ExpiredFPUploadSessions : global::Quartz.IJob
  {
    public ExpiredFPUploadSessions(ILogging log)
    {
      _log = log;
    }


    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var sessionCount = 0;
      var bitmapCount = 0;
      var templateCount = 0;
      try
      {
        _log.Information("Expired FP upload session Quartz task starting");

        using (var unitOfWork = new UnitOfWork())
        {
          var deleteOlderThan = DateTime.Now.Subtract(new TimeSpan(0, 0, 60, 0)); // delete upload sessions older than 60 minutes

          var expired = unitOfWork.Query<BIO_UploadSession>().Where(s => s.StartDate < deleteOlderThan);

          foreach (var entry in expired)
          {
            sessionCount++;

            #region Bitmaps
            var bitmaps = unitOfWork.Query<BIO_UploadBitmap>().Where(s => s.FPUploadSession == entry);
            foreach (var bitmap in bitmaps)
            {
              bitmapCount++;
              bitmap.Delete();
            }
            #endregion

            #region Templates
            var templates = unitOfWork.Query<BIO_UploadTemplate>().Where(s => s.FPUploadSession == entry);
            foreach (var template in templates)
            {
              templateCount++;
              template.Delete();
            }
            #endregion

            entry.Delete();
          }

          unitOfWork.CommitChanges();
        }

        if (sessionCount > 0)
        {
          _log.Information("Expired FP upload session Quartz task completed- deleted {0} session{3}, {1} template{4}, {2} bitmap{5}",
            sessionCount, templateCount, bitmapCount,
            sessionCount > 1 ? "s" : string.Empty, templateCount > 1 ? "s" : string.Empty, bitmapCount > 0 ? "s" : string.Empty);
        }
        else
        {
          _log.Information("Expired FP upload session Quartz task completed");
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }
    }


    #region Private vars

    private readonly ILogging _log;
    
    #endregion

  }
}
