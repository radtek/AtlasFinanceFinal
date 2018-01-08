/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Marks a branch as now being switched-over to PostgreSQL 
 *    (BRN_Config.DataSection == "DBTYPE" &  BRN_Config.DataSection == "RDDTYPE")
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
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Cache.Interfaces;


namespace ASSServer.Utils.PSQL.DbfImport
{
  public static class MarkBranchAsPSQL
  {
    /// <summary>
    /// Mark that the branch this server belongs to is now PostgreSQL
    /// </summary>
    /// <param name="branchServer"></param>
    /// <returns></returns>
    public static bool Execute(ICacheServer cache, Int64 branchId, List<string> progressMessages)
    {
      try
      {
        using (var unitOfWork = new UnitOfWork())
        {
          #region DBTYPE- PSQL (not important, if 'DBFNTX' = dBASE with NTX index)
          var dbType = unitOfWork.Query<BRN_Config>()
            .FirstOrDefault(s => s.Branch.BranchId == branchId && s.DataSection == "DBTYPE");
          if (dbType == null)
          {
            dbType = new BRN_Config(unitOfWork)
            {
              Branch = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branchId),
              DataSection = "DBTYPE",
              DataValue = "PSQL",
              CreatedDT = DateTime.Now
            };
          }
          else
          {
            dbType.DataValue = "PSQL";
          }
          #endregion

          #region RDDTYPE- SQLRDD (critical- used by ASS and RddSetDefault()...)
          var sqlRddType = unitOfWork.Query<BRN_Config>()
            .FirstOrDefault(s => s.Branch.BranchId == branchId && s.DataSection == "RDDTYPE");
          if (sqlRddType == null)
          {
            sqlRddType = new BRN_Config(unitOfWork)
            {
              Branch = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branchId),
              DataSection = "RDDTYPE",
              DataValue = "SQLRDD",
              CreatedDT = DateTime.Now
            };
          }
          else
          {
            sqlRddType.DataValue = "SQLRDD";
          }
          #endregion

          unitOfWork.CommitChanges();
          
        }

        return true;
      }
      catch (Exception err)
      {
        progressMessages.Add(string.Format("Unexpected error: '{0}'", err.Message));
        return false;
      }
    }

  }
}
