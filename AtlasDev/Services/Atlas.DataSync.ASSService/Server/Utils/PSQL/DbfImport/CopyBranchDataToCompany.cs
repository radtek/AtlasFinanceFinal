/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*    Copies all data from specified brXXX schema to the 'company' schema. Ensures all fields exist and creates tables if they
*    do not exist and appends any new fields (should never happen?!?).
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
using System.Text;
using System.Collections.Concurrent;
using System.Data;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Npgsql;

using Atlas.Cache.Interfaces;
using Atlas.Common.Interface;
using Atlas.Cache.DataUtils;


namespace ASSServer.Utils.PSQL.DbfImport
{
  /// <summary>
  /// This will copy non-master data from source schema 'dbSchemaName' to the destination 'company' schema. Tables which do not exist
  /// within the 'company' will be created and fields which do not exist within 'company' will be added.
  /// </summary>
  public static class CopyBranchDataToCompany
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(ICacheServer cache, IConfigSettings config, string legacyBranchNum, ConcurrentQueue<string> progressMessages)
    {
      try
      {
        legacyBranchNum = legacyBranchNum.PadLeft(3, '0').ToLower();
        var dbSchemaName = string.Format("br{0}", legacyBranchNum);

        var createSql = new List<string>();
        var deleteSql = new List<string>();
        var copySql = new List<string>();

        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();
          var masterTables = CacheUtils.GetServerTableNames(cache);

          #region Get listing of current SQLRDD tables in the 'company' schema which contain a 'sr_recno' column
          var companyTables = new List<string>();
          using (var getTables = conn.CreateCommand())
          {
            getTables.CommandText = "select distinct table_name from information_schema.columns where column_name = 'sr_recno' and table_schema = 'company'";
            getTables.CommandType = CommandType.Text;
            using (var rdr = getTables.ExecuteReader())
            {
              while (rdr.Read())
              {
                companyTables.Add(rdr.GetString(0));
              }
            }
          }
          #endregion

          #region Get listing of tables in 'brXXX' schema which contain a 'sr_recno' column- these tables must be imported
          var sourceTableNames = new List<string>();
          using (var getTables = conn.CreateCommand())
          {
            getTables.CommandText = string.Format("select distinct table_name from information_schema.columns where column_name = 'sr_recno' and table_schema = '{0}'", dbSchemaName);
            getTables.CommandType = CommandType.Text;

            using (var rdr = getTables.ExecuteReader())
            {
              while (rdr.Read())
              {
                sourceTableNames.Add(rdr.GetString(0));
              }
            }
          }
          #endregion

          #region Build SQL script
          foreach (var sourceTableName in sourceTableNames)
          {
            // AUDITxx to be renamed and imported as AUDIT...
            var companyTableName = Regex.IsMatch(sourceTableName, "^audit[0-9a-z][0-9]$") ? "audit" : sourceTableName;

            // Only copy non-master, non replication and non-SQLRDD tables
            if (masterTables.IndexOf(companyTableName) == -1 && !companyTableName.StartsWith("sr_") && !companyTableName.StartsWith("lrep_"))
            {
              #region Get branch field names for 'sourceTableName'
              var branchFieldNames = new List<string>();
              using (var getColInfo = conn.CreateCommand())
              {
                getColInfo.CommandText = string.Format(
                  "SELECT column_name FROM information_schema.columns WHERE table_schema = '{0}' AND table_name = '{1}'", dbSchemaName, sourceTableName);
                getColInfo.CommandType = CommandType.Text;
                using (var rdr = getColInfo.ExecuteReader())
                {
                  while (rdr.Read())
                  {
                    var fieldName = rdr.GetString(0).ToLower();
                    branchFieldNames.Add(fieldName);
                  }
                }
              }
              #endregion

              var companyFieldNames = new List<string>();
              if (companyTables.IndexOf(companyTableName) < 0)
              {
                #region The 'company' table does not exist- make a structural copy of the brXXX table
                //  Copy data, create recid column and index on lrep_brnum
                createSql.Add(string.Format(
                  "CREATE TABLE company.\"{0}\" ( LIKE {1}.\"{2}\" ); " +
                  "ALTER TABLE company.\"{0}\" ADD COLUMN recid BIGSERIAL, ADD COLUMN lrep_brnum character(3);" +
                  "ALTER TABLE company.\"{0}\" ADD PRIMARY KEY (recid);" +
                  "CREATE INDEX idx_{0}_branch ON company.\"{0}\" (lrep_brnum);", companyTableName, dbSchemaName, sourceTableName, legacyBranchNum));
                #endregion

                // Company field name now matches branch field names...
                companyFieldNames = new List<string>(branchFieldNames);
                companyFieldNames.Add("lrep_brnum");
              }
              else
              {
                #region Get company field names in 'companyTableName'
                using (var getColInfo = conn.CreateCommand())
                {
                  getColInfo.CommandText = string.Format("SELECT column_name FROM information_schema.columns WHERE table_schema = 'company' AND table_name = '{0}'", companyTableName);
                  getColInfo.CommandType = CommandType.Text;
                  using (var rdr = getColInfo.ExecuteReader())
                  {
                    while (rdr.Read())
                    {
                      var fieldName = rdr.GetString(0).ToLower();
                      companyFieldNames.Add(fieldName);
                    }
                  }
                }
                #endregion
              }

              var sql = new StringBuilder();

              #region Build SQL to update the 'company' table with any missing columns
              var missingFields = branchFieldNames.Where(s => companyFieldNames.IndexOf(s) < 0);
              if (missingFields.Any())
              {
                var missingFieldsCSV = string.Join(", ", missingFields.Select(s => string.Format("'{0}'", s)));

                using (var getColInfo = conn.CreateCommand())
                {
                  getColInfo.CommandText = string.Format(
                    "SELECT column_name, data_type, character_maximum_length, numeric_precision, numeric_scale " +
                    "FROM information_schema.columns WHERE table_schema = '{0}' AND table_name = '{1}' AND column_name IN ({2})", dbSchemaName, companyTableName, missingFieldsCSV);
                  getColInfo.CommandType = CommandType.Text;

                  using (var rdr = getColInfo.ExecuteReader())
                  {
                    while (rdr.Read())
                    {
                      var fieldName = rdr.GetString(0).ToLower();

                      sql.AppendFormat("ALTER TABLE company.\"{0}\" ADD COLUMN {1};\r\n", companyTableName,
                        GetSQLFieldDeclaration(fieldName: rdr.GetString(0), fieldType: rdr.GetString(1), maxStringLen: rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2),
                        precision: rdr.IsDBNull(3) ? 0 : rdr.GetInt32(3), scale: rdr.IsDBNull(4) ? 0 : rdr.GetInt32(4)));
                      companyFieldNames.Add(fieldName);
                      progressMessages.Enqueue(string.Format("Branch table '{0}' contains extra field: '{1}'- adding to company", companyTableName, rdr.GetString(0)));
                    }
                  }
                }
              }

              if (companyFieldNames.IndexOf("lrep_brnum") == -1)
              {
                sql.AppendFormat("ALTER TABLE company.\"{0}\" ADD COLUMN \"lrep_brnum\" CHARACTER(3);\r\n", companyTableName);
              }

              if (sql.Length > 0)
              {
                createSql.Add(sql.ToString());
              }
              #endregion

              #region Copy data across
              var branchFieldNamesCSV = string.Join(", ", branchFieldNames.Select(s => string.Format("\"{0}\"", s)));

              deleteSql.Add(string.Format(
                "DELETE FROM company.\"{0}\" WHERE lrep_brnum='{1}'", companyTableName, legacyBranchNum.ToUpper()));

              copySql.Add(string.Format(
                "INSERT INTO company.\"{0}\" (\"lrep_brnum\", {2}) SELECT '{1}', {2} FROM {3}.\"{4}\"",
                  companyTableName, legacyBranchNum.ToUpper(), branchFieldNamesCSV, dbSchemaName, sourceTableName));

              #endregion
            }
          }
          #endregion
        }

