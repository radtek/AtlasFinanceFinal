using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using DevExpress.Xpo;
using Npgsql;

using Atlas.Common.Utils;
using Atlas.Data.Utils;
using Atlas.DataSync.WCF.Interface;
using Atlas.Domain.Model;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;
using Atlas.Cache.DomainMapper;
using Atlas.Cache.DataUtils;


namespace ASSServer.WCF.Implementation.Admin
{
  public static class UploadMasterTableRow_Impl
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config,
      SourceRequest sourceRequest, string masterTableName, byte[] dataTable)
    {
      var methodName = "UploadMasterTableRow";
      try
      {
        #region Check parameters
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Error(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return false;
        }
        if (string.IsNullOrEmpty(masterTableName))
        {
          log.Error(new ArgumentNullException("masterTableName"), methodName);
          return false;
        }
        if (dataTable == null)
        {
          log.Error(new ArgumentNullException("dataTable"), methodName);
          return false;
        }

        #region Check dataTable
        if (dataTable.Length == 0 || dataTable.Length > 65000)
        {
          log.Error(new ArgumentException(string.Format("Invalid length: {Length} (too large or empty)", dataTable.Length), "dataTable"), methodName);
          return false;
        }
        #endregion

        #region Deserialize
        DataTable data;
        try
        {
          data = (!string.IsNullOrEmpty(sourceRequest.AppVer) && string.Compare(sourceRequest.AppVer, "1.2.0.0") >= 0) ?
            Utils.Serialization.FastJsonSerializer.DeserializeFromBytesJson<DataTable>(dataTable, true) :
            (DataTable)Serialization.DeserializeFromBytes(dataTable, true);
        }
        catch (Exception err)
        {
          log.Error(new ArgumentException(string.Format("Invalid format: '{0}'", err.Message), "dataRow"), methodName);
          return false;
        }
        #endregion

        #region Check is a master table
        if (!CacheUtils.GetServerTableNames(cache).Any(s => s == masterTableName))
        {
          log.Error(new ArgumentException(string.Format("Table '{0}' does not exist or is not a master table", masterTableName), "dataRow"), methodName);
          return false;
        }
        #endregion

        #endregion

        #region Pad all strings to their field length (required for SQLRDD synthetic + normal indexes) & set null booleans to false
        var stringCols = new Dictionary<string, int>();
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();
          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = string.Format(
              "SELECT column_name, character_maximum_length FROM information_schema.columns WHERE table_name = '{0}' AND table_schema = 'company' and data_type='character'", masterTableName);
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                if (!rdr.GetString(0).StartsWith("sr_"))
                {
                  stringCols.Add(rdr.GetString(0), rdr.GetInt32(1));
                }
              }
            }
          }
        }

        var booleanCols = data.Columns.Cast<DataColumn>().Where(s => s.DataType == typeof(bool)).ToList();
        for (var i = 0; i < data.Rows.Count; i++)
        {
          var row = data.Rows[i];
          if ((string)row["RowState"] == "Added" || (string)row["RowState"] == "Modified")
          {
            foreach (var colName in stringCols.Keys)
            {
              if (row[colName] != null && row[colName] != DBNull.Value)
              {
                row[colName] = ((string)row[colName]).PadRight(stringCols[colName], ' ');
              }
            }

            // Make any null booleans have a default of 'false'
            foreach (DataColumn col in booleanCols)
            {
              if (row[col] == null || row[col] == DBNull.Value)
              {
                row[col] = false;
              }
            }
          }
        }
        #endregion

        if (masterTableName == "asstmast")
        {
          #region Update PER_Person and PER_Security
          using (var unitOfWork = new UnitOfWork())
          {
            var blockCol = data.Columns.IndexOf("block");
            var blockedCol = data.Columns.IndexOf("blocked");
            var employeeType = unitOfWork.Query<PER_Type>().First(s => s.Type == Atlas.Enumerators.General.PersonType.Employee);

            for (var i = 0; i < data.Rows.Count; i++)
            {
              var row = data.Rows[i];
              if ((string)row["RowState"] == "Added" || (string)row["RowState"] == "Modified") // edited a user- update PER_...
              {
                var isBlocked = (row[blockCol] != DBNull.Value) ? (bool)row[blockCol] : false;
                if (!isBlocked && row[blockedCol] != DBNull.Value)
                {
                  isBlocked = (string)row[blockedCol] == "Y";
                }

                var operatorId = ((string)row["oper"]).TrimEnd();
                var firstName = ((string)row["firstname"]).TrimEnd();
                var lastName = ((string)row["surname"]).TrimEnd();
                var idNum = ((string)row["identno"]).TrimEnd();

                var personDb = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.IdNum == idNum);
                if (personDb == null)
                {
                  personDb = new PER_Person(unitOfWork)
                  {
                    PersonType = employeeType,
                    CreatedDT = DateTime.Now,
                  };
                }

                var securityDb = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.Person == personDb);
                if (securityDb == null)
                {
                  securityDb = new PER_Security(unitOfWork)
                  {
                    Person = personDb,
                    CreatedDT = DateTime.Now
                  };
                  personDb.Security = securityDb;
                }

                // Block/Unblocked user?
                if (securityDb.IsActive != !isBlocked)
                {
                  securityDb.IsActive = !isBlocked;
                }

                securityDb.LegacyOperatorId = operatorId;
                securityDb.IsActive = !isBlocked;
                // Changed ID/Security/etc.
                personDb.Firstname = firstName;
                personDb.Lastname = lastName;
                personDb.IdNum = idNum;
                personDb.LastEditedDT = DateTime.Now;

                // In case was a client, ensure now a consultant
                personDb.PersonType = unitOfWork.Query<PER_Type>().First(s => s.Type == Atlas.Enumerators.General.PersonType.Employee);

                unitOfWork.CommitChanges();
                cache.Set(new List<PER_Person_Cached> { CacheDomainMapper.Per_Person_Mapper(personDb) });
              }
            }
          }
          #endregion

          #region Re-encrypt fields if asstmast
          for (var i = 0; i < data.Rows.Count; i++)
          {
            var row = data.Rows[i];
            if ((string)row["RowState"] == "Added" || (string)row["RowState"] == "Modified")
            {
              row["oper"] = ClipperCrypto.ASSEncrypt((string)row["oper"], 1);
              if (row["firstname"] != null && row["firstname"] != DBNull.Value)
              {
                row["firstname"] = ClipperCrypto.ASSEncrypt((string)row["firstname"], 1);
              }
              if (row["surname"] != null && row["surname"] != DBNull.Value)
              {
                row["surname"] = ClipperCrypto.ASSEncrypt((string)row["surname"], 1);
              }

              if (row["identno"] != null && row["identno"] != DBNull.Value)
              {
                row["identno"] = ClipperCrypto.ASSEncrypt((string)row["identno"], 1);
              }

              if (row["level"] != null && row["level"] != DBNull.Value)
              {
                row["level"] = ClipperCrypto.ASSEncrypt((string)row["level"], 1);
              }
            }
          }
          #endregion
        }
        else if (masterTableName == "asstbran")
        {
          for (var i = 0; i < data.Rows.Count; i++)
          {
            var row = data.Rows[i];
            if ((string)row["RowState"] == "Added" || (string)row["RowState"] == "Modified")
            {
              row["oper"] = ClipperCrypto.ASSEncrypt((string)row["oper"], 1);
              row["password"] = ClipperCrypto.ASSEncrypt((string)row["password"], 1);
            }
          }
        }

        #region Get SQL to set synthetic columns's values from expressions
        var editExpressionsFieldValues = new List<string>();

        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = string.Format("SELECT lower(trim(both from \"idxkey_\")), lower(trim(both from idxcol_)) " +
                  "FROM \"company\".\"sr_mgmntindexes\" " +
                  "WHERE \"idxcol_\" LIKE '00%' " +
                  "AND lower(\"table_\") = '{0}' ORDER BY \"table_\", \"tagnum_\"", masterTableName.ToLower());

            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                var expression = rdr.GetString(0).ToLower();
                // 2. Replace + with ||
                expression = expression.Replace("+", " || ");

                // Replace +" "+ expression with +' '+
                expression = expression.Replace("+\" \"+", " || ' ' || ");

                // 3. Remove all "
                expression = expression.Replace("\"", "");

                // All character fields must be right padded- fieldname becomes rpad(COALESCE("fieldname", ''), character_len, ' ')
                foreach (var fieldName in stringCols.Keys)
                {
                  // Replace whole words only
                  var regEx = new System.Text.RegularExpressions.Regex(string.Format(@"\b{0}\b", fieldName));
                  if (expression.Contains(fieldName))
                  {
                    expression = regEx.Replace(expression, string.Format("rpad(coalesce(\"{0}\", ''), {1}, ' ')", fieldName, stringCols[fieldName]));
                  }
                }

                #region Prefix custom PSQL functions with 'public.'
                var sqlFunc = new System.Text.RegularExpressions.Regex(@"\bencrypt\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                expression = sqlFunc.Replace(expression, "public.encrypt");
                sqlFunc = new System.Text.RegularExpressions.Regex(@"\bstr\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                expression = sqlFunc.Replace(expression, "public.str");
                sqlFunc = new System.Text.RegularExpressions.Regex(@"\bdtos\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                expression = sqlFunc.Replace(expression, "public.dtos");
                // The 'order' fieldname must be delimited to avoid the SQL reserved word
                sqlFunc = new System.Text.RegularExpressions.Regex(@"\border\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                expression = sqlFunc.Replace(expression, "\"order\"");
                #endregion

                expression = expression + ", public.str(\"sr_recno\"::NUMERIC, 15::INTEGER)";

                editExpressionsFieldValues.Add(string.Format("indkey_{0} = CONCAT({1})", rdr.GetString(1), expression));
              }
            }
          }
        }
        #endregion

        #region Effect the update...
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          // Build SQL insert CSV fieldnames string
          var insertFieldNames = string.Join(",", data.Columns.Cast<DataColumn>()
            .Where(s => s.ColumnName != "sr_recno" && s.ColumnName != "RowState")
            .Select(s => string.Format("\"{0}\"", s.ColumnName)));

          var trans = conn.BeginTransaction();

          try
          {
            using (var cmd = conn.CreateCommand())
            {
              cmd.Transaction = trans;
              for (var i = 0; i < data.Rows.Count; i++)
              {
                var row = data.Rows[i];
                var sr_recno = (Decimal)row["sr_recno"];
                if ((string)row["RowState"] == "Added" || (string)row["RowState"] == "Modified")
                {
                  if ((string)row["RowState"] == "Added" || sr_recno <= 0)
                  {
                    #region No existing record- insert
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("INSERT INTO \"company\".\"{0}\" ({1}) VALUES ({2}) RETURNING sr_recno", masterTableName,
                      insertFieldNames,
                      string.Join(",", data.Columns.Cast<DataColumn>()
                        .Where(s => s.ColumnName != "sr_recno" && s.ColumnName != "RowState")
                        .Select(s => PostgresUtils.PSQLStringyfy(row[s.ColumnName]))));

                    log.Information("{0}-Insert SQL:'{1}'", methodName, cmd.CommandText);

                    using (var rdr = cmd.ExecuteReader())
                    {
                      if (rdr.Read())
                      {
                        row["sr_recno"] = rdr.GetDecimal(0);
                      }
                      else
                      {
                        log.Error(methodName, new Exception("Error reading inserted columns- no data"));
                        return false;
                      }
                    }
                    #endregion
                  }
                  else
                  {
                    #region Update row
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format(
                      "UPDATE \"company\".\"{0}\" SET {1} WHERE sr_recno = {2:F0}", masterTableName,
                      string.Join(",",
                        data.Columns.Cast<DataColumn>()
                          .Where(s => s.ColumnName != "sr_recno" && s.ColumnName != "RowState")
                          .Select(s => string.Format("\"{0}\" = {1}", s.ColumnName, PostgresUtils.PSQLStringyfy(row[s.ColumnName])))),
                      row["sr_recno"]);
                    log.Information("{0}-Update SQL:'{1}'", methodName, cmd.CommandText);
                    cmd.ExecuteNonQuery();
                    #endregion
                  }

                  if (editExpressionsFieldValues.Count > 0)
                  {
                    foreach (var expression in editExpressionsFieldValues)
                    {
                      // Do synthetic columns- we do this after, so we have sr_recno for inserts and the SQL field values                    
                      cmd.CommandText = string.Format(
                        "UPDATE \"company\".\"{0}\" SET {1} WHERE sr_recno = {2:F0}",
                        masterTableName, expression, row["sr_recno"]);
                      log.Information("{0}-Update Synthetic SQL:'{1}'", methodName, cmd.CommandText);
                      cmd.ExecuteNonQuery();
                    }
                  }
                }
                else if ((string)row["RowState"] == "Deleted" && sr_recno > 0)
                {
                  cmd.CommandType = CommandType.Text;
                  cmd.CommandText = string.Format(
                    "UPDATE \"company\".\"{0}\" SET sr_deleted = 'T' WHERE sr_recno = {1:F0}", masterTableName, sr_recno);
                  cmd.ExecuteNonQuery();
                }
              }
            }

            trans.Commit();
          }
          catch (Exception err)
          {
            errorMessage = err.Message;
            trans.Rollback();
            log.Error(err, methodName);
            return false;
          }
        }
        #endregion

        #region Log changes for branch outward replication
        using (var unitOfWork = new UnitOfWork())
        {
          var liveTableNames = CacheUtils.GetServerTableNames(cache);

          if (liveTableNames.Contains(masterTableName))
          {
            for (var i = 0; i < data.Rows.Count; i++)
            {
              var row = data.Rows[i];
              var sr_recno = Convert.ToString(row["sr_recno"]);
              new ASS_MasterTableChangeTracking(unitOfWork)
              {
                ChangedTS = DateTime.Now,
                KeyFieldName = "sr_recno",
                KeyFieldValue = sr_recno,
                TableName = masterTableName
              };
            }

            unitOfWork.CommitChanges();
          }
        }
        #endregion

        return true;
      }

      catch (Exception err)
      {
        log.Error(err, methodName);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);
        return false;
      }
    }

  }
}
