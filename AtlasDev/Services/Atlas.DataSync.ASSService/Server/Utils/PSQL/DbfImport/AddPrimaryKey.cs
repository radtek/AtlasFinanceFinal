/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Finds all SQLRDD tables and adds a primary key on the SR_RECNO field using naming template: pk_{0}_{1}
 *    Where {0} is the schema and {1} is the table name
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
using System.Data;
using System.Linq;

using Npgsql;

using Atlas.Common.Interface;


namespace ASSServer.Utils.PSQL.DbfImport
{
  public static class AddPrimaryKey
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(IConfigSettings config, string dbSchemaName, List<string> progressMessages)
    {
      progressMessages.Add("ADD PK- Checking for existing data");
      try
      {
        using (var sqlConn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          sqlConn.Open();

          using (var cmd = sqlConn.CreateCommand())
          {
            #region Get listing of all converted branch tables
            var tableNames = new List<string>();
            progressMessages.Add("  ADD PK- Reading table listing");
            cmd.CommandText = string.Format("select \"table_name\" from information_schema.tables WHERE \"table_catalog\" = 'ass' and " +
              "\"table_schema\" = '{0}' AND \"table_type\" = 'BASE TABLE'", dbSchemaName);
            cmd.CommandType = CommandType.Text;
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                var tableName = rdr.GetString(0).ToLower();
                if (!tableName.StartsWith("lrep_") && !tableName.StartsWith("sr_")) // lrep_ = local replication tables, sr_mgnmt = SQLRDD management tables
                {
                  tableNames.Add(tableName);
                }
              }
            }
            #endregion

            foreach (var tableName in tableNames)
            {
              progressMessages.Add(string.Format("  [{0}] ADD PK- Adding Primary Key..", tableName));
              cmd.CommandText = string.Format("ALTER TABLE \"{0}\".\"{1}\" ADD CONSTRAINT \"pk_{0}_{1}\" PRIMARY KEY (sr_recno);", dbSchemaName, tableName);
              cmd.ExecuteNonQuery();
              progressMessages.Add(string.Format("  [{0}] ADD PK- Complete", tableName));
            }

            return true;
          }
        }
      }
      catch (Exception err)
      {
        progressMessages.Add(string.Format("[FATAL]- ADD PK- '{0}'", err.Message));
        return false;
      }
    }

  }
}