        #region Execute scripts- running copy scripts in parallel seems to be slower... *much* faster in single transaction?
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          var trans = conn.BeginTransaction();
          using (var cmd = conn.CreateCommand())
          {
            try
            {
              cmd.Transaction = trans;

              progressMessages.Enqueue("Create missing company schema tables starting");
              foreach (var sqlToCreate in createSql)
              {
                progressMessages.Enqueue(string.Format("CREATE: Executing SQL: {0}", sqlToCreate));
                cmd.CommandText = sqlToCreate;
                cmd.CommandTimeout = (int)TimeSpan.FromSeconds(60).TotalSeconds;
                cmd.ExecuteNonQuery();
              }
              progressMessages.Enqueue("Create missing company schema tables complete");

              progressMessages.Enqueue("Delete existing company schema for branch data starting");
              foreach (var sqlToDelete in deleteSql)
              {
                progressMessages.Enqueue(string.Format("DELETE: Executing SQL: {0}", sqlToDelete));
                cmd.CommandText = sqlToDelete;
                cmd.CommandTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds;
                cmd.ExecuteNonQuery();
              }
              progressMessages.Enqueue("Delete existing company schema for branch data completed");

              progressMessages.Enqueue("Copy branch data to company schema starting");
              var copyTimer = Stopwatch.StartNew();
              foreach (var sql in copySql)
              {
                progressMessages.Enqueue(string.Format("COPY Executing SQL: {0}", sql));
                cmd.CommandText = sql;
                cmd.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalSeconds;
                cmd.ExecuteNonQuery();
              }
              progressMessages.Enqueue(string.Format("Copy branch data to company schema completed in {0:0.0}s", copyTimer.Elapsed.TotalSeconds));

              progressMessages.Enqueue("Commit beginning");
              trans.Commit();
              progressMessages.Enqueue("Commit completed");
            }
            catch
            {
              trans.Rollback();
              throw;
            }
          }
        }
        #endregion

        return true;
      }
      catch (Exception err)
      {
        progressMessages.Enqueue(string.Format("Unexpected error: '{0}'", err.Message));

        return false;
      }
    }


    #region Private methods

    /// <summary>
    /// Gets PSQL SQL scipt to create field creation declaration
    /// </summary>
    /// <param name="fieldName">field name</param>
    /// <param name="fieldType">field type</param>
    /// <param name="maxStringLen">maximum string length</param>
    /// <param name="precision">Numeric precision</param>
    /// <param name="scale">Numeric scale</param>
    /// <returns></returns>
    private static string GetSQLFieldDeclaration(string fieldName, string fieldType, int maxStringLen, int precision, int scale)
    {
      if (fieldType == "character" || fieldType == "character varying")
      {
        return string.Format("\"{0}\" {1}({2})", fieldName, fieldType, maxStringLen);
      }
      else if (fieldType == "numeric")
      {
        return string.Format("\"{0}\" {1}({2}, {3})", fieldName, fieldType, precision, scale);
      }
      else
      {
        return string.Format("\"{0}\" {1}", fieldName, fieldType);
      }
    }

    #endregion

  }
}
