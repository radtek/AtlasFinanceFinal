using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Utils;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;


namespace ASSServer.WCF.Implementation.DataSync
{
  
  public static class GetBranchSettings_Impl
  {
    public static List<KeyValueItem> Execute(ILogging log, ICacheServer cache, IConfigSettings config, SourceRequest sourceRequest)
    {     
      var methodName = "GetBranchSettings";
      var result = new List<KeyValueItem>();
      try
      {
        log.Information("{MethodName} starting- {@Request}", methodName, sourceRequest);

        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return null;
        }

        var branch = cache.Get<BRN_Branch_Cached>(server.Branch.Value);
        using (var unitOfWork = new UnitOfWork())
        {
          var settingsDb = unitOfWork.Query<BRN_Config>().Where(s => s.Branch.BranchId == branch.BranchId && !string.IsNullOrEmpty(s.DataSection));
          var cryptIV = settingsDb.Where(s => s.DataSection == "A").FirstOrDefault();
          var cryptPass = settingsDb.Where(s => s.DataSection == "B").FirstOrDefault();
          var sqlUsername = settingsDb.Where(s => s.DataSection == "C").FirstOrDefault();
          var sqlPassword = settingsDb.Where(s => s.DataSection == "D").FirstOrDefault();

          var dbRecordAdded = false;

          if (cryptIV == null)
          {
            cryptIV = new BRN_Config(unitOfWork)
            {
              Branch = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == server.Branch),
              DataSection = "A",
              DataValue = StringUtils.RandomString(50),
              CreatedDT = DateTime.Now,
              Description = string.Format("PostgreSQL creds- Salt- Branch '{0}'", server.Branch)
            };
            dbRecordAdded = true;
          }

          if (cryptPass == null)
          {
            cryptPass = new BRN_Config(unitOfWork)
            {
              Branch = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == server.Branch),
              DataSection = "B",
              DataValue = StringUtils.RandomString(15),
              CreatedDT = DateTime.Now,
              Description = string.Format("PostgreSQL creds- Password- Branch '{0}'", server.Branch)
            };
            dbRecordAdded = true;
          }

          if (sqlUsername == null)
          {
            sqlUsername = new BRN_Config(unitOfWork)
            {
              Branch = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == server.Branch),
              DataSection = "C",
              DataValue = BasicCrypto.Encrypt(cryptIV.DataValue, cryptPass.DataValue, StringUtils.RandomString(10).ToLower()),
              CreatedDT = DateTime.Now,
              Description = string.Format("PostgreSQL crypted username- Branch '{0}'", server.Branch)
            };
            dbRecordAdded = true;
          }

          if (sqlPassword == null)
          {
            sqlPassword = new BRN_Config(unitOfWork)
            {
              Branch = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branch.BranchId),
              DataSection = "D",
              DataValue = BasicCrypto.Encrypt(cryptIV.DataValue, cryptPass.DataValue, StringUtils.RandomString(15)),
              CreatedDT = DateTime.Now,
              Description = string.Format("PostgreSQL crypted password- Branch '{0}'", branch.LegacyBranchNum)
            };
            dbRecordAdded = true;
          }

          if (dbRecordAdded)
          {
            unitOfWork.CommitChanges();
          }

          foreach (var setting in settingsDb)
          {
            result.Add(new KeyValueItem() { Key = setting.DataSection, Value = setting.DataValue });
          }
        }

        return result;
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);
        return null;
      }
    }

  }
}
