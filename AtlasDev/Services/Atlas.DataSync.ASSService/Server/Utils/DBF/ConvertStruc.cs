/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Converts the DBF files to PostgreSQL, using the xHarbour console application (DBF2PSQL)
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-07-01 Created
 * 
 * 
 *  To do:
 *  ------------------
 *     
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace ASSServer.Utils.DBF
{  
  public static class ConvertStruc
  {
    /// <summary>
    /// Runs xHarbour console to create the equivalent SQLRDD SQL *structure* (tables, fields, indexes, sequences, PK) of all DBF's in a folder
    /// </summary>
    /// <param name="dbfFilePath">Path to ASS DBF files</param>
    /// <param name="serverAddress">PostgreSQL Address</param>
    /// <param name="userName">PostgreSQL user name</param>
    /// <param name="userPassword">PostgreSQL password</param>
    /// <returns>Any error, else null</returns>
    public static bool Execute(string dbfFilePath, string serverAddress, string userName, string userPassword, string dbSchemaName, List<string> progressMessages)
    {
      progressMessages.Add("XHB STRUCTURE- Converting structure process starting");
      try
      {
        var process = new Process();
        var info = new ProcessStartInfo("DBF2PSQL.exe", string.Format("{0} \"{1}\" {2} {3} {4}",
                 dbfFilePath, serverAddress, userName, userPassword, dbSchemaName));
        info.WindowStyle = ProcessWindowStyle.Normal;
        process.StartInfo = info;

        process.Start();
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
          throw new Exception("xHarbour structure conversion software encountered some errors and did not successfully convert all the data");
        }
        progressMessages.Add("XHB STRUCTURE- Process completed successfully");
        return true;
      }
      catch (Exception err)
      {
        progressMessages.Add(string.Format("  XHB STRUCTURE- >> FATAL << Error: '{0}'", err.Message));
        return false;
      }
    }

  }
}
