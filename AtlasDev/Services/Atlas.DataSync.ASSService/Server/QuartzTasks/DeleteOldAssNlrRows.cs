using System;

using Quartz;
using Npgsql;

using Atlas.Common.Interface;


namespace ASSServer.QuartzTasks
{
  [DisallowConcurrentExecution]
  internal class DeleteOldAssNlrRows: IJob
  {
    public DeleteOldAssNlrRows(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute( IJobExecutionContext context)
    {
      var methodName = "DeleteAssRows.Execute";
      try
      {
        _log.Information("{MethodName} started", methodName);
        #region Delete old CS records from company
        using (var conn = new NpgsqlConnection(_config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {         
            var olderThan = DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-3);  
            cmd.CommandText = string.Format(
              "BEGIN;\r\n" +
              "DELETE FROM company.\"nlrbatb2\" WHERE \"create_dat\" < '{0:yyyyMMdd}';\r\n" +
              "DELETE FROM company.\"cs_respx\" WHERE \"proc_date\" < '{0:yyyyMMdd}';\r\n " +
              "COMMIT;", olderThan);

            cmd.CommandTimeout = (int)TimeSpan.FromHours(1).TotalSeconds;
            var rowsDeleted = cmd.ExecuteNonQuery();
            _log.Information("Deleted {RowsDeleted}rows from company.nlrbatb2/company.cs_respx", rowsDeleted);
          }
        }
        #endregion
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
      }

      _log.Information("{MethodName} completed", methodName);
    }
      

    #region Private vars

    /// <summary>
    /// Logging
    /// </summary>
    private readonly ILogging _log;
    private readonly IConfigSettings _config;

    #endregion

  }
}
