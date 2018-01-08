using Serilog;
using System;



namespace Atlas.ACCPAC.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class UploadGLTransactions:  global::Quartz.IJob
  {
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      try
      {
        _log.Information("UploadGLTransactions Quartz task starting");
        var myUpload_GL_Transactions = new Atlas.ACCPAC.BLL.Accpac.GL.Upload_GL_Transactions();
        myUpload_GL_Transactions.StartProcess();
        _log.Information("UploadGLTransactions Quartz task completed");
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }
    }


    #region Private vars

    // Log4net
    private static readonly ILogger _log = Log.Logger.ForContext<UploadGLTransactions>();

    #endregion

  }
}
