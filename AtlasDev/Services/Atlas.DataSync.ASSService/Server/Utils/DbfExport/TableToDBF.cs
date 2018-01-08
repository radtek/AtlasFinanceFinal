using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SocialExplorer.IO.FastDBF;

using Atlas.Common.Interface;


namespace Atlas.Services.DbfGenerate
{
  public static class TableToDBF
  {
    public static void Execute(ILogging log, string connectionString, string schemaName, string tableName, string destDBFilePath)
    {      
      var tableInfo = new List<Tuple<string, string, int, byte, byte>>(); // Tuple- FieldName, Type, MaxLen, Precision, Scale

      using (var conn = new Npgsql.NpgsqlConnection(connectionString))
      {
        conn.Open();
        using (var cmd = conn.CreateCommand())
        {
          #region Get field info
          cmd.CommandText = string.Format(
            "SELECT column_name, data_type, character_maximum_length, numeric_precision, numeric_scale " +
            "FROM information_schema.columns " +
            "WHERE table_name='{0}' AND table_schema='{1}' " +
            "ORDER BY ordinal_position; ", tableName, schemaName);
          using (var rdr = cmd.ExecuteReader())
          {
            while (rdr.Read())
            {
              var fieldName = rdr.GetString(0);
              if (fieldName != "sr_recno" && fieldName != "sr_deleted" && fieldName != "recid" && !fieldName.StartsWith("indkey_"))
              {
                tableInfo.Add(new Tuple<string, string, int, byte, byte>(fieldName, rdr.GetString(1),
                  rdr.IsDBNull(2) ? 0 : rdr.GetInt32(2),
                  rdr.IsDBNull(3) ? (byte)0 : (byte)rdr.GetInt32(3),
                  rdr.IsDBNull(4) ? (byte)0 : (byte)rdr.GetInt32(4)));
              }
            }
          }
          #endregion

          var recs = 0;
          string lastVal = null;
          string sqlLastFieldName = null;
          string dbfLastFieldName = null;
          if (File.Exists(destDBFilePath))
          {
            File.Delete(destDBFilePath);
          }
          var odbf = new DbfFile(System.Text.ASCIIEncoding.ASCII);
          odbf.Open(destDBFilePath, FileMode.Create);

          try
          {
            #region Add fields
            foreach (var field in tableInfo)
            {
              // Don't copy SQLRDD fields and our custom sequence
              if (!field.Item1.StartsWith("indkey_") && field.Item1 != "sr_deleted" && field.Item1 != "sr_recno")
              {
                switch (field.Item2)
                {
                  case "character":
                  case "character varying":
                    odbf.Header.AddColumn(new DbfColumn(field.Item1, DbfColumn.DbfColumnType.Character, field.Item3, 0));
                    break;

                  case "date":
                    odbf.Header.AddColumn(new DbfColumn(field.Item1, DbfColumn.DbfColumnType.Date));
                    break;

                  case "numeric":
                    odbf.Header.AddColumn(new DbfColumn(field.Item1, DbfColumn.DbfColumnType.Number, field.Item4, field.Item5));
                    break;

                  case "boolean":
                    odbf.Header.AddColumn(new DbfColumn(field.Item1, DbfColumn.DbfColumnType.Boolean));
                    break;
                    
                  //case "bigint":
                  //  throw new ArgumentException(string.Format("Field {0}"))

                  case "integer":
                    odbf.Header.AddColumn(new DbfColumn(field.Item1, DbfColumn.DbfColumnType.Number, 15, 0));
                    break;
                                      
                  default:
                    throw new Exception(string.Format("Unexpected data type: '{0}'- Table: {1}, field: '{2}'", 
                      field.Item2, tableName, field.Item1));
                }
              }
            }
            #endregion

            #region Add data
            DbfRecord orec = new DbfRecord(odbf.Header)
            {
              AllowDecimalTruncate = false,
              AllowIntegerTruncate = false,
              AllowStringTurncate = false
            };

            using (var sqlCmd = conn.CreateCommand())
            {
              sqlCmd.CommandText = string.Format("SELECT * FROM {0}.\"{1}\" where sr_deleted = ' ' ORDER BY sr_recno",
                schemaName, tableName); // TODO: Sequential table scan.... no index... on sr_deleted?
              sqlCmd.CommandType = System.Data.CommandType.Text;
              sqlCmd.CommandTimeout = (int)TimeSpan.FromMinutes(10).TotalSeconds;

              var fldIdx = new List<int>();
              using (var sqlRdr = sqlCmd.ExecuteReader())
              {
                if (fldIdx.Count == 0) // Build field index match
                {
                  for (int i = 0; i < sqlRdr.FieldCount; i++)
                  {
                    fldIdx.Add(orec.FindColumn(sqlRdr.GetName(i)));
                  }
                }

                while (sqlRdr.Read())
                {
                  for (int i = 0; i < sqlRdr.FieldCount; i++)
                  {
                    var index = fldIdx[i];
                    if (index > -1)
                    {
                      sqlLastFieldName = sqlRdr.GetName(i);
                      dbfLastFieldName = string.Format("[{0}]- '{1}' ({2}, {3})",
                        orec.Header[index].Name, orec.Header[index].ColumnTypeChar, orec.Header[index].Length, orec.Header[index].DecimalCount);
                      lastVal = Atlas.Data.DBF.Utils.DBFUtils.DBFStringyfy(sqlRdr.GetValue(i), (byte)orec.Header[index].DecimalCount);
                      orec[index] = lastVal;
                    }
                  }
                  odbf.Write(orec, true);
                  recs++;
                  if (recs % 5000 == 0)
                  {
                    log.Information("Table {Schema}.{Table}- DBF records written: {Records:N0}", schemaName, tableName, recs);
                  }
                }
              }
            }
            #endregion
          }
          finally
          {
            odbf.WriteHeader();
            odbf.Close();
            odbf = null;            
          }
        }
      }
    }
  }
}
