using System;
using System.Diagnostics;
using System.IO;


namespace ASSServer.Utils.PSQL.DbfImport
{
  public static class DumpSingleSchemaStruct
  {

    /// <summary>
    /// Dumps the specified database structure, using plain SQL, for a specific schema
    /// </summary>
    /// <param name="pgDumpEXE">The pg_dump EXE</param>
    /// <param name="schemaName">The schema</param>
    /// <param name="dbName"></param>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="dumpFileName"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static bool Execute(string pgDumpEXE, string dbServer, string dbName, string schemaName, string userName, 
      string password, string dumpFileName, out string errorMessage)
    {
      errorMessage = null;
      try
      {
        var cmdLineParams = string.Format("--file=\"{0}\" --format=p --quote-all-identifiers --host={1} --port=5432 --username={2} --no-password --schema-only --schema=br{3} {4}",
          dumpFileName, dbServer, userName, schemaName, dbName);
        using (var pgDump = new Process())
        {
          ProcessStartInfo startInfo = new ProcessStartInfo(pgDumpEXE, cmdLineParams);
          startInfo.EnvironmentVariables.Add("PGPASSWORD", password);
          startInfo.WindowStyle = ProcessWindowStyle.Hidden;//.Normal;
          startInfo.UseShellExecute = false; // Required for environment

          pgDump.StartInfo = startInfo;

          if (!pgDump.Start())
          {
            errorMessage = "Failed to start the local PostgreSQL backup system!";
            return false;
          }

          var dumpWait = new Stopwatch();
          dumpWait.Start();
          while (dumpWait.Elapsed.TotalMinutes < 20)
          {
            if (pgDump.WaitForExit(10000) || pgDump.HasExited)
            {
              return pgDump.ExitCode == 0 && File.Exists(dumpFileName);
            }            
          }

          pgDump.Kill();
        }

        errorMessage = "Timed-out waiting for backup process to complete";
        try { File.Delete(dumpFileName); }
        catch { }
                
        return true;
      }
      catch (Exception err)
      {
        errorMessage = err.Message;
        return false;
      }
    }

  }
}
