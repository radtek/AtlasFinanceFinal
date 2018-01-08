using System.IO;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Text;

using Serilog;


namespace WinServices.BLL
{
  public class SharedMembers
  {
    #region Accpac General Operations
    
    public static DateTime SetDateFromDB(string DBDate)
    {
      try
      {
        DateTime val;
        if (DBDate.Length == 8)
        {
          val = new DateTime(Convert.ToInt32(DBDate.Substring(0, 4)), Convert.ToInt32(DBDate.Substring(4, 2)), Convert.ToInt32(DBDate.Substring(6, 2)));
        }
        else
        {
          var ex1 = new Exception("Invalid date format.");
          throw ex1;
        }
        return val;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }
    
    #endregion


    #region Properties

    public static string ApplID
    {
      get
      {
        return ConfigurationManager.AppSettings.Get("applID"); ;
      }
    }

    public static string ProgramName
    {
      get
      {
        return ConfigurationManager.AppSettings.Get("programName"); ;
      }
    }

    public static string AccpacVersion
    {
      get
      {
        return ConfigurationManager.AppSettings.Get("accpacVersion"); ;
      }
    }

    public static string UserName
    {
      get
      {
        return ConfigurationManager.AppSettings.Get("userName");
      }
    }

    public static string Psw
    {
      get
      {
        return ConfigurationManager.AppSettings.Get("psw");
      }
    }

    public static string CompanyDatabase
    {
      get
      {
        return ConfigurationManager.AppSettings.Get("companyDatabase");
      }
    }

    public static string MainDirectory
    {
      get
      {
        return ConfigurationManager.AppSettings.Get("MainDirectory"); ;
      }
    }


    public static string DownloadDirectory
    {
      get
      {
        return MainDirectory + @"Downloaded\";
      }
    }

    public static string ProcessedDirectory
    {
      get
      {
        return MainDirectory + @"Processed\";
      }
    }

    public static string FailedDirectory
    {
      get
      {
        return MainDirectory + @"Failed\";
      }
    }

    public static string GetDateStr()
    {
      return string.Format("{0:yyyy-MM-dd HH.mm.ss}", DateTime.Now);
    }

    public static string cnIntegration
    {
      get
      {
        if (ConfigurationManager.ConnectionStrings["cnIntegration"] == null)
          throw (new NullReferenceException("ConnectionString configuration for cnIntegration is missing from you web.config."));

        return ConfigurationManager.ConnectionStrings["cnIntegration"].ConnectionString;
      }
    }

    #endregion


    #region File Directory Operation

    public static void CreateDirectories()
    {
      Directory.CreateDirectory(SharedMembers.DownloadDirectory);
      Directory.CreateDirectory(SharedMembers.ProcessedDirectory);
      Directory.CreateDirectory(SharedMembers.FailedDirectory);
    }

    #endregion


    #region ASS File Processing

    // Imports and moves the file to the resultant folder ('Processed' folder if imported OKAY, 'Failed' folder if not)
    public static void ProcessAssFiles()
    {
      try
      {
        #region Import DBF and copy DBF to resultant directory
        var directoryInfo = new DirectoryInfo(SharedMembers.DownloadDirectory);
        var fileInfoList = directoryInfo.GetFiles("*.*");
        foreach (var file in fileInfoList)
        {
          var dest = String.Format("{0}{1}-{2}{3}.dbf", SharedMembers.ProcessedDirectory, SharedMembers.GetDateStr(), Path.GetFileNameWithoutExtension(file.Name), Path.GetExtension(file.Name));
          if (!SharedMembers.ProcessAssDayEndFile(file.FullName))
          {
            dest = String.Format("{0}{1}-{2}{3}.dbf", SharedMembers.FailedDirectory, SharedMembers.GetDateStr(), Path.GetFileNameWithoutExtension(file.Name), Path.GetExtension(file.Name));
          }

          File.Move(file.FullName, dest);
        }
        #endregion
      }
      catch (Exception ex)
      {
        _log.Error(ex, "SharedMembers.ProcessAssFiles");
      }
    }


    // Imports DBF file into 'ASS_DailyUpload_D' and creates single 'ASS_DailyUpload_Header' entry        
    public static bool ProcessAssDayEndFile(string fullFileName)
    {
      var currPos = "Initializing";
      SqlTransaction MSSqlTran = null;
      try
      {
        var dt = BusinessLogicLayer.DAL.ParseDBF.ReadDBF(fullFileName);

        if (dt.Rows != null && dt.Rows.Count > 0)
        {
          // These are the only sort keys we action...
          var filteredInfo = dt.Select("SORTKEY IN ('01','02','03','04','05','06','07','08','10','11'," +
            "'12','13','14','19','20','21','23','24','25','26','28','30','31','32','33','34','35','36'," +
            "'37','38','41','42','43','44','45','46','47','48','49','50','51','52','53')");

          if (filteredInfo.Length > 0)
          {
            currPos = "Connecting to SQL";
            var myConnection = new SqlConnection(SharedMembers.cnIntegration.Trim());
            myConnection.Open();
            MSSqlTran = myConnection.BeginTransaction();

            using (var cmd = myConnection.CreateCommand())
            {
              cmd.CommandText = "INSERT INTO ASS_DailyUpload_D ([branchCode], [tranDate], [transType], [transDetailInd], [description], [additionalInfo], [transAmt], [tranVatAmt], [ProcessedInd]) " +
                  "VALUES (@branchCode, @tranDate, @transType, @transDetailInd, @description, @additionalInfo, @transAmt, @tranVatAmt, 0) ";
              cmd.Transaction = MSSqlTran;

              var branchCode = cmd.Parameters.Add("branchCode", SqlDbType.VarChar, 50);
              var tranDate = cmd.Parameters.Add("tranDate", SqlDbType.Decimal, 18);
              var transType = cmd.Parameters.Add("transType", SqlDbType.VarChar, 50);
              var transDetailInd = cmd.Parameters.Add("transDetailInd", SqlDbType.VarChar, 9);
              var description = cmd.Parameters.Add("description", SqlDbType.VarChar, 100);
              var additionalInfo = cmd.Parameters.Add("additionalInfo", SqlDbType.VarChar, 100);

              var transAmt = cmd.Parameters.Add("transAmt", SqlDbType.Float);
              var tranVatAmt = cmd.Parameters.Add("tranVatAmt", SqlDbType.Float);

              for (int i = 0; i < filteredInfo.Length; i++)
              {
                branchCode.Value = ((string)filteredInfo[i]["CLASS_BR"]);
                tranDate.Value = decimal.Parse(((DateTime)filteredInfo[i]["TRDATE"]).ToString("yyyyMMdd"));
                transType.Value = ((string)filteredInfo[i]["SORTKEY"]);
                transDetailInd.Value = ((string)filteredInfo[i]["JNLCODE"]);
                description.Value = ((string)filteredInfo[i]["DESC"]);
                additionalInfo.Value = ((string)filteredInfo[i]["BANKACC"]);
                transAmt.Value = filteredInfo[i]["TRAMOUNT"];
                tranVatAmt.Value = filteredInfo[i]["TRVAT"];

                cmd.ExecuteNonQuery();
              }
            }

            #region Header
            currPos = "Creating header";
            using (var MSSqlcmd = new SqlCommand() { Connection = myConnection, CommandType = CommandType.Text, Transaction = MSSqlTran })
            {
              var sb = new StringBuilder("Insert into ASS_DailyUpload_Header ");
              string cmdString = "";
              sb.Append("(CLASS_BR, TRDATE, AddedDate, ProcessedDate, ProcessedInd, Error) ");
              sb.Append("values ");
              sb.Append(String.Format("('{0}', {1:yyyyMMdd}, getdate(), null, 0, '') ", filteredInfo[0]["CLASS_BR"], (DateTime)filteredInfo[0]["TRDATE"]));
              cmdString = sb.ToString();
              MSSqlcmd.CommandText = cmdString;
              MSSqlcmd.ExecuteNonQuery();
            }
            MSSqlTran.Commit();
            myConnection.Close();
            #endregion
          }
          else
          {
            _log.Warning("SharedMembers.ProcessAssDayEndFile: No matching sortkeys found in file '{0}'", fullFileName);
          }
        }
        else
        {
          _log.Error("SharedMembers.ProcessAssDayEndFile: No records found in file '{0}'", fullFileName);
        }
      }
      catch (Exception ex)
      {
        _log.Error(ex, "SharedMembers.ProcessAssDayEndFile: {CurrPos}", currPos);

        if (MSSqlTran != null)
          MSSqlTran.Rollback();

        return false;
      }
      return true;
    }

    #endregion


    #region General

    public static string StripValues(string baseString, string preFixToRemove, string postFixToRemove)
    {
      try
      {
        var cleanString = baseString.Trim();

        if (!string.IsNullOrEmpty(preFixToRemove) && baseString.StartsWith(cleanString))
        {
          baseString = baseString.Replace(preFixToRemove, string.Empty);
        }

        if (!string.IsNullOrEmpty(postFixToRemove) && baseString.EndsWith(postFixToRemove))
        {
          baseString = baseString.Substring(0, baseString.LastIndexOf(postFixToRemove));
        }

        return baseString.Trim();
      }
      catch
      {
        return "";
      }
    }

    #endregion


    #region Private vars

    // Log4net
    private static readonly ILogger _log = Log.Logger.ForContext<SharedMembers>();

    #endregion

  }
}
