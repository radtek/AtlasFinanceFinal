using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Quartz;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using ASSServer.Utils.PSQL;


namespace ASSServer.QuartzTasks
{
  public class AssCopyMasterDataToBrSchemas : IJob
  {
    public AssCopyMasterDataToBrSchemas(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    public void Execute(IJobExecutionContext context)
    {
      _log.Information("AssCopyMasterDataToBrSchemas.Execute starting");
      try
      {
        #region Get 'brXXX' schemas
        var schemas = new List<string>();
        using (var conn = new Npgsql.NpgsqlConnection(_config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "select schema_name from information_schema.schemata WHERE schema_name ~ '^br0[0-9A-Za-z]{1,1}[0-9]{1,1}$'";
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                schemas.Add(rdr.GetString(0));
              }
            }
          }
        }
        #endregion

        var progress = new ConcurrentQueue<string>();
        string message;
        foreach (var schema in schemas)
        {
          _log.Information("Copy master tables from company to schema {schema}..", schema);
          CopyMasterTablesToBranch.Execute(_cache, _config, schema, progress);
          while (progress.TryDequeue(out message))
          {
            _log.Information(message);
          }
          _log.Information("Copied master tables from company to schema {schema}", schema);
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "AssCopyMasterDataToBrSchemas.Execute");
      }

      _log.Information("AssCopyMasterDataToBrSchemas completed");
    }


    #region Private fields

    private ILogging _log;
    private IConfigSettings _config;
    private ICacheServer _cache;

    #endregion
  }  
}
