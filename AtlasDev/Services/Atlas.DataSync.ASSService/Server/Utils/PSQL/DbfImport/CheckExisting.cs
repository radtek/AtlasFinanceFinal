/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Ensures there are no ASS data (empty tables are ignored) in the specified PostgreSQL schema 
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
using System.Data;
using System.Collections.Generic;

using Npgsql;

using Atlas.Common.Interface;


namespace ASSServer.Utils.PSQL.DbfImport
{
  public static class CheckExisting
  {
    /// <summary>
    /// Ensure there is no existing data in any tables
    /// </summary>
    /// <param name="sqlConnectionString"></param>
    /// <returns>true if safe to proceed, false if data exists and may lose data</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(IConfigSettings config, string dbSchemaName, List<string> progressMessages)
    {
      progressMessages.Add("EXISTING DATA- Checking");
      try
      {
        #region check if any data in any user table
        using (var sqlConn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          sqlConn.Open();

          using (var cmd = sqlConn.CreateCommand())
          {
            #region Get listing of all converted branch tables
            var tableNames = new List<string>();
            progressMessages.Add("  EXISTING DATA- Reading table listing");
            cmd.CommandText = string.Format(
              "select \"table_name\" from information_schema.tables WHERE \"table_catalog\" = 'ass' and " +
              "\"table_schema\" = '{0}' AND \"table_type\" = 'BASE TABLE'", dbSchemaName);
            cmd.CommandType = CommandType.Text;
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                var tableName = rdr.GetString(0).ToLower();
                if (!tableName.StartsWith("sr_")) // ignore SQLRDD tables
                {
                  tableNames.Add(tableName);
                }
              }
            }
            #endregion

            #region Ensure no data in each table
            foreach (var tableName in tableNames)
            {
              progressMessages.Add(string.Format("    [{0}]- EXISTING DATA- Checking for existing data in table", tableName));

              // Fast row count estimate: string.Format("SELECT reltuples FROM pg_class WHERE oid = '{0}.{1}'::regclass", dbSchemaName, tableName);
              // This is okay and fast:
              // http://wiki.postgresql.org/wiki/FAQ#Why_is_.22SELECT_count.28.2A.29_FROM_bigtable.3B.22_slow.3F
              cmd.CommandText = string.Format("SELECT COUNT(*) AS REC_COUNT FROM \"{0}\".\"{1}\"", dbSchemaName, tableName);
              var count = (Int64)cmd.ExecuteScalar();
              if (count > 0)
              {
                progressMessages.Add(string.Format("EXISTING DATA- >> FATAL << Table '{0}.{1}' already contains some data- please clear the destination database/tables before proceeding!", dbSchemaName, tableName));
                return false;
              }
            }
            #endregion
          }
        }
        #endregion

        progressMessages.Add("EXISTING DATA- Completed");

        return true;
      }
      catch (Exception err)
      {
        progressMessages.Add(string.Format("[{0}] EXISTING DATA- >> Fatal << Error: '{0}'", err.Message));
        return false;
      }
    }

  }
}