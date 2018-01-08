/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Creates the specified PostgreSQL schema
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

using Npgsql;

using Atlas.Common.Interface;


namespace ASSServer.Utils.PSQL.DbfImport
{
  public static class CreateSchema
  {
    /// <summary>
    /// Creates schema
    /// </summary>
    /// <param name="sqlConnectionString">PostgreSQL connection string</param>
    /// <returns>Error message, null if successful</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(IConfigSettings config, string dbSchemaName, List<string> progressMessages)
    {
      progressMessages.Add("Creating database core");
      try
      {
        progressMessages.Add("[CreateDBCore] Task starting");
        using (var sqlConn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          sqlConn.Open();

          #region Create scherma if does not exist
          using (var cmd = sqlConn.CreateCommand())
          {
            cmd.CommandText = string.Format("DROP SCHEMA IF EXISTS {0}; CREATE SCHEMA {0};", dbSchemaName);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

          }
          #endregion
        }

        progressMessages.Add("[CreateDBCore] Task completed");
        return true;
      }
      catch (Exception err)
      {
        progressMessages.Add(string.Format("[CreateDBCore] >> Fatal << Error: '{0}'", err.Message));
        return false;
      }
    }

  }
}