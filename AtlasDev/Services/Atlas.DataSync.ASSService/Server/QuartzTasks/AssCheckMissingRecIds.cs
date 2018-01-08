using System;
using System.Linq;
using System.Collections.Generic;

using Atlas.Common.Interface;


namespace ASSServer.QuartzTasks
{
  /// <summary>
  /// TODO: This is incomplete!
  /// </summary>
  [global::Quartz.DisallowConcurrentExecution]
  public class AssCheckMissingRecIds : global::Quartz.IJob
  {
    public AssCheckMissingRecIds(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "CheckReplication.Execute";
      try
      {
        _log.Information("{MethodName} starting", methodName);

        //var tablesWithGaps = FindRecordGaps();
        // TODO: Use this...


        _log.Information("{MethodName task completed", methodName);
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }
    }


    /// <summary>
    /// Scans all schemas and finds gaps in - returns schema/table/missing 
    /// </summary>
    /// <returns>List containing schema/branch/missing recids</returns>
    private List<Tuple<string, string, List<Int64>>> FindRecordGaps()
    {
      // Issues- schema, table, list of gaps
      var recIdsMissing = new List<Tuple<string, string, List<Int64>>>();

      // Tables with Item 1-Schema, Item 2- Table, 3- Last value of sr_recid alerted
      var tablesWithLastRecId = new List<Tuple<string, string, Int64>>();

      // Item 1-Schema, Item 2- Table, 3- sr_recid alerted
      var maxCurrRecIds = new Dictionary<string, Int64>(); // Key is schema|table

      using (var conn = new Npgsql.NpgsqlConnection(_config.GetAssConnectionString()))
      {
        conn.Open();

        var schemas = new List<string>();
        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandType = System.Data.CommandType.Text;

          #region Get schemas
          cmd.CommandText = "select schema_name from information_schema.schemata WHERE schema_name ~ '^br0[0-9A-Za-z]{1,1}[0-9]{1,1}$'";
          using (var rdr = cmd.ExecuteReader())
          {
            while (rdr.Read())
            {
              schemas.Add(rdr.GetString(0));
            }
          }
          #endregion

          #region Get tables for each schema
          var tempTables = new List<Tuple<string, string>>();
          foreach (var schema in schemas)
          {
            cmd.CommandText = string.Format(
              "select distinct table_name from information_schema.columns where column_name = 'sr_recno' and table_schema = '{0}'", schema);
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                tempTables.Add(new Tuple<string, string>(schema, rdr.GetString(0)));
              }
            }
          }
          #endregion
          
          #region Get last recid accepted as checked          
          var lastRecIds = new List<Tuple<string, string, Int64>>();
          cmd.CommandText = "SELECT schema, table_name, last_sr_recno FROM company.lrep_rec_lastid";
          using (var rdr = cmd.ExecuteReader())
          {
            while (rdr.Read())
            {
              lastRecIds.Add(new Tuple<string,string,long>(rdr.GetString(0), rdr.GetString(1), rdr.GetInt64(2)));
            }
          }
          foreach(var tempTable in tempTables)
          {
            var lastRecId = lastRecIds.FirstOrDefault(s => s.Item1 == tempTable.Item1 && s.Item2 == tempTable.Item2);
            tablesWithLastRecId.Add(new Tuple<string,string,long>(tempTable.Item1, tempTable.Item2, lastRecId != null ? lastRecId.Item3 : 0));
          }
          #endregion

          #region Find missing records
          foreach (var table in tablesWithLastRecId)
          {
            // Get recid we already alerted
            var lastTableTuple = tablesWithLastRecId.FirstOrDefault(s => s.Item1 == table.Item1 && s.Item2 == table.Item2);
            var lastRecIdVal = lastTableTuple != null ? lastTableTuple.Item3 : 0;

            var key = string.Format("{0}|{1}", table.Item1, table.Item2);                
            
            // Generate series using the min/max values for sr_recno for this table and return the gaps
            cmd.CommandText = string.Format(
              "WITH seq_max AS (SELECT max(sr_recno::bigint) FROM {0}.{1}),seq_min AS (SELECT min(sr_recno::bigint) FROM {0}.{1}) " +
              "SELECT * FROM generate_series((SELECT min FROM seq_min),(SELECT max FROM seq_max)) EXCEPT SELECT sr_recno::bigint FROM {0}.{1}", table.Item1, table.Item2);

            using (var rdr = cmd.ExecuteReader())
            {
              if (rdr.HasRows)
              {
                var records = new List<Int64>();
                while (rdr.Read())
                {
                  var missingRecId = rdr.GetInt64(0);

                  if (lastRecIdVal < missingRecId)
                  {
                    records.Add(missingRecId);
                    // Mark highest recid for this table, so we don't alert next time this schedule is run
                    var currVal = (maxCurrRecIds.ContainsKey(key)) ? maxCurrRecIds[key] : 0;
                    if (currVal < missingRecId)
                    {
                      maxCurrRecIds[key] = currVal;
                    }                  
                  }                  
                }

                if (records.Count > 0)
                {
                  recIdsMissing.Add(new Tuple<string, string, List<long>>(table.Item1, table.Item2, records));
                }
              }
            }
          }
          #endregion
        }
      }

      return recIdsMissing;
    }


    #region Private fields

    private readonly ILogging _log;
    private readonly IConfigSettings _config;

    #endregion

  }
}
