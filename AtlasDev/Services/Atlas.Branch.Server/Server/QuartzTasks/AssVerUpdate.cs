using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Collections.Generic;

using Quartz;
using Serilog;

using ASSSyncClient.Utils;


namespace ASSSyncClient.QuartzTasks
{
  [DisallowConcurrentExecution]
  internal class AssVerUpdate : IJob
  {
    /// <summary>
    /// Run wyUpdate for ass and copy updated files to the ass install folder (C:\LOAN\Files\...)
    /// </summary>
    /// <param name="context">Quartz execution context</param>
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "AssVerUpdate.Execute";
      var wyUpdateEXE = Path.Combine(SOURCE_PATH, "wyupdate.exe");
      try
      {
        try
        {
          _log.Information("{MethodName} starting", methodName);

          #region Checks
          if (!Directory.Exists(SOURCE_PATH))
          {
            throw new Exception(string.Format("{0} Source wyUpdate directory missing: '{1}'", methodName, SOURCE_PATH));
          }

          if (!Directory.Exists(DEST_PATH))
          {
            throw new Exception(string.Format("{0} Destination Ass directory missing: '{1}'", methodName, DEST_PATH));
          }

          if (!File.Exists(wyUpdateEXE))
          {
            throw new Exception(string.Format("{0} File '{1}' missing ", methodName, wyUpdateEXE));
          }
          #endregion

          using (var wyUpdate = new Process())
          {
            // http://wyday.com/wybuild/help/wyupdate-commandline.php:
            //   /fromservice-  Run wyUpdate with the commandline switch alone to begin updating silently from a Windows Service
            //   -logfile="<filename>" When updating from a service you can write out the success or error message to a file. This can only be used with the "/fromservice" commandline switch; otherwise it's ignored:
            var logFile = string.Format("{0}wyupdate_{1:yyyy_MM_dd_HHmmss}.log\"", SOURCE_PATH, DateTime.Now);
            var cmdLineArgs = string.Format("/fromservice -logfile=\"{0}\"", logFile);

            _log.Information("{MethodName} Running {ExeName} with {Arguments}", methodName, wyUpdateEXE, cmdLineArgs);
            wyUpdate.StartInfo = new ProcessStartInfo(wyUpdateEXE, cmdLineArgs);
            wyUpdate.StartInfo.CreateNoWindow = true;
            wyUpdate.StartInfo.UseShellExecute = false;

            if (wyUpdate.Start())
            {
              _log.Information("{MethodName} wyUpdate was successfully started", methodName);

              if (wyUpdate.WaitForExit((int)TimeSpan.FromMinutes(45).TotalMilliseconds))
              {
                _log.Information("{MethodName} wyUpdate completed with exit code {ExitCode}", methodName, wyUpdate.ExitCode);
                /*if (wyUpdate.ExitCode == 0) // == 1 -no update
                {
                  _log.Information("wyUpdate indicates all files are up-to-date");
                }
                else*/
                if (wyUpdate.ExitCode == 1) // == 1 -Error
                {
                  throw new Exception("wyUpdate returned with an error");
                }
                if (wyUpdate.ExitCode != 0 && wyUpdate.ExitCode != 2) // == 2 -Updated     == 0 -No update
                {
                  throw new Exception(string.Format("wyUpdate returned unexpected exit code: {0}", wyUpdate.ExitCode));
                }

                var srcFiles = GetFilesExceptWyUpdate(SOURCE_PATH);
                if (srcFiles.Count == 0)
                {
                  _log.Information("{MethodName} No files in source", methodName);
                  return;
                }
                var destFiles = GetFilesExceptWyUpdate(DEST_PATH, srcFiles.Keys.ToList());

                _log.Information("{MethodName} Files: {@SourceFiles} {@DestFiles}", methodName, srcFiles, destFiles);

                // where source file does not exist in destination,             or hash between the files differs: copy
                var toCopy = srcFiles.Keys.Where(s => !destFiles.ContainsKey(s) || destFiles[s] != srcFiles[s]);
                if (!toCopy.Any())
                {
                  _log.Information("{MethodName} All files are up-to-date", methodName);
                  return;
                }

                #region Rename all DLLs and EXEs first... this allows us to update while EXE is in use
                foreach (var nameOnly in toCopy.Where(s => s.EndsWith(".exe") || s.EndsWith(".dll")))
                {
                  var currName = Path.Combine(DEST_PATH, nameOnly);
                  var renamedTo = Path.Combine(DEST_PATH, string.Format("{0}_{1:yyyy_MM_dd_HH_mm_ss}", nameOnly, DateTime.Now));
                  _log.Information("{MethodName} EXE/DLL: Renaming {CurrName} to {RenamedTo}", methodName, currName, renamedTo);
                  if (File.Exists(currName))
                  {
                    File.Move(currName, renamedTo);
                  }
                }
                #endregion

                #region Copy updated/new files to ass
                foreach (var nameOnly in toCopy)
                {
                  var sourceFile = Path.Combine(SOURCE_PATH, nameOnly);
                  var destFile = Path.Combine(DEST_PATH, nameOnly);
                  _log.Information("{MethodName} Update: Copying {SourceFile} to {DestFile}", methodName, sourceFile, destFile);
                  File.Copy(sourceFile, destFile, true);
                  LogEvents.Log(DateTime.Now, methodName, string.Format("Updated ASS file '{0}'", nameOnly), 0);
                }
                #endregion
              }
              else
              {
                if (!wyUpdate.HasExited)
                {
                  wyUpdate.Kill();
                }

                throw new Exception("Timed-out waiting for wyUpdate to complete");
              }
            }
            else
            {
              throw new Exception("wyUpdate.exe Process.Start() failed");
            }
          }
        }
        catch (Exception err)
        {
          LogEvents.Log(DateTime.Now, methodName, err.Message, 12);
          _log.Error(err, "{MethodName}", methodName);
        }
      }
      finally
      {
        // Delete old wyUpdate log files
        if (Directory.Exists(SOURCE_PATH))
        {
          var logFiles = Directory.GetFiles(SOURCE_PATH, "wyupdate_*.log");
          foreach (var file in logFiles)
          {
            var fileInfo = new FileInfo(file);
            if (DateTime.Now.Subtract(fileInfo.CreationTime).TotalDays > 10)
            {
              File.Delete(file);
            }
          }
        }
      }

