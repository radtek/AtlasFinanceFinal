using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

using Npgsql;


namespace Atlas.Server.Utils
{
  internal static class CopyData
  {
    /// <summary>
    /// Utility to copy across selected rows from one PostgreSQL table to another
    /// </summary>
    /// <param name="selectSql">The SQL select statement</param>
    /// <param name="destTableName">The name of the table to write to</param>
    /// <param name="sourceConnectionString">The source connection string</param>
    /// <param name="destConnectionString">The destination connection string</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static void CopyAtlasCoreData(string selectSql, string destTableName, string sourceConnectionString, string destConnectionString)
    {
      using (var srcConn = new NpgsqlConnection(sourceConnectionString))
      using (var destConn = new NpgsqlConnection(destConnectionString))
      {
        srcConn.Open();
        destConn.Open();

        using (var srcCmd = srcConn.CreateCommand())
        using (var destCmd = destConn.CreateCommand())
        {
          srcCmd.CommandTimeout = 240;
          srcCmd.CommandText = selectSql;

          using (var rdr = srcCmd.ExecuteReader())
          {
            while (rdr.Read())
            {
              if (string.IsNullOrEmpty(destCmd.CommandText))
              {
                var fields = new List<Tuple<string, string>>();
                for (var i = 0; i < rdr.FieldCount; i++)
                {
                  fields.Add(new Tuple<string, string>(rdr.GetName(i), rdr.GetDataTypeName(i)));
                }

                destCmd.CommandText = string.Format("INSERT INTO \"{0}\" ({1}) VALUES ({2});", destTableName,
                  string.Join(",", fields.Select(s => string.Format("\"{0}\"", s.Item1))),
                  string.Join(",", fields.Select(s => string.Format("@{0}", s.Item1))));
                for (var i = 0; i < rdr.FieldCount; i++)
                {
                  var paramName = string.Format("{0}", rdr.GetName(i));
                  switch (rdr.GetDataTypeName(i))
                  {
                    case "bool":
                      destCmd.Parameters.Add(paramName, NpgsqlTypes.NpgsqlDbType.Boolean, 0);
                      break;

                    case "varchar":
                      destCmd.Parameters.Add(paramName, NpgsqlTypes.NpgsqlDbType.Varchar, 0);
                      break;
                      
                    case "bpchar":
                      destCmd.Parameters.Add(paramName, NpgsqlTypes.NpgsqlDbType.Char, 0);
                      break;
                      
                    case "bytea":
                      destCmd.Parameters.Add(paramName, NpgsqlTypes.NpgsqlDbType.Bytea, 0);
                      break;

                    case "int4":
                      destCmd.Parameters.Add(paramName, NpgsqlTypes.NpgsqlDbType.Integer, 0);
                      break;

                    case "int8":
                      destCmd.Parameters.Add(paramName, NpgsqlTypes.NpgsqlDbType.Bigint, 0);
                      break;

                    case "timestamp":
                      destCmd.Parameters.Add(paramName, NpgsqlTypes.NpgsqlDbType.Timestamp, 0);
                      break;

                    default:
                      throw new NotSupportedException(string.Format("Type {0} not supported", rdr.GetDataTypeName(i)));
                  }
                }
              }
              for (var i = 0; i < rdr.FieldCount; i++)
              {
                var paramName = string.Format("{0}", rdr.GetName(i));
                destCmd.Parameters[paramName].Value = rdr.GetValue(i);
              }

              destCmd.ExecuteNonQuery();
            }
          }
        }
      }
    }

  }
}
