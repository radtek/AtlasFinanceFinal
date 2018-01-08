/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Ensures that every DBF row in a specific dBASE file can be read
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
using System.IO;
using System.Collections.Generic;

using SocialExplorer.IO.FastDBF;


namespace ASSServer.Utils.DBF
{
  public static class CheckCanReadData
  {
    /// <summary>
    /// Checks validity of DBF file by then trying to read every single field and checking dates 
    /// & numbers are within reasonable bounds
    /// </summary>
    /// <param name="dbfFileName">The full path to the DBF to be checked</param>
    /// <returns>Any error, else null</returns>
    public static bool Execute(string dbfFileName, ConcurrentQueue<string> copyProgressMessages)
    {
      var fileNameOnly = Path.GetFileNameWithoutExtension(dbfFileName);
      try
      {
        copyProgressMessages.Enqueue(string.Format("[{0}] READ- Opening", fileNameOnly));

        var odbf = new DbfFile(System.Text.ASCIIEncoding.ASCII);
        odbf.Open(dbfFileName, FileMode.Open);
        try
        {
          var recordCount = odbf.Header.RecordCount;
          copyProgressMessages.Enqueue(string.Format("[{0}] READ- Counted {1} records", fileNameOnly, recordCount));

          if (recordCount == 0)
          {
            copyProgressMessages.Enqueue(string.Format("[{0}] READ- Complete- no records", fileNameOnly));
            return true;
          }

          var dateCols = new List<int>();
          var numberCols = new List<int>();

          for (int i = 0; i < odbf.Header.ColumnCount; i++)
          {
            if (odbf.Header[i].ColumnType == DbfColumn.DbfColumnType.Date)
            {
              dateCols.Add(i);
            }
            if (odbf.Header[i].ColumnType == DbfColumn.DbfColumnType.Number)
            {
              numberCols.Add(i);
            }
          }

          var currRec = 0;
          copyProgressMessages.Enqueue(string.Format("[{0}] READ- Reading data", fileNameOnly));
          try
          {
            var orec = new DbfRecord(odbf.Header);
            for (int i = 0; i < odbf.Header.RecordCount; i++)
            {
              if (!odbf.Read(i, orec))
                throw new Exception(string.Format("Can't read data row {0}", i + 1));

              #region Check dates
              DateTime colDate;
              foreach (var col in dateCols)
              {
                if (!string.IsNullOrWhiteSpace(orec[col]))
                {
                  if (!DateTime.TryParseExact(orec[col], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out colDate)
                    || (colDate > new DateTime(2050, 01, 01) || colDate < new DateTime(1990, 1, 1)))
                  {
                    copyProgressMessages.Enqueue(string.Format("[{0}] READ- >> FATAL << Invalid DBF date: '{1}'- Data row {2}, Col: {3}", fileNameOnly, orec[col], i + 1, odbf.Header[col].Name));
                    return false;
                  }                  
                }
              }
              #endregion
                            
              #region Check all numbers
              foreach (var col in numberCols)
              {
                if (!string.IsNullOrWhiteSpace(orec[col]))
                {
                  Decimal val;
                  if (!Decimal.TryParse(orec[col].Trim(), System.Globalization.NumberStyles.AllowDecimalPoint | System.Globalization.NumberStyles.AllowLeadingSign,
                    System.Globalization.CultureInfo.InvariantCulture, out val))
                  {
                    throw new Exception(string.Format("[{0}] >> FATAL << Invalid DBF number: '{1}'- Data row {2}, Col: {3}", fileNameOnly, orec[col], i + 1, odbf.Header[col].Name));
                  }
                }
              }
              #endregion

            }
          }
          catch (Exception err)
          {
            copyProgressMessages.Enqueue(string.Format("[{0}] READ- >> FATAL << Error reading DBF field: '{1}'- Data row {2}", fileNameOnly, err.Message, currRec));
            return false;
          }
        }
        finally
        {
          odbf.Close();
          copyProgressMessages.Enqueue(string.Format("[{0}] READ- Complete", fileNameOnly));
        }

        return true;
      }
      catch (Exception err)
      {
        copyProgressMessages.Enqueue(string.Format("[{0}] READ- >> FATAL << Error: '{1}'", fileNameOnly, err.Message));
        return false;
      }
    }

  }
}