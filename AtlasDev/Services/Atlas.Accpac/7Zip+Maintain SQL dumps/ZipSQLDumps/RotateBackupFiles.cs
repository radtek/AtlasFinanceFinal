using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;


namespace ZipSQLDumps
{
  public static class RotateBackupFiles
  {
    /// <summary>
    /// Deletes unnecessary 7z files. Keeps last 'daysToKeep' days, and 'weeksToKeep' weekly backup on a Sun/Sat/Fri/Thu
    /// </summary>
    /// <param name="fileDir">Base folder</param>
    /// <param name="daysToKeep">How many days to keep rolling backup</param>
    /// <param name="weeksToKeep">How many weeks to keep rolling backup (Sat)</param>
    /// <returns></returns>
    public static string Execute(string fileDir, int daysToKeep, int weeksToKeep)
    {
      var errors = new StringBuilder();

      try
      {
        var paths = Directory.GetDirectories(fileDir, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var path in paths)
        {
          #region Get the files
          var files = Directory.GetFiles(path, "*.7z", SearchOption.TopDirectoryOnly);
          var filesInfo = new List<FileInfo>();
          foreach (var sqlDumpFile in files)
          {
            filesInfo.Add(new FileInfo(sqlDumpFile));
          }
          #endregion

          if (filesInfo.Count > 0)
          {
            // Files which match last x days
            var dailyOK = filesInfo.Where(s => DateTime.Now.Subtract(s.CreationTime).TotalDays <= daysToKeep)
                .Select(s => s.FullName)
                .ToList();

            #region Weekly files to keep
            var weeklyOK = new List<string>();
            var currDay = DayOfWeek.Sunday;
            var noFiles = false;
            do
            {
              weeklyOK = filesInfo.Where(s => s.CreationTime.DayOfWeek == currDay &&
                                              DateTime.Now.Subtract(s.CreationTime).TotalDays <= (weeksToKeep * 7))
              .Select(s => s.FullName)
              .ToList();

              switch (currDay)
              {
                case DayOfWeek.Sunday:
                  currDay = DayOfWeek.Saturday;
                  break;

                case DayOfWeek.Saturday:
                  currDay = DayOfWeek.Friday;
                  break;

                case DayOfWeek.Friday:
                  currDay = DayOfWeek.Thursday;
                  break;

                default:
                  errors.AppendFormat("No weekly backups found for path: '{0}'\r\n", path);
                  noFiles = true;
                  break;
              }

            } while (!noFiles && weeklyOK.Count == 0);
            #endregion

            #region Delete files neither on the daily or weekly list
            foreach (var file in filesInfo)
            {
              if (dailyOK.IndexOf(file.FullName) == -1 && weeklyOK.IndexOf(file.FullName) == -1)
              {
                try
                {
                  File.Delete(file.FullName);
                }
                catch (Exception delErr)
                {
                  errors.AppendFormat("Failed to delete file '{0}', error: [{1}]\r\n", file, delErr);
                }
              }
            }
            #endregion
          }
        }
      }
      catch (Exception err)
      {
        errors.AppendFormat("Unexpected error in [RotateBackFiles.Execute]: '{0}'\r\n", err.Message);
      }

      return errors.ToString();
    }

  }
}
