using System;
using System.IO;
using System.Text;
using System.Diagnostics;

using Serilog;


namespace ZipSQLDumps
{
  class ZipSQLDumpFiles
  {
    /// <summary>
    /// 7Zips all files in given 'srcDir', outputting them to 'destDir' (appending last directory)
    /// </summary>
    /// <param name="srcDir">Source folder</param>
    /// <param name="destDir">Destination folder</param>
    /// <returns>Any errors, empty string if no errors</returns>
    public static string Execute(string srcDir, string destDir)
    {
      var errors = new StringBuilder();

      #region Ensure 7-zip installed
      var sevenZipPath = Path.Combine(Environment.GetEnvironmentVariable("ProgramW6432"), "7-Zip", "7z.exe");
      if (!File.Exists(sevenZipPath))
      {
        Log.Information("The 7zip exe missing- expected at location '{0}'", sevenZipPath);
        throw new Exception(string.Format("The 7zip exe missing- expected at location '{0}'", sevenZipPath));
      }
      #endregion

      try
      {
        // Handle directories
        var paths = Directory.GetDirectories(srcDir, "*.*", SearchOption.TopDirectoryOnly);
        foreach (var path in paths)
        {
          var files = Directory.GetFiles(path, "*.bak", SearchOption.TopDirectoryOnly);

          foreach (var sqlDumpFile in files)
          {
            var lpos1 = -1;
            var lpos2 = -1;
            lpos1 = sqlDumpFile.LastIndexOf(Path.DirectorySeparatorChar); // Search backward for last backslash
            if (lpos1 > -1)
            {
              lpos2 = sqlDumpFile.LastIndexOf(Path.DirectorySeparatorChar, lpos1 - 1); // Search backward to get next backslash
            }

            if (lpos2 > -1 && lpos1 > -1)
            {
              #region Ensure destination directory exists
              var dbName = sqlDumpFile.Substring(lpos2 + 1, lpos1 - lpos2 - 1);
              var dest7ZDir = Path.Combine(destDir, dbName);
              if (!Directory.Exists(dest7ZDir))
              {
                Directory.CreateDirectory(dest7ZDir);
              }
              #endregion

              var dest7ZipFile = Path.Combine(dest7ZDir, Path.GetFileNameWithoutExtension(sqlDumpFile) + ".7z");

              #region 7zip it- Console is fast, simple and easy to use...
              if (!File.Exists(dest7ZipFile))
              {
                try
                {
                  //  a : add files
                  //  -mx3 : Fast compression mode                  
                  //  -t7z : 7Zip file format
                  //  -m0=lzma2 : use lzma2 (automatically mult-threaded)               
                  var run7Zip = new ProcessStartInfo(sevenZipPath, string.Format("a -mx3 -t7z -m0=lzma2 \"{0}\" \"{1}\"", dest7ZipFile, sqlDumpFile))
                  {
                    WindowStyle = ProcessWindowStyle.Hidden
                  };

                  using (var process = Process.Start(run7Zip))
                  {
                    process.WaitForExit();

                    var errCode = process.ExitCode;
                    if (errCode != 0)
                    {
                      Log.Error(new Exception(string.Format("Exit code: {0} on {1}", errCode, sqlDumpFile)), "Execute");
                      errors.AppendFormat("Error 7zipping file: '{0}', return code: {1}\r\n", sqlDumpFile, errCode);
                    }
                  }
                }
                catch (Exception zipErr)
                {
                  Log.Error(zipErr, "Start Process");
                  errors.AppendFormat("Unexpected zipping file '{0}', error: [{1}]\r\n", sqlDumpFile, zipErr.Message);
                }
              }
              #endregion
            }
          }
        }

      }
      catch (Exception err)
      {
        Log.Error(err, "Execute");
        errors.AppendFormat("Unexpected error in [ZipSQLDumpFiles.Execute]: '{0}'\r\n", err.Message);
      }
     
      return errors.ToString();
    }
  }
}
