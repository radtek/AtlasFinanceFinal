using System;
using System.Data;
using System.Linq;

using Npgsql;

using Atlas.Common.Utils;
using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;
using Atlas.Cache.DataUtils;
using ASSServer.Utils.Serialization;


namespace ASSServer.WCF.Implementation.Admin
{
  public static class GetMasterTableData_Impl
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static byte[] Execute(ILogging log, ICacheServer cache, IConfigSettings config,
      SourceRequest sourceRequest, string masterTableName)
    {
      var methodName = "GetMasterTableData";
    
      try
      {
        #region Check parameters
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Error(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return null;
        }

        if (!CacheUtils.GetServerTableNames(cache).Any(s => s == masterTableName))
        {
          log.Error(new ArgumentException(string.Format("Table {MasterTableName} does not exist or is not master table", masterTableName), "dataRow"), methodName);
          return null;
        }
        #endregion

        #region Get table to a DataTable
        var dataTable = new DataTable();
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = string.Format("SELECT * FROM \"company\".{0} WHERE sr_deleted = ' '", masterTableName);
            cmd.CommandType = CommandType.Text;

            using (var adapter = new NpgsqlDataAdapter(cmd))
            {
              adapter.FillSchema(dataTable, SchemaType.Mapped);
              adapter.Fill(dataTable);
            }
          }
        }
        #endregion

        #region Remove 'indkey_xxx' synthetic index columns
        for (int i = dataTable.Columns.Count - 1; i >= 0; i--)
        {
          if (dataTable.Columns[i].ColumnName.StartsWith("indkey_"))
          {
            dataTable.Columns.RemoveAt(i);
          }
        }
        #endregion

        #region Decrypt asstmast columns
        if (masterTableName == "asstmast")
        {
          dataTable.Columns["oper"].Unique = true;
          dataTable.Columns["oper"].AllowDBNull = false;
          dataTable.Columns["firstname"].AllowDBNull = false;

          foreach (DataRow row in dataTable.Rows)
          {
            row["oper"] = ClipperCrypto.ASSEncrypt((string)row["oper"], -1);
            if (row["firstname"] != null && row["firstname"] != DBNull.Value)
            {
              row["firstname"] = ClipperCrypto.ASSEncrypt((string)row["firstname"], -1);
            }
            if (row["surname"] != null && row["surname"] != DBNull.Value)
            {
              row["surname"] = ClipperCrypto.ASSEncrypt((string)row["surname"], -1);
            }

            if (row["identno"] != null && row["identno"] != DBNull.Value)
            {
              row["identno"] = ClipperCrypto.ASSEncrypt((string)row["identno"], -1);
            }

            if (row["level"] != null && row["level"] != DBNull.Value)
            {
              row["level"] = ClipperCrypto.ASSEncrypt((string)row["level"], -1);
            }
          }
          dataTable.AcceptChanges();
        }
        else if (masterTableName == "asstbran")
        {
          foreach (DataRow row in dataTable.Rows)
          {
            row["oper"] = ClipperCrypto.ASSEncrypt((string)row["oper"], -1);
            if (row["password"] != null && row["password"] != DBNull.Value)
            {
              row["password"] = ClipperCrypto.ASSEncrypt((string)row["password"], -1);
            }
          }
        }
        #endregion

        return (!string.IsNullOrEmpty(sourceRequest.AppVer) && string.Compare(sourceRequest.AppVer, "1.2.0.0") >= 0) ? 
          FastJsonSerializer.SerializeToBytesJson(dataTable, true) : 
          Serialization.SerializeToBytes(dataTable, true);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);

        return null;
      }
    }
  }
}
