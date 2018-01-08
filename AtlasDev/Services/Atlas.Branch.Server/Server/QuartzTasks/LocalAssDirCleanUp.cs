/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    Deletes old ASS work files
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *    2014-02-28- Created
 
 * 
 *  Comments:
 *  ------------------
 *   
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.IO;
using Quartz;
using Serilog;


namespace ASSSyncClient.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class LocalAssDirCleanUp : IJob
  {
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "LocalAssDirCleanUp.Execute";
      _log.Information("LocalAssDirCleanUp.Execute Starting", methodName);
      try
      {
        var deleteCount = 0;
        var deleteFiles = new List<string>();
        var sourceDir = "C:\\LOAN\\FILES";
        if (Directory.Exists(sourceDir))
        {          
          var files = Directory.GetFiles(sourceDir);
          foreach(var fileName in files)
          {
            #region Remove work files
            var fileOnly = Path.GetFileNameWithoutExtension(fileName).ToUpper();
            if (fileOnly.StartsWith("WK") || fileOnly.StartsWith("WRK"))
            {
              var fileInfo = new FileInfo(fileName);
              if (DateTime.Now.Subtract(fileInfo.CreationTime) > TimeSpan.FromDays(2))
              {
                deleteFiles.Add(fileName);
              }
            }
            #endregion

            #region Remove any old ASS*.exe's, except ASS.EXE
            var extension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(extension))
            {
              extension = extension.Trim('.').ToUpper();

              if (fileOnly.StartsWith("ASS") && fileOnly.Length > 3 /* don't delete ASS.exe */ && extension == "EXE")
              {
                var fileInfo = new FileInfo(fileName);
                if (DateTime.Now.Subtract(fileInfo.CreationTime) > TimeSpan.FromDays(2))
                {
                  deleteFiles.Add(fileName);
                }
              }
            #endregion
            }
          }

          foreach(var fileToDelete in deleteFiles)
          {
            try
            {
              File.Delete(fileToDelete);
              deleteCount++;
            }
            catch(Exception err)
            {
              _log.Error(err, "Failed to delete {File}", methodName, fileToDelete);
            }
          }
        }

        _log.Information("Execute Completed- Deleted {DeleteCount} file(s)", methodName, deleteCount);
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
      }

      _log.Information("{MethodName} completed", methodName);
    }
    

    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<LocalAssDirCleanUp>();

    #endregion

  }

}
