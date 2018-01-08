/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   Determines the ASS database version, using the V* file
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
 * ----------------------------------------------------------------------------------------------------------------- */
 
using System;
using System.IO;
using System.Linq;


namespace ASSServer.Utils.DBF
{
  public static class GetAssVer
  {
    /// <summary>
    /// Get ASS DB version
    /// </summary>
    /// <param name="dbfPath">Path to DBF files</param>
    /// <param name="version">Version found</param>
    /// <param name="errorMessage">Any error message</param>
    /// <returns></returns>
    public static bool Execute(string dbfPath, out string version, out string errorMessage)
    {
      version = null;
      errorMessage = null;

      var versionFiles = Directory.GetFiles(dbfPath, "v0*.").OrderBy(s => s).ToList();
      if (versionFiles.Count == 0)
      {
        errorMessage = "Unable to determine database version- no version files included in directory!";
        return false;
      }
      var versionFile = versionFiles[versionFiles.Count - 1];
      var versionText = File.ReadAllText(versionFile);
      if (string.IsNullOrEmpty(versionText))
      {
        errorMessage = string.Format("Version file '{0}' does not contain any version information!", versionFile);
        return false;
      }
      var words = versionText.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
      if (words.Length == 0)
      {
        errorMessage = string.Format("Version file '{0}' does not contain any version information!", versionFile);
        return false;
      }

      var dbVersion = words[words.Length - 1];
      if (System.Text.RegularExpressions.Regex.IsMatch(@"^\d\d\d\d\D$", dbVersion))
      {
        errorMessage = string.Format("Version file '{0}' contains invalid version any version information: '{1}'", versionFile, dbVersion);
        return false;
      }

      version = dbVersion.Trim();
      return true;
    }

  }
}
