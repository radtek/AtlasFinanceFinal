using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Reflection;
using System.IO.Compression;
using System.Configuration;

using FileHelpers;
using Npgsql;
using MongoDB.Driver;

using Atlas.Evolution.Server.Code.Layout;
using Evolution.Mongo.Entity;


namespace LoadFixed
{
  class Program
  {
    static void Main()
    {
      var filename = "AT0001_ALL_T702_M_20161031_1_1.txt";
      var path = @"D:\Temp";
      GetFile(filename, path);

      var lookup = LoadNuPayBranchCodes();

      var engine = new FileHelperEngine<Monthly_Data>();
      Log("loading...");
      var records = engine.ReadFileAsList(Path.Combine(path, filename));
      Log($"Loaded: {records.Count}");

      foreach (var rec in records)
      {
        rec.LegacyBranchCode = lookup[rec.BRANCH_CODE];
      }

      #region First name < 3 chars
      var badFirstName = records.Where(s => s.FORENAME_OR_INITIAL_1.Trim().Length < 3).ToList();
      if (badFirstName.Any())
      {
        Log("Bad first name:");
        foreach (var name in badFirstName)
        {
          Log($"{name.LegacyBranchCode}x{name.ACCOUNT_NO}x{name.SUB_ACCOUNT_NO} | {name.FORENAME_OR_INITIAL_1} | {name.FORENAME_OR_INITIAL_2} | {name.FORENAME_OR_INITIAL_3}");
        }
        Log("----------------------------------------------");
      }
      #endregion

      #region Where passport, ID must be zero padded
      var errs = 0;
      var recs = records.Where(s => !string.IsNullOrWhiteSpace(s.NON_SA_ID_NUMBER)).ToList();
      if (recs.Any())
      {
        Log("ID not zero padded");
        foreach (var nonSA in recs)
        {
          if (string.Compare(nonSA.SA_ID_NUMBER, "0000000000000") != 0)
          {
            errs++;
            Log($"Invalid SA: '{nonSA.SA_ID_NUMBER}'");
          }
        }
        Log($"Complete- Errs: {errs}");
        Log("----------------------------------------------");
      }
      #endregion

      #region NO vowels
      var vowels = new List<char> { 'A', 'E', 'I', 'O', 'U', 'Y', 'a', 'e', 'i', 'o', 'u', 'y' };
      var noVowels = records.Where(s => !s.FORENAME_OR_INITIAL_1.ToArray().Any(c => vowels.Contains(c)) ||
        (!string.IsNullOrWhiteSpace(s.FORENAME_OR_INITIAL_2) && !s.FORENAME_OR_INITIAL_2.ToArray().Any(c => vowels.Contains(c))) ||
        (!string.IsNullOrWhiteSpace(s.FORENAME_OR_INITIAL_3) && !s.FORENAME_OR_INITIAL_3.ToArray().Any(c => vowels.Contains(c)))).ToList();
      if (vowels.Any())
      {
        Log("No vowels:");
        foreach (var name in noVowels)
        {
          Log($"{name.LegacyBranchCode}x{name.ACCOUNT_NO}x{name.SUB_ACCOUNT_NO} | {name.FORENAME_OR_INITIAL_1} | {name.FORENAME_OR_INITIAL_2} | {name.FORENAME_OR_INITIAL_3}");
        }
        Log("----------------------------------------------");
      }
      #endregion

      #region Not A-Z      
      var regex = new System.Text.RegularExpressions.Regex(@"^[A-Za-z \-']*$");
      var invalidChars = records.Where(s => !regex.IsMatch(s.FORENAME_OR_INITIAL_1.Trim()) ||
        !regex.IsMatch(s.FORENAME_OR_INITIAL_2) ||
        !regex.IsMatch(s.FORENAME_OR_INITIAL_3)).ToList();
      if (invalidChars.Any())
      {
        Log("Name regex fail");
        foreach (var name in invalidChars)
        {
          Log($"{name.LegacyBranchCode}x{name.ACCOUNT_NO}x{name.SUB_ACCOUNT_NO} | {name.FORENAME_OR_INITIAL_1} | {name.FORENAME_OR_INITIAL_2} | {name.FORENAME_OR_INITIAL_3}");
        }
        Log("----------------------------------------------");
      }
      #endregion

      #region Date account opened in future      
      var badOpen = records.Where(s => s.DATE_ACCOUNT_OPENED > 20161031);
      if (badOpen.Any())
      {
        Log("Bad date account opened:");
        foreach (var date in badOpen)
        {
          Log($"{date.DATE_ACCOUNT_OPENED}");
        }
        Log("----------------------------------------------");
      }
      #endregion

      //#region Handed over < 91.31 days since last payment 
      //var ninetyDays = int.Parse(new DateTime(2016, 10, 31).Subtract(TimeSpan.FromDays(92)).ToString("yyyyMMdd"));
      //var badState = new List<string> { "W", "I", "J", "L" };
      //var badHOvrs = records.Where(s => badState.Contains(s.STATUS_CODE) && s.DATE_OF_LAST_PAYMENT > ninetyDays && s.REPAYMENT_FREQUENCY >= 3).ToList();
      //if (badHOvrs.Any())
      //{
      //  Log($"Handed over <91 days ({ninetyDays})");
      //  foreach (var badHOvr in badHOvrs)
      //  {
      //    Log($"{badHOvr.BRANCH_CODE} ({badHOvr.LegacyBranchCode})|{badHOvr.ACCOUNT_NO}|{badHOvr.SUB_ACCOUNT_NO}|{badHOvr.DATE_OF_LAST_PAYMENT}|{badHOvr.STATUS_CODE}");
      //  }
      //}
      //#endregion

      #region Instalment amount > Current balance
      var instalErr = records.Where(s => s.INSTALMENT_AMOUNT > s.CURRENT_BALANCE).ToList();
      if (instalErr.Any())
      {
        Log("Instalment > current balance");
        foreach (var instal in instalErr)
        {
          Log($"{instal.BRANCH_CODE} ({instal.LegacyBranchCode})|{instal.ACCOUNT_NO}|{instal.SUB_ACCOUNT_NO}|{instal.INSTALMENT_AMOUNT}|{instal.CURRENT_BALANCE}|{instal.DATE_OF_LAST_PAYMENT}");
        }
      }
      #endregion

      #region Last paid
      errs = 0;
      var badLastPaid = records.Where(s =>                                                                        // ---- Early settled
        (s.DATE_OF_LAST_PAYMENT < 20160101 || s.STATUS_DATE < 20160101) && !string.IsNullOrEmpty(s.STATUS_CODE) && s.STATUS_CODE != "T" && s.STATUS_CODE != "V").ToList();
      if (badLastPaid.Any())
      {
        Log("Bad last paid/status date");
        foreach (var date in badLastPaid)
        {
          errs++;
          Log($"{date.BRANCH_CODE} ({date.LegacyBranchCode})|{date.ACCOUNT_NO}|{date.SUB_ACCOUNT_NO}|{date.DATE_OF_LAST_PAYMENT}|{date.STATUS_CODE}");
        }
        Log($"Complete- Errs: {errs}");
        Log("----------------------------------------------");
      }
      #endregion

      #region Account dupes
      errs = 0;
      var dupes = records
        .GroupBy(s => $"{s.BRANCH_CODE} ({s.LegacyBranchCode})|{s.ACCOUNT_NO}|{s.SUB_ACCOUNT_NO}").ToList()
        .Where(s => s.Count() > 1).ToList();
      if (dupes.Any())
      {
        Log("Account duplicates");
        foreach (var date in dupes)
        {
          errs++;
          var codes = string.Join(", ", date.Select(s => s.STATUS_CODE));
          Log($"{date.Key}- {date.Count()}|{codes}");
        }
        Log($"Complete- Errs: {errs}");
        Log("----------------------------------------------");
      }
      #endregion

      #region Current balance zero
      errs = 0;
      var closed = new string[] { "C", "D", "G", "H", "K", "L", "P", "S", "T", "W", "Z" };
      Log("Invalid balance zero");
      var balErr = records.Where(s => s.CURRENT_BALANCE == 0 && string.IsNullOrEmpty(s.STATUS_CODE))
        .OrderBy(s => $"{s.BRANCH_CODE} ({s.LegacyBranchCode}){s.ACCOUNT_NO}{s.SUB_ACCOUNT_NO}").ToList();
      if (balErr.Any())
      {
        foreach (var balance in balErr)
        {
          errs++;
          Log($"{balance.STATUS_CODE}|{balance.BRANCH_CODE}({balance.LegacyBranchCode})|{balance.ACCOUNT_NO}|{balance.SUB_ACCOUNT_NO}|{balance.CURRENT_BALANCE}");
        }
        Log($"Complete- Errs: {errs}");
        Log("----------------------------------------------");
      }
      #endregion

      #region Closed and no date      
      var closedNoDate = records.Where(s => !string.IsNullOrWhiteSpace(s.STATUS_CODE) && s.STATUS_DATE < 20160801).ToList();
      if (closedNoDate.Any())
      {
        Log("Closed with no date");
        foreach (var notClosed in closedNoDate)
        {
          errs++;
          Log($"{notClosed.STATUS_CODE}|{notClosed.BRANCH_CODE}({notClosed.LegacyBranchCode})|{notClosed.ACCOUNT_NO}|{notClosed.SUB_ACCOUNT_NO}");
        }
      }
      #endregion

      #region Status date == 0
      var status_date = records.Where(s => s.STATUS_DATE == 0 && !string.IsNullOrWhiteSpace(s.STATUS_CODE)).ToList();
      var has_status = records.Where(s => !string.IsNullOrWhiteSpace(s.STATUS_CODE))
        .GroupBy(s => s.STATUS_CODE).Select(s => new { s.Key, Count = s.Count() }).ToList();

      #endregion

      var noSubAccount = records.Where(s => string.IsNullOrWhiteSpace(s.SUB_ACCOUNT_NO)).ToList();
            
      errs = 0;
      var noPayment = records.Where(s => s.DATE_OF_LAST_PAYMENT == 0 && s.DATE_ACCOUNT_OPENED < 20160630).ToList();
      if (noPayment.Any())
      {
        Log("No payment and not handed over");
        foreach (var noPay in noPayment)
        {
          errs++;
          Log($"Acc open:{noPay.DATE_ACCOUNT_OPENED}, BR:{noPay.LegacyBranchCode}, Client: {noPay.ACCOUNT_NO} Loan: {noPay.SUB_ACCOUNT_NO}");
        }
        Log($"Complete- Errs: {errs}");
        Log("----------------------------------------------");
      }
      var bad_date = records.Where(s => (s.STATUS_CODE == "W" || s.STATUS_CODE == "I" || s.STATUS_CODE == "J" || s.STATUS_CODE == "L") && (s.DATE_OF_LAST_PAYMENT < 20160601)).ToList();

      Log("Press a key...");
      Console.ReadKey();
    }

    
    private static void GetFile(string fileName, string path)
    {
      var client = GetMongoClient();
      var db = client.GetDatabase("evolution");
      var collection = db.GetCollection<Evolution_Batch_File>("batch_file");
      var xx = collection.AsQueryable<Evolution_Batch_File>();
      var thisDoc = xx.Where(s => s.Filename == fileName).OrderByDescending(s => s.CreatedDT).FirstOrDefault(); // works now?!?
      if (thisDoc != null)
      {
        var filename = $"{path}\\{thisDoc.Filename}";
        using (var fd = File.Create(filename))
        using (var fs = new MemoryStream(thisDoc.File))
        using (var csStream = new GZipStream(fs, CompressionMode.Decompress))
        {
          byte[] buffer = new byte[1024];
          int nRead;
          while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
          {
            fd.Write(buffer, 0, nRead);
          }
        }
      }
      return;

      // Get users
      //var command = new BsonDocument("usersInfo", 1);
      //var result = db.RunCommand<BsonDocument>(command);

      var filter = Builders<Evolution_Batch_File>.Filter.Eq(s => s.Filename, fileName);
      //var filter2 = Builders<Evolution_Batch_File>.Filter.Eq("Id", new ObjectId("577962f567b0000a184a840d"));

      //ObjectId("57a2421867b0002b3c4a91a0")
      var cursor = collection.Find(filter).ToCursor();
      foreach (var doc in cursor.ToEnumerable())
      {
        Console.WriteLine($"dc.Id- {doc.Filename}");

        var filename = $"{path}\\{doc.Filename}";
        using (var fd = File.Create(filename))
        using (var fs = new MemoryStream(doc.File))
        using (var csStream = new GZipStream(fs, CompressionMode.Decompress))
        {
          byte[] buffer = new byte[1024];
          int nRead;
          while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
          {
            fd.Write(buffer, 0, nRead);
          }
        }
      }
    }

