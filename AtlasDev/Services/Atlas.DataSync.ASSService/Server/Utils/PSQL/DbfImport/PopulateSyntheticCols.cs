/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Populates SQLRDD synthetic columns with data
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
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;

using Npgsql;

using Atlas.Common.Interface;


namespace ASSServer.Utils.PSQL.DbfImport
{
  /// <summary>
  /// Class function to populate the values of 'synthetic' SQLRDD columns
  /// </summary>
  public static class PopulateSyntheticCols
  {
    /// <summary>
    /// Creates synthetic index column data in SQL
    /// </summary>
    /// <param name="tableName">The table name</param>
    /// <param name="dbSchemaName">The PostgreSQL schema name</param>
    /// <param name="progressMessages">Feedback</param>
    
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(IConfigSettings config, string tableName, string dbSchemaName, ConcurrentQueue<string> progressMessages)
    {
      try
      {
        var syntheticTimer = new Stopwatch();
        syntheticTimer.Start();

        progressMessages.Enqueue(string.Format("  [{0}]  Create synthetic index process started", tableName));

        var expressions = new List<string>();
        using (var sqlConn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          sqlConn.Open();

          using (var cmd = sqlConn.CreateCommand())
          {
            #region Get fixed character field names and character length for the table
            var charFields = new List<Tuple<string, int>>();
            cmd.CommandText = string.Format(
              "SELECT column_name, character_maximum_length " +
              "FROM information_schema.columns " +
              "WHERE table_schema = '{0}' AND table_name = '{1}' AND data_type = 'character'", dbSchemaName, tableName.ToLower());
            cmd.CommandType = CommandType.Text;
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                charFields.Add(new Tuple<string, int>(rdr.GetString(0), rdr.GetInt32(1)));
              }
            }
            #endregion

            // Get synthetic index definitions (xbase expression)
            cmd.CommandText = string.Format("SELECT lower(trim(both from \"table_\")), lower(trim(both from \"idxkey_\")), lower(trim(both from idxcol_)) " +
              "FROM \"{0}\".\"sr_mgmntindexes\" " +
              "WHERE \"idxcol_\" LIKE '00%' " +
              "AND lower(\"table_\") = '{1}' ORDER BY \"table_\", \"tagnum_\"", dbSchemaName, tableName.ToLower());
            cmd.CommandType = CommandType.Text;

            #region Convert Clipper expression to PostgreSQL equivalent
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                // 1.  Lowercase
                var expression = rdr.GetString(1).ToLower();

                var sourceExpression = expression;

                // 2. Replace + with ||
                expression = expression.Replace("+", " || ");

                // Replace +" "+ expression with +' '+
                expression = expression.Replace("+\" \"+", " || ' ' || ");

                // 3. Remove all "
                expression = expression.Replace("\"", "");

                // All character fields must be right padded- fieldname becomes rpad(COALESCE("fieldname", ''), character_len, ' ')
                foreach (var charField in charFields)
                {
                  // Replace whole words only
                  var regEx = new Regex(string.Format(@"\b{0}\b", charField.Item1));
                  if (expression.Contains(charField.Item1))
                  {
                    expression = regEx.Replace(expression, string.Format("rpad(coalesce(\"{0}\", ''), {1}, ' ')", charField.Item1, charField.Item2));
                  }
                }

                #region Prefix custom PSQL functions with 'public.'
                var sqlFunc = new Regex(@"\bencrypt\b", RegexOptions.IgnoreCase);
                expression = sqlFunc.Replace(expression, "public.encrypt");
                sqlFunc = new Regex(@"\bstr\b", RegexOptions.IgnoreCase);
                expression = sqlFunc.Replace(expression, "public.str");
                sqlFunc = new Regex(@"\bdtos\b", RegexOptions.IgnoreCase);
                expression = sqlFunc.Replace(expression, "public.dtos");
                // The 'order' fieldname must have " "
                sqlFunc = new Regex(@"\border\b", RegexOptions.IgnoreCase);
                expression = sqlFunc.Replace(expression, "\"order\"");
                #endregion

                expression = expression + ", public.str(\"sr_recno\"::NUMERIC, 15::INTEGER)";

                progressMessages.Enqueue(string.Format("[{0}]  Source expression- '{1}', Dest: '{2}'", tableName, sourceExpression, expression));

                var sql = string.Format("UPDATE \"{0}\".\"{1}\" SET indkey_{2} = CONCAT({3});", dbSchemaName, rdr.GetString(0), rdr.GetString(2), expression);
                expressions.Add(sql);

              }
            }
            #endregion

            #region Execute the SQL update to populate the synthetic index columns with data
            foreach (var sql in expressions)
            {
              progressMessages.Enqueue(string.Format("  [{0}] Creating synthetic index {1}", tableName, sql));

              var timer = new Stopwatch();
              timer.Start();

              cmd.CommandText = sql;
              cmd.CommandType = CommandType.Text;
              cmd.CommandTimeout = 900;
              cmd.ExecuteNonQuery();

              progressMessages.Enqueue(string.Format("[{0}]  Create synthetic index process completed in {1:N2} seconds", tableName, timer.Elapsed.TotalSeconds));
              Thread.Sleep(200); // Give PostgreSQL some breathing room....
            }
            #endregion
          }
        }

        progressMessages.Enqueue(string.Format("[{0}]  Create synthetic index process completed in {1:N2} seconds", tableName, syntheticTimer.Elapsed.TotalSeconds));

        return true;
      }
      catch (Exception err)
      {
        progressMessages.Enqueue(string.Format("[{0}] >> FATAL << Error: '{1}'", tableName, err.Message));
        return false;
      }
    }

  }
}
