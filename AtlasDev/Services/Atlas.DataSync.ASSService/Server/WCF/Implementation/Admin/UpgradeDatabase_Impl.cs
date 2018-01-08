using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

using DevExpress.Xpo;
using Npgsql;

using Atlas.Domain.Model;
using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Cache.DomainMapper;
using Atlas.Cache.DataUtils;


namespace ASSServer.WCF.Implementation.Admin
{
  public static class UpgradeDatabase_Impl
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config, 
      SourceRequest sourceRequest, string version, string description, string sqlUpdateScript)
    {
      var methodName = "UpgradeDatabase";

      try
      {
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          var transPosition = "Starting";
          var trans = conn.BeginTransaction();

          try
          {
            using (var cmd = conn.CreateCommand())
            {
              cmd.Transaction = trans;

              /*
               * We cannot execute script against 'company' schema in all cases:
               * -----------------------------------------------------------------
               *    For *new* master tables: sr_recid is *sequence* for master tables on company, while on branch it is not
               *    for *new* non-master tables: sr_recid is a sequence on branch, while on master it is not.
               *    
               * Simplification rules:
               * --------------------------
               *    Create all company (master and non-master) tables manually and the scripts must *only* be run against 
               *    each branch server and the brXXX schema's. sequences will be deleted from brXXX schemas.
               *  
               * 
              #region Execute script against the 'company' schema
              log.Information("Running update script against schema 'company'");

              transPosition = "Updating 'company' schema";
              // Remove all 
              cmd.CommandText = string.Format("SET search_path to company;\r\n{0}", sqlUpdateScript);
              cmd.CommandTimeout = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;
              cmd.ExecuteNonQuery();
              log.Information("Successfully updated schema 'company'");
              #endregion
              */
              #region Get listing of current brXXX schemas
              transPosition = "Getting list of brXXX schemas";
              var branchSchemaNames = new List<string>();
              log.Information("  Getting list of schemas");
              cmd.CommandText = "select schema_name from information_schema.schemata WHERE schema_name ~ '^br0[0-9A-Za-z]{1,1}[0-9]{1,1}$'";
              cmd.CommandType = CommandType.Text;

              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  branchSchemaNames.Add(rdr.GetString(0));
                }
              }
              #endregion

              #region Execute the upgrade script against all 'brXXX' schemas
              foreach (var schema in branchSchemaNames)
              {
                log.Information("Running update script against schema {0}", schema);
                transPosition = string.Format("Updating '{0}' schema", schema);
                cmd.CommandText = string.Format("SET search_path to {0}, public;\r\n{1}", schema, sqlUpdateScript);
                cmd.CommandTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds;
                cmd.ExecuteNonQuery();

                // Update lrep_db_info with version it is now running
                cmd.CommandText = string.Format(
                  "WITH upsert AS (UPDATE {0}.lrep_db_info SET \"value\" = '{1}' WHERE \"setting\" = 1 RETURNING *) " +
                  "INSERT INTO {0}.lrep_db_info (\"setting\", \"value\") SELECT 1, '{1}' WHERE NOT EXISTS (SELECT * FROM upsert)", schema, version);
                cmd.ExecuteNonQuery();

                log.Information("Successfully updated schema {Schema}", schema);
              }
              #endregion

              transPosition = "Committing brXXX transactions";              
              trans.Commit();

              #region Remove PSQL Triggers created by this script, as they only apply to the branch db              
              log.Information("Removing brXXX schema triggers");
              transPosition = "Reading brXXX schema trigger listing";
              // Triggers
              var dbTriggers = new List<Tuple<string, string, string>>();
              cmd.CommandText = "SELECT DISTINCT trigger_name, trigger_schema, event_object_table " +
                  "FROM information_schema.triggers " +
                  "WHERE trigger_schema ~ '^br*' AND trigger_schema NOT IN ('pg_catalog', 'information_schema');";
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  dbTriggers.Add(new Tuple<string, string, string>(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2)));
                }
              }
              foreach(var dbObject in dbTriggers)
              {
                transPosition = string.Format("Dropping trigger {0} on {1}.{2}", dbObject.Item1, dbObject.Item2, dbObject.Item3);
                cmd.CommandText = string.Format("DROP TRIGGER \"{0}\" ON {1}.{2};", dbObject.Item1, dbObject.Item2, dbObject.Item3);
                cmd.ExecuteNonQuery();
              }              
              #endregion

              #region Remove Sequences created by this script, as they only apply to the branch db
              log.Information("Removing brXXX schema sequences");
              transPosition = "Reading brXXX schema sequence listing";
              var dbSequences = new List<Tuple<string, string>>();
              cmd.CommandText = "select sequence_schema, sequence_name from information_schema.sequences where sequence_schema ~ '^br*'";
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  dbSequences.Add(new Tuple<string, string>(rdr.GetString(0), rdr.GetString(1)));
                }
              }
              foreach (var dbSequence in dbSequences)
              {
                transPosition = string.Format("Dropping sequence {0}.{1}", dbSequence.Item1, dbSequence.Item2);
                cmd.CommandText = string.Format("DROP SEQUENCE IF EXISTS {0}.{1} CASCADE;", dbSequence.Item1, dbSequence.Item2);
                cmd.ExecuteNonQuery();
              }              
              #endregion

              using (var uow = new UnitOfWork())
              {                
                #region Update ASS_DbUpdateScript
                transPosition = "Adding script to ASS_DbUpdateScript";
                var dbUpdScript = new ASS_DbUpdateScript(uow)
                {
                  CreatedDT = DateTime.Now,
                  DbVersion = version,
                  Description = description,
                  PreviousVersion = uow.Query<ASS_DbUpdateScript>().First(s => s.DbUpdateScriptId == CacheUtils.GetCurrentDbVersion(cache).DbUpdateScriptId),
                  UpdateScript = sqlUpdateScript
                };
                uow.CommitChanges();

                cache.Set<ASS_DbUpdateScript_VerString_Cached>(new[] { CacheDomainMapper.ASS_DbUpdateScriptVerString_Mapper(dbUpdScript) }); 
                #endregion

                uow.CommitChanges();

                // Add this script to the cache
                var scriptCache = CacheDomainMapper.ASS_DbUpdateScript_Mapper(dbUpdScript);
                cache.Set(new List<ASS_DbUpdateScript_Cached> { scriptCache });

                CacheUtils.SetCurrentDbVersion(cache, new ASS_DbUpdateScript_VerString_Cached { DBVersion = scriptCache.DbVersion, DbUpdateScriptId = scriptCache.DbUpdateScriptId });
                                   
                // Update servers to use this version         
                var servers = cache.GetAll<ASS_BranchServer_Cached>().Where(s => s.Branch != null && s.Machine != null).ToList();
                var foundVer = cache.GetAll<ASS_DbUpdateScript_VerString_Cached>()?.FirstOrDefault(s => s.DBVersion == version);
                
                foreach (var server in servers)
                {
                  server.UseDBVersion = dbUpdScript.DbUpdateScriptId;
                }                
                cache.Set(servers);
              }
            }
          }
          catch (Exception err)
          {
            log.Error(err, "{MethodName} @ {Position}", methodName, transPosition);
            trans.Rollback();
          }
        } 
            
        return true;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        return false;
      }
    }

  }
}
