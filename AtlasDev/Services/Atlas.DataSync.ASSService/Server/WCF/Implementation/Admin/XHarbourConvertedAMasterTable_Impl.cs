using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

using DevExpress.Xpo;
using Npgsql;

using Atlas.Domain.Model;
using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Cache.DomainMapper;


namespace ASSServer.WCF.Implementation.Admin
{
  public static class XHarbourConvertedAMasterTable_Impl
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest, string tableName)
    {
      var methodName = "XHarbourConvertedAMasterTable";

      try
      {
        using (var unitOfWork = new UnitOfWork())
        {
          #region Add record change tracking for every row uploaded
          using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
          {
            conn.Open();

            #region Read each row of the table and mark that it was changed
            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandText = string.Format("SELECT sr_recno FROM company.\"{0}\"", tableName);
              cmd.CommandType = CommandType.Text;

              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  var tracking = new ASS_MasterTableChangeTracking(unitOfWork)
                  {
                    ChangedTS = DateTime.Now,
                    TableName = tableName,
                    KeyFieldName = "sr_recno",
                    KeyFieldValue = rdr.GetDecimal(0).ToString("F0")
                  };
                }
              }
            }
            #endregion
          }
          #endregion

          #region Update the domain with master table
          var masterTable = unitOfWork.Query<ASS_ServerTable>().FirstOrDefault(s => s.TableName == tableName);
          if (masterTable == null)
          {
            masterTable = new ASS_ServerTable(unitOfWork)
            {
              TableName = tableName,
              Description = tableName,
              LiveUpdates = false,
              LastUpdatedDT = DateTime.Now
            };
          }
          else
          {
            masterTable.LastUpdatedDT = DateTime.Now;
          }
          #endregion

          unitOfWork.CommitChanges();                
   
          cache.Set(new List<ASS_ServerTable_Cached> { CacheDomainMapper.ASS_ServerTable_Mapped(masterTable) });         
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
