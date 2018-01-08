/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Updates the branch's schema with version number: brXXX.lrep_db_info.settings = "version"
 *       
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     22 Nov 2013- Created
 * 
 * 
 *  Comments:
 *  ------------------
 *   
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;
using System.Collections.Generic;

using Npgsql;
using DevExpress.Xpo;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;

using Atlas.Domain.Model;


namespace ASSServer.Utils.PSQL.DbfImport
{
  public static class SetBranchDBReplicationStatus
  {
    /// <summary>
    /// Updates the databases (brXXX.lrep_db_info + atlas_core.ASS_BranchServer) with branch database version info + current master change tracking recid
    /// </summary>
    /// <param name="branchServerId">Server branch ID (ASS_BranchServer.BranchServerId</param>
    /// <param name="version">The database version</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(IConfigSettings config, ICacheServer cache, Int64 serverId, string version, out string errorMessage)
    {
      errorMessage = null;

      try
      {        
        var server = cache.Get<ASS_BranchServer_Cached>(serverId);
        if (server == null || server.Branch == null)
        {
          errorMessage = string.Format("Failed to locate branch server with ID: {0}", serverId);
          return false;
        }

        var branch = cache.Get<BRN_Branch_Cached>(server.Branch.Value);     
        var schemaName = string.Format("br{0}", branch.LegacyBranchNum.PadLeft(3, '0').ToLower());
        var foundVer = cache.GetAll<ASS_DbUpdateScript_VerString_Cached>()?.FirstOrDefault(s => s.DBVersion == version);
        if (foundVer != null && server.RunningDBVersion != foundVer.DbUpdateScriptId)
        {
          server.RunningDBVersion = foundVer.DbUpdateScriptId;
          cache.Set(new List<ASS_BranchServer_Cached> { server });
        }
        
        long serverMasterRecId = 0;
        using (var unitOfWork = new UnitOfWork())
        {
          try
          {
            serverMasterRecId = unitOfWork.Query<ASS_MasterTableChangeTracking>().Max(s => s.RecId);
          }
          catch (Exception err)
          {
            errorMessage = string.Format("Failed to locate server master table change tracking: '{0}'", err.Message);
            return false;
          }
        }

        #region Update branch lrep_db_info table with version (required in case branch restores DB from server)
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          // This is not the most efficient SQL, but it will run rarely and is nicely self-contained
          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = string.Format(
              "CREATE TABLE IF NOT EXISTS \"{0}\".\"lrep_db_info\" (\"recid\" SERIAL PRIMARY KEY, \"setting\" int NOT NULL, \"value\" varchar(500) NOT NULL);" +
              "DELETE FROM \"{0}\".\"lrep_db_info\" WHERE \"setting\" IN (1, 2);" +
              "INSERT INTO \"{0}\".\"lrep_db_info\" (\"setting\", \"value\") VALUES (1, {1});" +
              "INSERT INTO \"{0}\".\"lrep_db_info\" (\"setting\", \"value\") VALUES (2, {2});",
              schemaName, Atlas.Data.Utils.PostgresUtils.PSQLStringyfy(version), serverMasterRecId);
            cmd.ExecuteNonQuery();
          }
        }
        #endregion

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
