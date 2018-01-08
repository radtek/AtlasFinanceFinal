/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Determines/confirms the ASS branch from a bunch of DBF files, using the AUDIT and COMPANY DBF files 
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

using SocialExplorer.IO.FastDBF;


namespace ASSServer.Utils.DBF
{
  class GetAssBranch
  {
    /// <summary>
    /// Returns 3-digit Branch number, using AUDITxx.DBF and confirming with COMPANY.DBF
    /// </summary>
    /// <param name="dbfPath">Path to the DBF files</param>
    /// <param name="branchNum">Found branchnumber, zero padded to 3 characters</param>
    /// <param name="errorMessage">Any errors processing</param>
    /// <returns></returns>
    public static bool Execute(string dbfPath, out string branchNum, out string errorMessage)
    {
      branchNum = null;
      errorMessage = null;

      #region Get branch number from AUDIT filename
      var auditFiles = Directory.GetFiles(dbfPath, "AUDIT*.DBF");
      if (auditFiles.Length == 0)
      {
        errorMessage = "No audit file in directory- unable to check for branch";
        return false;
      }
      if (auditFiles.Length > 1)
      {
        errorMessage = "Multiple AUDITxx.DBF files- please ensure only the correct AUDITxx.DBF file is in place";
        return false;
      }

      var auditFileName = Path.GetFileNameWithoutExtension(auditFiles[0]);
      var auditBranchNum = auditFileName.Substring(auditFileName.Length - 2, 2).PadLeft(3, '0');
      if (!System.Text.RegularExpressions.Regex.IsMatch(auditBranchNum, "0[0-9A-Z][0-9]"))
      {
        errorMessage = string.Format("Invalid branch number: '{0}'", auditBranchNum);
        return false;
      }      
      #endregion
            
      #region Check audit matches with COMPANY.BRNUMC field
      string companyBranchNum = null;
      var dbfFileName = Path.Combine(dbfPath, "COMPANY.DBF");
      if (!File.Exists(dbfFileName))
      {
        errorMessage = string.Format("The 'COMPANY.DBF' file is missing ({0})", dbfFileName);
        return false;
      }
      var odbf = new DbfFile(System.Text.ASCIIEncoding.ASCII);
      odbf.Open(dbfFileName, FileMode.Open);
      try
      {
        var orec = new DbfRecord(odbf.Header);
        if (!odbf.Read(0, orec))
        {
          errorMessage = "Failed to read the 'COMPANY.DBF' record";
          return false;        
        }
        
        var colIndex = orec.FindColumn("BRNUMC");
        if (colIndex == -1)
        {
          errorMessage = "The 'COMPANY.DBF' file is missing critical column data";
          return false;
        }
        companyBranchNum = orec[colIndex].PadLeft(3, '0');
      }
      finally
      {
        odbf.Close();
      }

      if (companyBranchNum != auditBranchNum)
      {
        errorMessage = string.Format("AUDITxx.DBF and COMPANY.DBF report different branch numbers- " +
          "cannot safely determine the branch number! Audit: {0}, Company: {1}", auditBranchNum, companyBranchNum);
        return false;
      }
      #endregion

      branchNum = auditBranchNum;
      return true;
    }

  }
}
