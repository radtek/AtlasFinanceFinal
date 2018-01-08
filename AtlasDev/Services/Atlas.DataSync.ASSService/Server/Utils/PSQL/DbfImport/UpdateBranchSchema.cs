/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Upgrade a branch schema to match the current system schema version.
 *    This will run necessary SQL scripts to upgrade the schema and populate the lrep_ and sr_ tables.
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *       
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;
using System.Collections.Generic;

using Npgsql;

using System.Text;

using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace ASSServer.Utils.PSQL
{
  public class UpdateBranchSchemaToLatest
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(IConfigSettings config, ICacheServer cache, string schemaName, out string newDbVersion, out string errorMessage)
    {
      errorMessage = null;
      var successful = true;
      newDbVersion = null;

      try
      {
        string currDbVersion = null;
        List<VerUpdateScripts> scripts = null;

        #region Determine current ASS DB version in the schema
        using (var sqlConn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          sqlConn.Open();

          using (var cmd = sqlConn.CreateCommand())
          {            
            cmd.CommandText = string.Format("SELECT \"value\" FROM \"{0}\".\"lrep_db_info\" WHERE \"setting\" = 1", schemaName);
            var scalarValue = cmd.ExecuteScalar();
            if (scalarValue == null)
            {
              errorMessage = string.Format("Unable to determine current ASS database version for schema: {0}", schemaName);
              return false;
            }
            currDbVersion = (string)scalarValue;
            newDbVersion = currDbVersion;
          }          
        }
        #endregion        
                      
        if (GetBranchDbUpdateScript.Execute(cache, currDbVersion, out scripts, out errorMessage))
        {
          var sql = new StringBuilder();

          #region Get scripts to update from this version to the current version
          sql.AppendFormat("set search_path TO {0};", schemaName);
          if (scripts != null && scripts.Count > 0)
          {
            foreach (var script in scripts)
            {
              if (!string.IsNullOrEmpty(script.SQLScript))
              {
                sql.AppendLine(script.SQLScript);
              }
              currDbVersion = script.Version;
            }
          }
          #endregion

          #region Update lrep_db_info
          sql.AppendFormat("DELETE FROM \"{0}\".\"lrep_db_info\" WHERE \"setting\" = 1;", schemaName);
          sql.AppendFormat("INSERT INTO \"{0}\".\"lrep_db_info\" (\"setting\", \"value\") VALUES(1, '{1}');", schemaName, currDbVersion);
          #endregion

          #region Update sr_mgmnttables and sr_mgmntindexes
          // !!!!! NO !!!! Done by update script and master tables sr_mgmnttables copied on request
          //sql.AppendFormat("DELETE FROM \"{0}\".\"sr_mgmnttables\";", schemaName);
          //sql.AppendFormat("DELETE FROM \"{0}\".\"sr_mgmntindexes\";", schemaName);
          //sql.AppendFormat("INSERT INTO \"{0}\".\"sr_mgmnttables\" SELECT * FROM \"company\".\"sr_mgmnttables\";", schemaName);
          //sql.AppendFormat("INSERT INTO \"{0}\".\"sr_mgmntindexes\" SELECT * FROM \"company\".\"sr_mgmntindexes\";", schemaName);
          //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
          #endregion

          #region Commit the update
          using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
          {
            conn.Open();
            var transaction = conn.BeginTransaction();
            try
            {
              using (var cmd = conn.CreateCommand())
              {
                cmd.Transaction = transaction;
                cmd.CommandText = sql.ToString();
                cmd.ExecuteNonQuery();
              }

              transaction.Commit();
            }
            catch (Exception err)
            {
              transaction.Rollback();
              errorMessage = err.Message;
              return false;
            }
          }
          #endregion
          newDbVersion = currDbVersion;
        }        
      }
      catch (Exception err)
      {
        errorMessage = err.Message;
        successful = false;
      }

      return successful;
    }
  }
}