      _log.Information("{MethodName} completed", methodName);
    }


    /// <summary>
    /// Returns dictionary of files found in parameter 'path', with the key field containing the
    /// filename only and the value field containing the SHA256 hash value of the contents of the file
    /// </summary>
    /// <param name="path">The directory to scan</param>
    /// <returns>Dictionary containing filename and file content SHA256 hash</returns>
    private static Dictionary<string, string> GetFilesExceptWyUpdate(string path, List<string> includeOnlyFilesNamed = null)
    {
      var result = new Dictionary<string, string>();
      var fileList = Directory.GetFiles(path, "*.*");
      foreach (var file in fileList)
      {
        var nameOnly = Path.GetFileName(file).ToLower();

        if (includeOnlyFilesNamed == null)
        {
          if (nameOnly != "client.wyc" && nameOnly != "wyupdate.exe" &&
            !nameOnly.EndsWith(".log") && !nameOnly.StartsWith("wrk") &&
            !nameOnly.EndsWith(".dbf") && !nameOnly.Substring(0, nameOnly.Length - 1).EndsWith(".nx") && !nameOnly.EndsWith(".mem") &&
            !nameOnly.StartsWith("cl1") && !nameOnly.StartsWith("js1") && !nameOnly.StartsWith("li1") && !nameOnly.StartsWith("x5") &&
            !nameOnly.EndsWith(".str") && /*!nameOnly.EndsWith(".csv") &&*/ !nameOnly.EndsWith(".001") && !nameOnly.StartsWith("rul1") &&
            !nameOnly.EndsWith(".tmp") &&
            !nameOnly.EndsWith(".zip") && !nameOnly.EndsWith(".7z") && !nameOnly.EndsWith(".cab") &&
            !nameOnly.EndsWith(".his") && !nameOnly.EndsWith(".old"))
          {
            result.Add(nameOnly, GetSHA256Checksum(file));
          }
        }
        else
        {
          if (includeOnlyFilesNamed.Contains(nameOnly))
          {
            result.Add(nameOnly, GetSHA256Checksum(file));
          }
        }
      }

      return result;
    }



    /// <summary>
    /// SHA256 hexadecimal hash value for a file
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private static string GetSHA256Checksum(string file)
    {
      using (var stream = File.OpenRead(file))
      {
        using (var sha = new SHA256Managed())
        {
          var checksum = sha.ComputeHash(stream);
          return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }
      }
    }


    #region Private consts

    /// <summary>
    /// Path where wyUpdate for ass is located and will download its files
    /// </summary>
    private const string SOURCE_PATH = @"C:\Atlas\LMS\Updates\ass\";

    /// <summary>
    /// Destination directory- ass
    /// </summary>
    private const string DEST_PATH = @"C:\LOAN\FILES\";


    #endregion


    #region Private vars

    // Logging
    private static readonly ILogger _log = Log.Logger.ForContext<BackupDatabase>();

    #endregion

  }
}
