using System;
using System.IO;

using Serilog;
using Npgsql;

using Atlas.ThirdParty.CS.WCF.Interface;
using Atlas.Server.Training.QuartzTasks;


namespace Atlas.Server.Training
{
  internal class ScorecardServer : IScorecardServer
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public ScorecardV2Result GetScorecardV2(string legacyBranchNum, string firstName,
      string surname, string idNumber, string gender, DateTime dateOfBirth, string addressLine1, string addressLine2,
      string addressLine3, string addressLine4, string postalCode, string homeTelCode, string homeTelNo, string workTelCode,
      string workTelNo, string cellNo, bool isIdPassportNo)
    {
      var methodName = "GetScorecardV2";
      var tempZip = Path.Combine(ConfigHelper.GetTempPath(), string.Format("{0}.zip", Guid.NewGuid().ToString("N")));
      var tempPath = Path.Combine(ConfigHelper.GetTempPath(), Guid.NewGuid().ToString("N"));
      var result = new ScorecardV2Result();

      _log.Information("{MethodName}- {legacyBranchNum}, {firstName}, {surname}, {idNumber}, {gender}, {dateOfBirth}, " +
        "{addressLine1}, {addressLine2}, {addressLine3}, {addressLine4}, {postalCode}, {homeTelCode}, {homeTelNo}, " +
        "{workTelCode}, {workTelNo}, {cellNo}, {isIdPassportNo}",
        methodName, legacyBranchNum, firstName, surname, idNumber, gender, dateOfBirth,
        addressLine1, addressLine2, addressLine3, addressLine4, postalCode, homeTelCode, homeTelNo,
        workTelCode, workTelNo, cellNo, isIdPassportNo);
      try
      {
        try
        {
          var destSystemConn = new NpgsqlConnectionStringBuilder(ConfigHelper.PSQLDestConnectionString()) { Database = "atlas_core" };
          using (var conn = new NpgsqlConnection(destSystemConn.ConnectionString))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              // Select a random scorecard
              cmd.CommandText = string.Format("SELECT \"OriginalResponse\" FROM \"BUR_Storage\" LIMIT 1 OFFSET {0}", new Random().Next(20));
              using (var rdr = cmd.ExecuteReader())
              {
                if (rdr.Read())
                {
                  var buffer = new byte[81892];
                  using (var fs = new FileStream(tempZip, FileMode.CreateNew, FileAccess.Write))
                  {
                    long bytesRead = 0;
                    long offset = 0;
                    while ((bytesRead = rdr.GetBytes(0, offset, buffer, 0, buffer.Length)) > 0)
                    {
                      fs.Write(buffer, 0, (int)bytesRead);
                      offset += bytesRead;
                    }
                  }
                }
              }
            }
          }

          if (File.Exists(tempZip))
          {
            byte[] userFile;
            byte[] score;
            Atlas.ThirdParty.CS.Enquiry.ScoreXmlUtils.UnzipScorecard(_log, tempZip, out userFile, out score);
            result.UserDisplayFileBase64 = Convert.ToBase64String(userFile);
            result.ScorecardXmlFileBase64 = Convert.ToBase64String(score);
            result.Successful = true;
            result.ErrorMessage = "";
          }
          else
          {
            result.ErrorMessage = "Unexpected server error- no file";
            result.Successful = false;
          }
        }
        catch (Exception err)
        {
          _log.Error(err, "{MethodName}", methodName);
          result.ErrorMessage = "Unexpected server error";
        }
      }
      finally
      {
        if (File.Exists(tempZip))
        {
          File.Delete(tempZip);
        }
        if (Directory.Exists(tempPath))
        {
          Directory.Delete(tempPath, true);
        }
      }

      _log.Information("{MethodName} Result: {Success} {ErrorMessage}", methodName, result.Successful, result.ErrorMessage);
      return result;
    }

    public ScorecardSimpleResult GetSimpleScorecard(string legacyBranchNum, string firstName, string surname, string idNumber, bool isIdPassportNo)
    {
      throw new NotImplementedException();
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<ScorecardServer>();

    #endregion

  }
}
