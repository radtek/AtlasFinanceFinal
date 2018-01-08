/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Drops the specified schema
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

using System.Data;

using Npgsql;

using Atlas.Common.Interface;


namespace ASSServer.Utils.PSQL.DbfImport
{
  class DropSchemaCascade
  {
    /// <summary>
    /// Drops named schema with 'CASCADE' option
    /// </summary>
    /// <param name="schemaName"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(IConfigSettings config, string schemaName)
    {
      try
      {
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();
          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("DROP SCHEMA IF EXISTS {0} CASCADE", schemaName);
            cmd.ExecuteNonQuery();
          }
        }

        return true;
      }
      catch
      {
        return false;
      }
    }

  }
}