    public static Dictionary<string, string> LoadNuPayBranchCodes()
    {
      var result = new Dictionary<string, string>();
      using (var conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["atlas_core"].ConnectionString))
      {
        conn.Open();
        using (var cmd = conn.CreateCommand())
        {
          cmd.CommandText = "SELECT s.\"BranchCode\", b.\"LegacyBranchNum\" " +
                            "FROM \"BUR_Service\" s " +
                            "JOIN \"BRN_Branch\" b ON b.\"BranchId\" = s.\"BranchId\" " +
                            "WHERE LENGTH(TRIM(s.\"BranchCode\")) > 0 AND s.\"ServiceTypeId\" = 0 " +
                            "ORDER BY b.\"LegacyBranchNum\"";

          using (var rdr = cmd.ExecuteReader())
          {
            while (rdr.Read())
            {
              result.Add(rdr.GetString(0), rdr.GetString(1));
            }
          }
        }
      }

      return result;
    }



    private static MongoClient GetMongoClient()
    {
      var host = ConfigurationManager.AppSettings["mongodb-host"] ?? "10.0.0.249";
      var port = ConfigurationManager.AppSettings["mongodb-port"] ?? "27017";
      var user = ConfigurationManager.AppSettings["mongodb-user"] ?? "evolution-server";
      var pass = ConfigurationManager.AppSettings["mongodb-pass"] ?? "grt6y65hiuoodfsetbv";
      var dbName = ConfigurationManager.AppSettings["mongodb-db"] ?? "evolution";

      return new MongoClient(new MongoClientSettings
      {
        Credentials = new[] { MongoCredential.CreateCredential(dbName, user, pass) },
        ConnectionMode = ConnectionMode.Direct,
        //ConnectTimeout = TimeSpan.FromSeconds(5),
        //ReadPreference = ReadPreference.Nearest,
        Server = new MongoServerAddress(host, int.Parse(port)),
        UseSsl = false,
        ConnectTimeout = TimeSpan.FromSeconds(10),
        ReplicaSetName = "rs1",
        //Servers = new[] { new MongoServerAddress(host) },
        ServerSelectionTimeout = TimeSpan.FromSeconds(5)
      });
    }

    private static void Log(string log)
    {
      Console.WriteLine(log);
      using (var file = File.OpenWrite(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logs.txt")))
      {
        var data = Encoding.UTF8.GetBytes($"{log}\r\n");
        file.Position = file.Length;
        file.Write(data, 0, data.Length);
      }
    }


  }
}
