/* -----------------------------------------------------------------------------------------------------------------
* 
*  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*     Useful PostgreSQL routines to get column/index/primary key SQL definitions into POCO's.
*     
*     
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*  
*     2013-11-29 Created
*     
*     2013-12-06 Small updates
*     
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;


namespace Atlas.Data
{
  /// <summary>
  /// Useful PostgreSQL utilities for working with PostgreSQL tables and columns
  /// </summary>
  public static class PSQLUtils
  {
    /// <summary>
    /// Get table names of non-system, PostgreSQL tables in the schema named 'schemaName'
    /// </summary>
    /// <param name="conn">The open connection to use</param>
    /// <param name="schemaName"></param>
    /// <returns>List of table names</returns>
    public static List<string> GetAllTables(Npgsql.NpgsqlConnection conn, string schemaName)
    {
      var result = new List<string>();
      using (var cmd = conn.CreateCommand())
      {
        cmd.CommandText = string.Format(
          "SELECT table_name " +
          "FROM information_schema.tables " +
          "WHERE table_schema = '{0}' AND table_type = 'BASE TABLE'", schemaName);
        cmd.CommandType = System.Data.CommandType.Text;
        using (var rdr = cmd.ExecuteReader())
        {
          while (rdr.Read())
          {
            var fieldName = rdr.GetString(0).ToLower();
            result.Add(fieldName);
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Gets table names of all ASS SQLRDD data tables in schema named 'schemaName'- excludes 'sr_*' and 'lrep_*' tables
    /// </summary>
    /// <param name="conn">The open connection to use</param>
    /// <param name="schemaName">The schema to check</param>
    /// <returns>List of table names</returns>
    public static List<string> GetAllNonSQLRDDTables(Npgsql.NpgsqlConnection conn, string schemaName)
    {
      var result = new List<string>();
      using (var cmd = conn.CreateCommand())
      {
        // Use column 'sr_recno' to identify SQLRDD ASS
        cmd.CommandText = string.Format(
          "SELECT table_name " +
          "FROM information_schema.columns " +
          "WHERE column_name = 'sr_recno' and table_schema = '{0}' and table_name !~ '^(sr_|lrep_)'" +
          "GROUP BY table_name " +
          "ORDER BY table_name", schemaName);
        cmd.CommandType = System.Data.CommandType.Text;
        using (var rdr = cmd.ExecuteReader())
        {
          while (rdr.Read())
          {
            var fieldName = rdr.GetString(0).ToLower();
            result.Add(fieldName);
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Returns all the column definitions for table 'tableName', in schema 'schemaName'
    /// </summary>
    /// <param name="conn">The open connection to use</param>
    /// <param name="schemaName">The PostgresSQL schema to use</param>
    /// <param name="tableName">The PostgresSQL table name</param>
    /// <param name="schemaless">Make the definition schema-less</param>
    /// <returns>List of column definitions for this table</returns>
    public static List<SQLColumnDefinition> GetColumnDefitionsForTable(Npgsql.NpgsqlConnection conn, string schemaName, string tableName, bool schemaless = true)
    {
      var result = new List<SQLColumnDefinition>();
      using (var cmd = conn.CreateCommand())
      {
        cmd.CommandText = string.Format(
          "SELECT column_name, data_type, COALESCE(character_maximum_length, 0), COALESCE(numeric_precision, 0), " +
          "COALESCE(numeric_scale, 0), is_nullable, COALESCE(column_default, '') " +
          "FROM information_schema.columns " +
          "WHERE table_schema = '{0}' AND table_name = '{1}' " +
          "ORDER BY ordinal_position", schemaName, tableName);
        cmd.CommandType = System.Data.CommandType.Text;

        using (var rdr = cmd.ExecuteReader())
        {
          while (rdr.Read())
          {

            #region Get column type
            SQLColumnDataType columnType;
            var colTypeText = rdr.GetString(1).ToLower();
            switch (colTypeText)
            {
              case "character":
                columnType = SQLColumnDataType.SQLCharacter;
                break;

              case "character varying":
                columnType = SQLColumnDataType.SQLCharacterVarying;
                break;

              case "numeric":
                columnType = SQLColumnDataType.SQLNumeric;
                break;

              case "date":
                columnType = SQLColumnDataType.SQLDate;
                break;

              case "datetime":
                columnType = SQLColumnDataType.SQLDateTime;
                break;

              case "boolean":
                columnType = SQLColumnDataType.SQLBoolean;
                break;

              case "text":
                columnType = SQLColumnDataType.SQLText;
                break;

              case "bytea":
                columnType = SQLColumnDataType.SQLByteA;
                break;

              case "money":
                columnType = SQLColumnDataType.SQLMoney;
                break;

              case "integer":
                columnType = SQLColumnDataType.SQLInteger;
                break;

              case "smallint":
                columnType = SQLColumnDataType.SQLSmallInt;
                break;

              case "bigint":
                columnType = SQLColumnDataType.SQLBigInt;
                break;

              default:
                throw new Exception(string.Format("SQL data type '{0}' is unrecognised", colTypeText));
            }
            #endregion

            var defaultValExpr = (schemaless) ?
              rdr.GetString(6).Replace(string.Format("nextval({0}.", schemaName), "nextval(") : rdr.GetString(6);

            result.Add(new SQLColumnDefinition(
              columnName: rdr.GetString(0), 
              columnType: columnType, 
              columnTypeString: rdr.GetString(1).ToLower(),
              characterLength: rdr.GetInt32(2),
              numericPrecision: rdr.GetInt32(3), 
              numericScale: rdr.GetInt32(4),
              isNullable: rdr.GetString(5) == "YES", 
              defaultValue: defaultValExpr));
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Gets list of field names in table 'tableName', in schema 'schemaName'
    /// </summary>
    /// <param name="conn">The connection to use</param>
    /// <param name="schemaName">The database schema name</param>
    /// <param name="tableName">The database table name</param>
    /// <returns></returns>
    public static List<string> GetColumnNamesForTable(Npgsql.NpgsqlConnection conn, string schemaName, string tableName)
    {
      var result = new List<string>();
      using (var cmd = conn.CreateCommand())
      {
        cmd.CommandText = string.Format(
          "SELECT column_name " +
          "FROM information_schema.columns " +
          "WHERE table_schema = '{0}' AND table_name = '{1}'", schemaName, tableName);
        cmd.CommandType = System.Data.CommandType.Text;
        using (var rdr = cmd.ExecuteReader())
        {
          while (rdr.Read())
          {
            var fieldName = rdr.GetString(0);
            result.Add(fieldName);
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Get the primary key constraint definition for a table
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="schemaName"></param>
    /// <param name="tableName"></param>
    /// <returns></returns>
    public static SQLConstraintDefinition GetPKConstraintForTable(Npgsql.NpgsqlConnection conn, string schemaName, string tableName)
    {
      SQLConstraintDefinition result = null;

      using (var cmd = conn.CreateCommand())
      {
        cmd.CommandText = string.Format(
          "SELECT pg_constraint.conname as constraintname, pg_get_constraintdef(pg_constraint.oid) AS constraintdef " +
          "FROM pg_constraint " +
          "INNER JOIN pg_class ON conrelid=pg_class.oid " +
          "INNER JOIN pg_namespace ON pg_namespace.oid=pg_class.relnamespace " +
          "WHERE relname = '{1}' and nspname = '{0}'", schemaName, tableName);
        using (var rdr = cmd.ExecuteReader())
        {
          if (rdr.Read())
          {
            result = new SQLConstraintDefinition(rdr.GetString(0), rdr.GetString(1));
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Gets the indexes defined for a table, in a specific schema
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="schemaName"></param>
    /// <param name="tableName"></param>
    /// <returns>List of index defintions</returns>
    public static List<SQLIndexDefinition> GetIndexesForTable(Npgsql.NpgsqlConnection conn, string schemaName, string tableName)
    {
      var result = new List<SQLIndexDefinition>();

      using (var cmd = conn.CreateCommand())
      {
        cmd.CommandText = string.Format(
          "SELECT distinct i.relname as index_name, pg_get_indexdef(i.oid) as index_def " +
          "FROM pg_class t " +
          "JOIN pg_index d ON t.oid = d.indrelid " +
          "JOIN pg_class i ON d.indexrelid = i.oid AND i.relkind='i' " +
          "JOIN pg_namespace ns ON t.relnamespace = ns.oid " +
          "WHERE d.indisprimary = 'f' " +
          "AND t.relname = '{1}' " +
          "AND ns.nspname = '{0}'", schemaName, tableName);
        cmd.CommandType = System.Data.CommandType.Text;

        using (var rdr = cmd.ExecuteReader())
        {
          while (rdr.Read())
          {
            var sql = rdr.GetString(1);
            var sqlUpper = sql.ToUpper();            
            var pos2 = sqlUpper.IndexOf(" USING ");
            result.Add(new SQLIndexDefinition(rdr.GetString(0), sql.Substring(pos2)));
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Copy a table's contents from source schema/table to destination schema/table, 
    /// optionally including indexes, constraints and default values
    /// </summary>
    /// <param name="conn">The open connection to use</param>
    /// <param name="sourceSchemaName">The source schema name</param>
    /// <param name="sourceTableName">The source table name</param>
    /// <param name="destSchemaName">The destination schema name</param>
    /// <param name="destTableName">The destination table name</param>
    /// <param name="copyAllIndexes">Copy index definitions</param>
    /// <param name="copyConstraints">Copy constraints (PK, FK)</param>
    /// <param name="copyFieldDefaults">Copy default values</param>
    /// <returns></returns>
    public static void CopyTable(Npgsql.NpgsqlConnection conn, string sourceSchemaName, string sourceTableName,
      string destSchemaName, string destTableName, bool copyIndexes, bool copyConstraints, bool copyFieldDefaults)
    {
      var include = new StringBuilder();
      include.Append(copyIndexes ? " INCLUDE INDEXES" : null);
      include.Append(copyConstraints ? " INCLUDE CONSTRAINTS" : null);
      include.Append(copyFieldDefaults ? " INCLUDE DEFAULTS" : null);

      var cmdSQL = string.Format("CREATE TABLE {0}.\"{1}\" ( LIKE {2}.\"{3}\" {4} )", sourceSchemaName, sourceTableName,
        destSchemaName, destTableName, include.ToString());
      using (var cmd = conn.CreateCommand())
      {
        cmd.CommandText = cmdSQL;
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.ExecuteNonQuery();
      }
    }


    public static string CreateTableSQL(string schemaName, string tableName, List<SQLColumnDefinition> columns,
      List<SQLIndexDefinition> indexes, SQLConstraintDefinition primaryKey, bool uniquePrimaryKeyName, bool uniqueIndexNames)
    {
      var sql = new StringBuilder();
      sql.AppendFormat("CREATE TABLE \"{0}\".\"{1}\" (\r\n  {2}{3});\r\n", schemaName, tableName,
         string.Join(",\r\n  ", columns.Select(s => s.ToSQLDefinition())),
         (primaryKey != null) ? string.Format(",\r\n  CONSTRAINT {0}", primaryKey.PKInlineConstraintToSQL(schemaName, tableName, uniquePrimaryKeyName)) : string.Empty);

      if (indexes != null && indexes.Count > 0)
      {
        sql.Append(string.Join(";\r\n\r\n", indexes
          .Select(s => s.IndexToSQL(schemaName, tableName, uniqueIndexNames))));
      }

      return sql.ToString();
    }


    /// <summary>
    /// PostgreSQL column type- not comprehensive- covers the needs of the ASS database only!!
    /// </summary>
    public enum SQLColumnDataType
    {
      SQLCharacter, SQLCharacterVarying,
      SQLNumeric, SQLMoney,
      SQLBoolean,
      SQLDateTime, SQLDate,
      SQLText, SQLByteA,
      SQLInteger, SQLSmallInt, SQLBigInt
    };
       
    /// <summary>
    /// Simple class to describe a PostgreSQL column- this is not comprehensive- it only covers the needs of the ASS schema!!
    /// </summary>
    public class SQLColumnDefinition
    {
      #region Public constructor

      public SQLColumnDefinition(string columnName, SQLColumnDataType columnType, string columnTypeString, int characterLength, int numericPrecision, int numericScale, bool isNullable, string defaultValue)
      {
        ColumnName = columnName;
        ColumnType = columnType;
        ColumnTypeString = columnTypeString;
        CharacterLength = characterLength;
        NumericPrecision = numericPrecision;
        NumericScale = numericScale;
        IsNullable = isNullable;
        DefaultValue = defaultValue;
      }

      #endregion


      #region Public properties

      public string ColumnName { get; private set; }
      public SQLColumnDataType ColumnType { get; private set; }
      public string ColumnTypeString { get; private set; }
      public int CharacterLength { get; private set; }
      public int NumericPrecision { get; private set; }
      public int NumericScale { get; private set; }
      public bool IsNullable { get; private set; }
      public string DefaultValue { get; private set; }

      #endregion

      /// <summary>
      /// Gets the column definition as SQL DDL string, i.e "numeric(15, 2) NOT NULL DEFAULT 0"
      /// </summary>
      public string ToSQLDefinition()
      {
        string sqlType;
        switch (ColumnType)
        {
          case SQLColumnDataType.SQLBoolean:
            sqlType = string.Format("boolean", ColumnName);
            break;

          case SQLColumnDataType.SQLCharacter:
            sqlType = string.Format("character({1})", ColumnName, CharacterLength);
            break;

          case SQLColumnDataType.SQLCharacterVarying:
            sqlType = string.Format("character varying({1})", ColumnName, CharacterLength);
            break;

          case SQLColumnDataType.SQLDateTime:
            sqlType = string.Format("datetime", ColumnName);
            break;

          case SQLColumnDataType.SQLDate:
            sqlType = string.Format("date", ColumnName);
            break;

          case SQLColumnDataType.SQLNumeric:
            sqlType = string.Format("numeric({1},{2})", ColumnName, NumericPrecision, NumericScale);
            break;

          case SQLColumnDataType.SQLText:
            sqlType = string.Format("text", ColumnName);
            break;

          case SQLColumnDataType.SQLByteA:
            sqlType = string.Format("bytea", ColumnName);
            break;

          case SQLColumnDataType.SQLMoney:
            sqlType = string.Format("money", ColumnName);
            break;

          case SQLColumnDataType.SQLInteger:
            sqlType = string.Format("integer", ColumnName);
            break;

          case SQLColumnDataType.SQLSmallInt:
            sqlType = string.Format("smallint", ColumnName);
            break;

          default:
            throw new NotImplementedException();
        }

        return string.Format("\"{0}\" {1} {2} {3}", ColumnName, sqlType, 
          IsNullable ? "NULL" : "NOT NULL", 
          !string.IsNullOrEmpty(DefaultValue) ? string.Format(" DEFAULT {0}", DefaultValue) : string.Empty);
      }
    }


    /// <summary>
    /// Class to hold SQL index definition
    /// </summary>
    public class SQLIndexDefinition
    {
      #region Public constructor

      public SQLIndexDefinition(string indexName, string indexSQL)
      {
        IndexName = indexName;
        IndexSQL = indexSQL;
      }
      #endregion


      #region Public properties

      public string IndexName { get; private set; }
      public string IndexSQL { get; private set; }

      #endregion

      public string IndexToSQL(string schemaName, string tableName, bool makeIndexNameUnique)
      {
        return makeIndexNameUnique ?
          string.Format("CREATE INDEX {0} ON {1}.\"{2}\" {3}", string.Format("idx_{0}_{1}_{2}", schemaName, tableName, IndexName), schemaName, tableName, IndexSQL) :
          string.Format("CREATE INDEX {0} ON {1}.\"{2}\" {3}", IndexName, schemaName, tableName, IndexSQL);
      }
    }


    /// <summary>
    /// Class to hold a SQL Constraint definition (for ASS, the primary key)
    /// </summary>
    public class SQLConstraintDefinition
    {
      public SQLConstraintDefinition(string constraintName, string constraintSQL)
      {
        ConstraintName = constraintName;
        ConstraintSQL = constraintSQL;
      }

      public string ConstraintName { get; set; }
      public string ConstraintSQL { get; set; }


      /// <summary>
      /// Create PRIMARY KEY CONSTRAINT SQL text, *as part of CREATE TABLE*
      /// </summary>
      /// <param name="schemaName">The schema</param>
      /// <param name="tableName">The table</param>
      /// <param name="makeConstraintNameUnique">Don't use the contraints name- create a unique constraint name</param>
      /// <returns>SQL text to add the constraint as part of a CREATE TABLE</returns>
      public string PKInlineConstraintToSQL(string schemaName, string tableName, bool makeConstraintNameUnique)
      {
        string constraintName = (makeConstraintNameUnique) ?
          string.Format("pk_{0}_{1}_{2}", schemaName, tableName, ConstraintName) : ConstraintName;

        return string.Format("{0} {1}", constraintName, ConstraintSQL);
      }

      /// <summary>
      /// Create PRIMARY KEY CONSTRAINT SQL, to add to an existing table
      /// </summary>
      /// <param name="schemaName">The schema</param>
      /// <param name="tableName">The table</param>
      /// <param name="makeConstraintNameUnique">Don't use the contraints name- create a unique constraint name</param>
      /// <returns>SQL text to add the constraint to an existing table</returns>
      public string PKAddConstraintSQL(string schemaName, string tableName, bool makeConstraintNameUnique)
      {
        string constraintName = (makeConstraintNameUnique) ?
          string.Format("pk_{0}_{1}_{2}", schemaName, tableName, ConstraintName) : ConstraintName;

        return string.Format("ALTER TABLE \"{0}\".\"{1}\" ADD CONSTRAINT {2} PRIMARY KEY {3}", schemaName, tableName, constraintName, ConstraintSQL);
      }
    }

  }
}
