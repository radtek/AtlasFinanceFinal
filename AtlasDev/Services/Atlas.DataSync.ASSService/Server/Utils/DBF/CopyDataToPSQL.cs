/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Copies across data from DBF to PostgreSQL using prepared statements- the xHarbour copy process is 
 *     far too slow  (in the order of 4-8 times slower)
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
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

using Npgsql;
using SocialExplorer.IO.FastDBF;

using Atlas.Data.DBF.Utils;
using Atlas.Common.Interface;


namespace ASSServer.Utils.DBF
{
  public static class CopyDataToPSQL
  {
    #region Public methods

    /// <summary>
    /// Copies across data from DBF to PostgreSQL
    /// </summary>
    /// <param name="dbfFileName">The source DBF to use for data</param>
    /// <param name="dbSchemaName">The PostgreSQL schema name to copy into</param>
    /// <param name="copyProgressMessages">Provide feedback/progress</param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(IConfigSettings config, string dbfFileName, string dbSchemaName, ConcurrentQueue<string> copyProgressMessages)
    {
      var fileName = Path.GetFileNameWithoutExtension(dbfFileName).ToLower();
      var tableName = Path.GetFileNameWithoutExtension(dbfFileName).ToLower();

      copyProgressMessages.Enqueue(string.Format("[{0}] COPY- Copy data process starting", tableName));

      // We make these top-level for debugging/data issues in exception handler
      string lastSQL = null;
      string prepareSQL = null;
      string deallocateSQL = null;

      var currRow = 0;

      try
      {
        var tableTimer = new Stopwatch();
        tableTimer.Start();

        var odbf = new DbfFile(ASCIIEncoding.ASCII);
        odbf.Open(dbfFileName, FileMode.Open);
        try
        {
          var recordCount = odbf.Header.RecordCount;
          if (recordCount == 0)
          {
            copyProgressMessages.Enqueue(string.Format("  [{0}] COPY- No data to copy", fileName));
            return true;
          }

          using (var sqlConn = new NpgsqlConnection(config.GetAssConnectionString()))
          {
            sqlConn.Open();

            var sqlFieldTypes = new StringBuilder();
            var sqlParams = new StringBuilder();
            var sqlFieldNames = new StringBuilder();

            var orec = new DbfRecord(odbf.Header);
            for (var i = 0; i < odbf.Header.ColumnCount; i++)
            {
              sqlFieldTypes.AppendFormat("{0}{1}", i > 0 ? ", " : "", Atlas.Data.DBF.Utils.PSQLUtils.TypeAsSQLType(orec.Column(i).ColumnType));
              sqlParams.AppendFormat("{0}${1}", i > 0 ? ", " : "", i + 1);
              sqlFieldNames.AppendFormat("{0}\"{1}\"", i > 0 ? ", " : "", orec.Column(i).Name.ToLower());
            }

            using (var insertDataCommand = sqlConn.CreateCommand())
            {
              prepareSQL = string.Format("PREPARE bigInsert_{0}_{1} ({2}) AS INSERT INTO \"{0}\".\"{1}\" ({3}) VALUES({4});",
                dbSchemaName, tableName, sqlFieldTypes, sqlFieldNames, sqlParams);
              insertDataCommand.CommandType = CommandType.Text;
              insertDataCommand.CommandText = prepareSQL;
              insertDataCommand.ExecuteNonQuery();
                            
              var progressRecs = 0;
              var tableRows = 0;              
              var values = new StringBuilder();

              for (var row = 0; row < recordCount; row++)
              {                
                currRow++;
                progressRecs++;

                if (!odbf.Read(row, orec))
                  throw new Exception(string.Format("Failed to read row {0}", row + 1));

                if (!orec.IsDeleted)
                {
                  values.AppendFormat("EXECUTE bigInsert_{0}_{1} (", dbSchemaName, tableName);
                                    
                  for (var field = 0; field < orec.ColumnCount; field++)
                  {
                    values.AppendFormat("{0}{1}", field > 0 ? ", " : string.Empty,
                      PSQLUtils.PSQLStringyfy(orec[field], orec.Column(field).ColumnType, orec.Column(field).Length));
                  }
                  values.Append(");\r\n");

                  if (currRow >= BATCH_ROWS_SIZE)
                  {
                    lastSQL = values.ToString();
                    insertDataCommand.CommandText = lastSQL;
                    var rows = insertDataCommand.ExecuteNonQuery();
                    values.Clear();
                    tableRows += rows;
                    currRow = 0;
                    System.Threading.Thread.Sleep(200);  // Give postgresql some recovery time...
                  }

                  if (progressRecs >= PROGRESS_SHOW_RECS)
                  {
                    copyProgressMessages.Enqueue(string.Format("  [{0}] COPY- Copied {1:N0} rows of {2:N0}- {3:N2}%",
                      fileName, tableRows, recordCount, tableRows > 0 ? ((float)tableRows / (float)recordCount) * 100 : 100));
                    progressRecs = 0;
                  }
                }
              }

              if (currRow > 0)
              {
                insertDataCommand.CommandText = values.ToString();
                var rows = insertDataCommand.ExecuteNonQuery();
                values.Clear();
                progressRecs += rows;
                tableRows += rows;
                currRow = 0;
              }

              deallocateSQL = string.Format("DEALLOCATE bigInsert_{0}_{1};", dbSchemaName, tableName);
              insertDataCommand.CommandText = deallocateSQL;
              insertDataCommand.ExecuteNonQuery();

              var elapsedSeconds = tableTimer.Elapsed.TotalSeconds;
              copyProgressMessages.Enqueue(string.Format("  [{0}] COPY- Copied {1:N0} rows in {2:N2} seconds- {3:N2} recs/sec",
                fileName, tableRows, elapsedSeconds, elapsedSeconds > 0.1 && tableRows > 500 ? tableRows / elapsedSeconds : 0));
            }
          }
        }
        finally
        {
          odbf.Close();
        }

        return true;
      }

      catch (Exception err)
      {
        copyProgressMessages.Enqueue(string.Format("[{0}] COPY- >> FATAL << Error '{1}' at row {2}", fileName, err.Message, currRow));
        File.WriteAllLines(string.Format("{0}-{1}-{2:yyyy-MM-dd-HH-mm-ss}.sql", dbSchemaName, Path.GetFileName(dbfFileName), DateTime.Now), new string[] { prepareSQL, lastSQL, deallocateSQL });
        return false;
      }
    }

    #endregion

    
    #region Private vars

    /// <summary>
    /// How many rows to commit at a time? Higher value = more RAM used, but PSQL inserts faster
    /// </summary>
    private static readonly int BATCH_ROWS_SIZE = 4500;

    /// <summary>
    /// How often to report back on progress
    /// </summary>
    private static readonly int PROGRESS_SHOW_RECS = 10000;

    #endregion

  }
}
