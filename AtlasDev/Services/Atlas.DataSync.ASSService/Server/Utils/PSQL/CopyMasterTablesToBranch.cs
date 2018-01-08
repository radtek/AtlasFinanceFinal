using System;
using System.Collections.Concurrent;
using System.Data;
using System.Collections.Generic;
using System.Linq;

using Npgsql;

using Atlas.Cache.Interfaces;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces.Classes;


namespace ASSServer.Utils.PSQL
{
  public static class CopyMasterTablesToBranch
  {
    /// <summary>
    /// Copies all master tables (+ indexes/defaults) to a specific schema (i.e. copies all master company.ZZZZZ tables to brXXX.ZZZZZZ)
    /// </summary>
    /// <param name="dbSchemaName">Fully qualified schema name- i.e. br001, br0m1</param>
    /// <param name="progressMessages"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(ICacheServer cache, IConfigSettings config, string dbSchemaName, ConcurrentQueue<string> progressMessages)
    {
      var masterTables = cache.GetAll<ASS_ServerTable_Cached>().Select(s => s.TableName).ToList();
      var position = "Starting";
      using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
      {
        conn.Open();

        //var trans = conn.BeginTransaction();
        // New npgsql gives error with transaction ??? WTF??
        try
        {
          var cmd = conn.CreateCommand();
          //cmd.Transaction = trans;
          cmd.CommandType = CommandType.Text;
          cmd.CommandTimeout = (int)TimeSpan.FromSeconds(60).TotalSeconds;

          // Copy data from company master tables, to brXXX master table, without company baggage
          foreach (var table in masterTables)
          {
            position = string.Format("Copying master data table [{0}] to branch schema", table);
            progressMessages.Enqueue(position);

            var completed = false;
            var attemptCount = 0;
            while (!completed) // this seems to time frequently?
            {
              try
              {
                cmd.CommandText = string.Format(
                  "BEGIN; " +
                  "DROP TABLE IF EXISTS \"{0}\".\"{1}\"; " +
                  "CREATE TABLE \"{0}\".\"{1}\" (LIKE company.\"{1}\" INCLUDING INDEXES);" +
                  "ALTER TABLE \"{0}\".\"{1}\" DROP CONSTRAINT IF EXISTS {1}_pkey;" +
                  "INSERT INTO \"{0}\".\"{1}\" SELECT * FROM company.\"{1}\"; " +
                  "COMMIT;", dbSchemaName, table);
                progressMessages.Enqueue(cmd.CommandText);
                cmd.ExecuteNonQuery();
                completed = true;
              }
              catch (Exception err)
              {
                progressMessages.Enqueue(string.Format("Position: {0}, Error: {1}. Count {2}", position, err.Message, attemptCount));
                if (attemptCount++ == 5)
                {
                  throw;
                }
              }
            }
          }

          // Copy sr_ tables
          position = "Copying SQLRDD tables";
          progressMessages.Enqueue(position);

          var branchCode = dbSchemaName.Substring(dbSchemaName.Length - 2).ToUpper();
          cmd.CommandText = string.Format(
            "BEGIN; " +
            "DELETE FROM {0}.sr_mgmnttables; " +
            "INSERT INTO {0}.sr_mgmnttables (table_, signature_, created_, type_, reginfo_) " +
              "SELECT table_, signature_, created_, type_, reginfo_ FROM company.sr_mgmnttables; " +
            "DELETE FROM {0}.sr_mgmntindexes; " +
            "INSERT INTO {0}.sr_mgmntindexes (table_, signature_, idxname_, phis_name_, idxkey_, idxfor_, idxcol_, tag_, tagnum_) " +
              "SELECT table_, signature_, idxname_, phis_name_, idxkey_, idxfor_, idxcol_, tag_, tagnum_ FROM company.sr_mgmntindexes; " +
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!! If any indexes on AUDIT table changes- must be manually 
            "UPDATE {0}.sr_mgmntindexes SET table_ = 'AUDIT{1}', idxname_ ='AUDIT{1}_IDX_1', phis_name_ = 'AUDIT{1}_IDX_1_1' WHERE table_ = 'AUDITXX'; " +
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!
              "COMMIT;", dbSchemaName, branchCode);

          progressMessages.Enqueue(cmd.CommandText);

          cmd.ExecuteNonQuery();

          position = "Committing";
          progressMessages.Enqueue(position);
          //trans.Commit();
        }
        catch (Exception err)
        {
          progressMessages.Enqueue(string.Format("Position: {0}, Error: {1}", position, err.Message));
          //trans.Rollback();
          return false;
        }
      }

      return true;
    }
  }
}
