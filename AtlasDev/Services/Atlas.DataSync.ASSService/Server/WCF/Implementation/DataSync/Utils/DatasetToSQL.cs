/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     WCF support utilities- Convert DataSet rows to PostgreSQL SQL updates- all or nothing transaction-based
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
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using Npgsql;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.DataUtils;


namespace ASSServer.WCF.Implementation.DataSync.Utils
{
  public static class DatasetToSQL
  {
    #region Public methods

    /// <summary>
    /// Writes DataTable changes to PostgreSQL
    /// </summary>
    /// <param name="schema">Name of PSQL schema</param>
    /// <param name="table">The DataTable containing full row details for rows changed</param>
    /// <param name="dataSet">The DataSet containing full row details for tables/rows changed</param>
    /// <param name="thisIsACompanyTable">Is this table the 'merged' data table?</param>
    /// <param name="legacyBranchNum">The ASS Legacy branch number</param>
    /// <param name="errorMessage"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config,
      string schema, DataSet dataSet, bool thisIsACompanyTable, string legacyBranchNum, out string errorMessage)
    {
      errorMessage = null;
      string lastSQL = null;

      try
      {
        var masterTables = CacheUtils.GetServerTableNames(cache);

        var regExAudit = new Regex("audit[0-9a-z][0-9]");
        var updated = 0;
        var deleted = 0;
        var inserted = 0;

        legacyBranchNum = legacyBranchNum.ToUpper().PadLeft(3, '0');
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();
          var transaction = conn.BeginTransaction();

          try
          {
            foreach (DataTable table in dataSet.Tables)
            {
              var tableName = table.TableName.ToLower();

              // Don't allow updates to master tables
              if (string.IsNullOrEmpty(tableName) || masterTables.Contains(tableName))
              {
                log.Error("Skipping updates for table {TableName}- table is master or null", tableName);
                continue;
              }

              var tableUpdated = 0;
              var tableInserted = 0;
              var tableDeleted = 0;

              if (thisIsACompanyTable && regExAudit.IsMatch(tableName))
              {
                tableName = "audit";
              }

              #region Check table basics
              if (table.Columns.Count < 4)
              {
                errorMessage = string.Format("Missing fields. Table '{0}', Column count: {1}- Cols: {2}", 
                  table.TableName, table.Columns.Count, string.Join(", ", table.Columns.OfType<DataColumn>().Select(s => s.ColumnName)));
                return false;
              }
              var colChangeType = table.Columns.IndexOf("change_type");
              if (colChangeType == -1)
              {
                errorMessage = "Missing column 'change_type'";
                return false;
              }
              var colRecNo = table.Columns.IndexOf("sr_recno");
              if (colRecNo == -1)
              {
                errorMessage = "Missing column 'sr_recno'";
                return false;
              }

              if (table.Columns[colChangeType].DataType != typeof(string))
              {
                errorMessage = "Column 'change_type' has invalid type";
                return false;
              }
              #endregion

              #region Build specific where clause (if company schema, also requires match on field 'lrep_brnum', else just match on field 'sr_recno'
              var fullTableName = string.Format("\"{0}\".\"{1}\"", schema, tableName);
              var whereFormat = thisIsACompanyTable ?
                "\"sr_recno\" = {0:F0} AND \"lrep_brnum\" = '{1}'" :
                "\"sr_recno\" = {0:F0}";
              #endregion

              #region Build insert field name statement
              var insertSQLFields = new StringBuilder();
              var prependText = string.Empty;
              for (var i = 0; i < table.Columns.Count; i++)
              {
                if (i != colChangeType)
                {
                  insertSQLFields.AppendFormat("{0}\"{1}\"", prependText, table.Columns[i].ColumnName);
                  prependText = ",";
                }
              }

              if (thisIsACompanyTable)
              {
                insertSQLFields.Append(", \"lrep_brnum\"");
              }
              #endregion

              #region Perform the update
              using (var updateDb = conn.CreateCommand())
              using (var checkExists = conn.CreateCommand())
              {               
                checkExists.CommandTimeout = 10;
                updateDb.CommandTimeout = 10;

                updateDb.Transaction = transaction;

                for (var i = 0; i < table.Rows.Count; i++)
                {
                  var currRow = table.Rows[i];
                  var changeType = currRow.Field<string>(colChangeType);
                  var sr_recno = currRow.Field<Decimal>(colRecNo);

                  if (sr_recno == 0)
                  {
                    errorMessage = "Empty value for field sr_recno!";
                    return false;
                  }
                  if (changeType != "U" && changeType != "I" && changeType != "D")
                  {
                    errorMessage = string.Format("Invalid value for change_type column: '{0}'", changeType);
                    return false;
                  }

                  #region If 'insert' and record already exists, effect an update. If 'update' and record does not exist, effect an insert
                  checkExists.CommandText = string.Format("SELECT COUNT(*) FROM {0} WHERE {1}", fullTableName, string.Format(whereFormat, sr_recno, legacyBranchNum));
                  lastSQL = checkExists.CommandText;
                  var recCount = (Int64)checkExists.ExecuteScalar();

                  if (changeType == "I" && recCount == 1) // Append where record already exists- update
                  {
                    changeType = "U";
                  }
                  else if (changeType == "U" && recCount == 0) // Update where record does not exist- append
                  {
                    changeType = "I";
                  }
                  else if (changeType == "D" && recCount == 0)
                  {
                    // TODO: Log this data error!
                  }
                  #endregion

                  #region Update/append the row in the database
                  switch (changeType)
                  {
                    case "I":
                      var values = new StringBuilder();
                      var prependInsert = string.Empty;
                      for (var field = 0; field < table.Columns.Count; field++)
                      {
                        if (field != colChangeType)
                        {
                          values.AppendFormat("{0}{1}", prependInsert, UnicodeStringyfy(currRow[field], table.Columns[field].ColumnName));
                          prependInsert = ",";
                        }
                      }
                      if (thisIsACompanyTable)
                      {
                        values.AppendFormat(", '{0}'", legacyBranchNum);
                      }
                      updateDb.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES({2});", fullTableName, insertSQLFields, values);
                      tableInserted++;

                      break;

                    case "U":
                      var setValues = new StringBuilder();

                      var prependUpdate = string.Empty;
                      for (var field = 0; field < table.Columns.Count; field++)
                      {
                        if (field != colChangeType && colChangeType != colRecNo)
                        {
                          setValues.AppendFormat("{0}\"{1}\" = {2}", prependUpdate,
                            table.Columns[field].ColumnName, UnicodeStringyfy(currRow[field], table.Columns[field].ColumnName));
                          prependUpdate = ",";
                        }
                      }
                      if (thisIsACompanyTable)
                      {
                        setValues.AppendFormat(", \"lrep_brnum\" = '{0}'", legacyBranchNum);
                      }

                      var whereClauseUpd = string.Format(whereFormat, sr_recno, legacyBranchNum);
                      updateDb.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", fullTableName, setValues, whereClauseUpd);
                      tableUpdated++;

                      break;

                    case "D":
                      var whereClauseDel = string.Format(whereFormat, sr_recno, legacyBranchNum);
                      updateDb.CommandText = thisIsACompanyTable ?
                        string.Format("UPDATE {0} SET sr_deleted = 'T' WHERE {1}", fullTableName, whereClauseDel) : // Don't physically delete from company schema
                        string.Format("DELETE FROM {0} WHERE {1}", fullTableName, whereClauseDel);                  // Always delete from brXXX schemas to keep the PG restore clean
                      tableDeleted++;

                      break;
                  }
                  #endregion

                  lastSQL = updateDb.CommandText;
                  updateDb.ExecuteNonQuery();
                }
              }

              log.Information("Table {Table} updates successful- {Schema}, {Branch}, {UpdatedRows}, {InsertedRows}, {DeletedRows}",
                 table, schema, legacyBranchNum, tableUpdated, tableInserted, tableDeleted);

              updated += tableUpdated;
              inserted += tableInserted;
              deleted += tableDeleted;
              #endregion
            }

            transaction.Commit();
            log.Information("Branch {Branch} updates successful- '{Schema}', {UpdatedRows}, {InsertedRows}, {DeletedRows}",
              legacyBranchNum, schema, updated, inserted, deleted);

            return true;
          }
          catch (Exception err)
          {
            transaction.Rollback();
            errorMessage = string.Format("{0}- {1}", err.Message, lastSQL);
            return false;
          }
        }
      }
      catch (Exception err)
      {
        errorMessage = err.Message;
        return false;
      }
    }
    #endregion


    /// <summary>
    /// Creates a safe PosgresSQL SQL textual expression of given value
    /// </summary>
    /// <param name="value">The value to stringyfy</param>
    /// <param name="fieldName">The fieldname- if 'sr_recno' does not bother with fractional part</param>
    /// <returns>A Postgres SQL safe textual representation of the objects value</returns>
    private static string UnicodeStringyfy(object value, string fieldName = "")
    {
      if (value == null || value == DBNull.Value)
      {
        return "null";
      }

      try
      {
        var valueType = value.GetType();

        if (valueType == typeof(string))
        {
          #region Create safe string using uncode escaping
          var stringVal = value as string;
          var sb = new StringBuilder();

          var valueBytes = ASCIIEncoding.ASCII.GetBytes(stringVal);
          var escaped = string.Empty;           // The escape prefix (if required)
          for (var i = 0; i < valueBytes.Length; i++)
          {
            if (valueBytes[i] >= 32 && valueBytes[i] < 127 && valueBytes[i] != 92 && valueBytes[i] != 96 && valueBytes[i] != 39) // Avoid: ' \ `
            {
              sb.Append((char)valueBytes[i]);
            }
            else
            {
              escaped = "U&";
              //  Do unicode
              sb.AppendFormat("\\{0:x4}", (int)valueBytes[i]);
            }
          }
          #endregion

          return string.Format("{0}'{1}'", escaped, sb);
        }

        else if (valueType == typeof(DateTime))
        {
          var dateVal = value as DateTime?;
          return string.Format("'{0:yyyy-MM-dd} 00:00:00'", dateVal);
        }

        else if (valueType == typeof(Decimal))
        {
          var decimalVal = value as Decimal?;
          return decimalVal.Value.ToString("F2", CultureInfo.InvariantCulture);
        }

        else if (valueType == typeof(float))
        {
          var floatVal = value as float?;
          return floatVal.Value.ToString("F2", CultureInfo.InvariantCulture);
        }

        else if (valueType == typeof(Boolean))
        {
          var booleanVal = value as bool?;
          return booleanVal.Value ? "true" : "false";
        }

        else if (valueType == typeof(double))
        {
          var doubleVal = value as double?;
          return fieldName == "sr_recno" ? doubleVal.Value.ToString("F0", CultureInfo.InvariantCulture) : doubleVal.Value.ToString("F2", CultureInfo.InvariantCulture);
        }

        else
        {
          throw new Exception(string.Format("Unknown data type {0}", valueType));
        }
      }
      catch
      {
        throw;
      }
    }
  }
}