using System;
using System.IO;
using System.Collections.Generic;

using SocialExplorer.IO.FastDBF;
using Serilog;


namespace Atlas.Services.DbfGenerate.QuartzTasks.Utils
{
  internal class CreateTemplateBranch
  {
    internal static void Execute(ILogger log, ICollection<string> masterTables, string dbfDir, string sourceBranch, string destFolder)
    {
      try
      {
        log.Information("Creating blank source branch");       
        if (!Directory.Exists(destFolder))
        {
          Directory.CreateDirectory(destFolder);
        }

        var sourcePath = Path.Combine(dbfDir, sourceBranch);
        if (Directory.Exists(sourcePath))
        {
          var files = Directory.GetFiles(sourcePath, "*.dbf");

          foreach (var dbfFile in files)
          {
            var tableName = Path.GetFileNameWithoutExtension(dbfFile).ToLower();
            var destFilename = Path.Combine(destFolder, Path.GetFileName(dbfFile));

            if (!masterTables.Contains(tableName)) // not a master- create an empty clone
            {
              var tempFilename = Path.Combine(destFolder, string.Format("{0}.tmp", Path.GetFileName(dbfFile)));
              File.Copy(dbfFile, tempFilename, true);

              var src = new DbfFile(System.Text.Encoding.ASCII);
              src.Open(tempFilename, FileMode.Open);

              if (File.Exists(destFilename))
              {
                File.Delete(destFilename);
              }

              var dest = new DbfFile(System.Text.Encoding.ASCII);
              dest.Open(destFilename, FileMode.Create);
              for (var i = 0; i < src.Header.ColumnCount; i++)
              {
                dest.Header.AddColumn(src.Header[i].Name, src.Header[i].ColumnType, src.Header[i].Length, src.Header[i].DecimalCount);
              }
              dest.WriteHeader();
              dest.Close();

              src.Close();
              File.Delete(tempFilename);
            }
            else
            {
              if (File.Exists(destFilename))
              {
                File.Delete(destFilename);
              }
              File.Copy(dbfFile, destFilename);
            }
          }
        }
        else
        {
          log.Error("Missing source branch directory: {0}", sourcePath);
        }
      }
      catch (Exception err)
      {
        log.Error(err, "CreateTemplateBranch.Execute");
      }
    }
  }
}
