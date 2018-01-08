using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DevExpress.Xpo;
using Atlas.Common.Interface;

using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Common.Utils;
using Atlas.Cache.Interfaces;
using Atlas.Server.Classes.CustomException;
using Atlas.Data.Repository;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class GetDBSettings_Impl
  {
    internal static int Execute(ILogging log, IConfigSettings config, ICacheServer cache,
      string machineIP, string machineName, string machineFP, string env_branch, string env_live,
      out string branchNum, out string rddType, out string connectionString, out string schemaName,
      out string errorMessage)
    {
      branchNum = string.Empty;
      rddType = "DBFNTX";
      connectionString = string.Empty;
      errorMessage = string.Empty;
      schemaName = string.Empty;

      var methodName = "GetDBSettings";

      log.Information("{MethodName} Machine: {@Parameters}", methodName,
        new { machineIP, machineName, machineFP, env_branch, env_live });

      try
      {
        #region Check parameters
        if (string.IsNullOrEmpty(machineIP) || machineIP.Length < 8)
        {
          throw new BadParamException("Invalid IP address");
        }

        if (string.IsNullOrEmpty(machineName) || machineName.Length < 6)
        {
          throw new BadParamException("Invalid machine name");
        }

        if (string.IsNullOrEmpty(machineFP) || machineFP.Length < 6)
        {
          throw new BadParamException("Invalid machine fingerprint");
        }
        #endregion

        // TODO: Lots of repetitive code follows- make DRY

        #region Special case- Michael's PC- contains SQL old call centre server + merged client.dbf - allow to choose via env_branch ('0c3' = SQL, else DBF)
        if (machineName.ToLower() == "0c3-00-07")
        {
          if (string.IsNullOrEmpty(env_branch) || env_branch.Length != 3)
          {
            throw new BadParamException("Missing environment parameters");
          }

          if (env_branch.ToLower() == "0c3") // live SQL on local machine
          {
            using (var unitOfWork = new UnitOfWork())
            {
              // Settings
              var branch = unitOfWork.Query<BRN_Branch>().First(s => s.LegacyBranchNum.PadLeft(3, '0') == env_branch);
              var settingsDb = unitOfWork.Query<BRN_Config>().Where(s => s.Branch == branch);
              var cryptIV = settingsDb.Where(s => s.DataSection == "A").FirstOrDefault();
              var cryptPass = settingsDb.Where(s => s.DataSection == "B").FirstOrDefault();
              var sqlUsername = settingsDb.Where(s => s.DataSection == "C").FirstOrDefault();
              var sqlPassword = settingsDb.Where(s => s.DataSection == "D").FirstOrDefault();

              if (cryptIV == null || cryptPass == null || sqlUsername == null || sqlPassword == null)
              {
                log.Error(new Exception(string.Format("Missing SQL ASS_Settings for branch {0}", branch.LegacyBranchNum)), methodName);
                return (int)General.WCFCallResult.ServerError;
              }

              // Server IP
              var branchServer = unitOfWork.Query<ASS_BranchServer>().FirstOrDefault(s => s.Branch == branch);
              if (branchServer == null)
              {
                throw new BadParamException($"Branch '{branchNum}' does not have a registered server");
              }

              var ips = branchServer.Machine.MachineIPAddresses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

              // Machine is at HO
              var serverIP = ips.FirstOrDefault(s => s.StartsWith("10.0.0"));
              if (string.IsNullOrEmpty(serverIP))
              {
                throw new BadParamException($"Branch '{branchNum}' does not have a server with an expected IP");
              }

              var connStr = string.Format("PGS={0};UID={1};PWD={2};DTB=ass;PRT=5432",
                serverIP,
                BasicCrypto.Decrypt(cryptIV.DataValue, cryptPass.DataValue, sqlUsername.DataValue),
                BasicCrypto.Decrypt(cryptIV.DataValue, cryptPass.DataValue, sqlPassword.DataValue));

              log.Information("Machine: {0}, IP: {1}, Fingerprint:{2}, Connection string returned: '{3}'", machineIP, machineName, machineFP, connStr);

              rddType = ASSUtils.ASSEncrypt("SQLRDD");
              connectionString = ASSUtils.ASSEncrypt(connStr);

              return (int)General.WCFCallResult.OK;
            }
          }
          else
          {
            log.Information("Machine: {0}, IP: {1}, Fingerprint:{2} -> Use DBF", machineIP, machineName, machineFP);
            return (int)General.WCFCallResult.BadParams;// use DBF
          }
        }
        #endregion

        #region Special case- RemoteApp/training/read-only function to connect to HO
        var allowedTSMachines = new List<string>
        {
          "HO-SRV-TS-2", // TS Server
          "000-AC-01"    // Jackie Govendor- ops          
        };

        if (allowedTSMachines.Contains(machineName) && machineIP.StartsWith("10.0.0"))
        {
          var wantLiveDb = false;

          if (string.IsNullOrEmpty(env_branch) && string.IsNullOrEmpty(env_live) ||
            env_branch.Length != 3 || !bool.TryParse(env_live, out wantLiveDb))
          {
            throw new BadParamException("Missing environment parameters");
          }

          rddType = ASSUtils.ASSEncrypt("SQLRDD");

          var connStr = wantLiveDb ?
            "PGS=10.0.0.245;UID=postgres;PWD=s1DT81ChqlVkPZMlRO8b;DTB=ass_ro;PRT=5432" :
            "PGS=10.0.0.245;UID=postgres;PWD=s1DT81ChqlVkPZMlRO8b;DTB=ass;PRT=5432";

          schemaName = string.Format("br{0}", env_branch);
          log.Information("RemoteApp Machine: {MachineIP}, {MachineName}, Fingerprint:{MachineId}- " +
            "Connection string returned: {ConnectionString}, Schema: {SchemaName}",
            machineIP, machineName, machineFP, connStr, schemaName);

          connectionString = ASSUtils.ASSEncrypt(connStr);
          return (int)General.WCFCallResult.OK;
        }
        #endregion

        #region Special case- can connect directly to branches- Hub
        var allowedDirectMachines = new List<string> {
          "000-AU-01", /* Tina Bothma- auditor - use to fix issues 2015-08-05 - Joggie appv. */
          "000-TR-20" /* HUB */,
          "000-AC-12" /* Anne */,
          "MINDA" /* Minda */,
          "000-OP-22"  /* Sylvia- OPs manger */};

        if ((Regex.IsMatch(machineName, "(HUB)|(OP)\\-\\d{2,2}") ||
          allowedDirectMachines.Contains(machineName)) && !string.IsNullOrEmpty(env_branch) && env_branch.Length == 3)
        {
          using (var unitOfWork = new UnitOfWork())
          {
            // Settings
            var branch = AtlasData.FindBranch(env_branch);
            var settingsDb = unitOfWork.Query<BRN_Config>().Where(s => s.Branch != null && s.Branch.BranchId == branch.BranchId);
            var cryptIV = settingsDb.Where(s => s.DataSection == "A").FirstOrDefault();
            var cryptPass = settingsDb.Where(s => s.DataSection == "B").FirstOrDefault();
            var sqlUsername = settingsDb.Where(s => s.DataSection == "C").FirstOrDefault();
            var sqlPassword = settingsDb.Where(s => s.DataSection == "D").FirstOrDefault();

            if (cryptIV == null || cryptPass == null || sqlUsername == null || sqlPassword == null)
            {
              throw new BadParamException($"Missing SQL ASS_Settings for branch {branch.LegacyBranchNum}");
            }

            // Server IP
            var branchServer = unitOfWork.Query<ASS_BranchServer>().FirstOrDefault(s => s.Branch != null && s.Branch.BranchId == branch.BranchId);
            if (branchServer == null)
            {
              throw new BadParamException($"Branch '{branchNum}' does not have a registered server");
            }

            string serverIP = null;
            var serverIPs = branchServer.Machine.MachineIPAddresses;
            var ips = serverIPs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var ip in ips)
            {
              // All branches should start with 192.168...
              if (ip.StartsWith("192.168."))
              {
                serverIP = ip;
                break;
              }
              if (ip.StartsWith("10.14")) // 3g fail-over
              {
                serverIP = ip;
                break;
              }
              // Use as long as not loopback, but keep scanning
              if (ip != "127.0.0.1")
              {
                serverIP = ip;
              }
            }

            if (string.IsNullOrEmpty(serverIP))
            {
              throw new BadParamException($"Branch '{branchNum}' does not have a server with an expected IP");
            }

            var connStr = string.Format("PGS={0};UID={1};PWD={2};DTB=ass;PRT=5432",
              serverIP,
              BasicCrypto.Decrypt(cryptIV.DataValue, cryptPass.DataValue, sqlUsername.DataValue),
              BasicCrypto.Decrypt(cryptIV.DataValue, cryptPass.DataValue, sqlPassword.DataValue));

            log.Information("Machine: {0}, IP: {1}, Fingerprint:{2}, Connection string returned: '{3}'", machineIP, machineName, machineFP, connStr);

            rddType = ASSUtils.ASSEncrypt("SQLRDD");
            connectionString = ASSUtils.ASSEncrypt(connStr);

            return (int)General.WCFCallResult.OK;
          }
        }

        #endregion

        using (var unitOfWork = new UnitOfWork())
        {
          var machine = SecurityData.FindOrAddMachine(unitOfWork, machineFP, machineIP, machineName);
          if (machine.LastBranchCode == null)
          {
            errorMessage = string.Format("No last branch number for machine '{0}'- IPs: {1}- assuming 'DBFNTX'", machineName, machineIP);
            log.Warning(new Exception(errorMessage), methodName);
            rddType = ASSUtils.ASSEncrypt("DBFNTX");
            return (int)General.WCFCallResult.OK;
          }

          #region Get branch RDD name
          var branchConfig = unitOfWork.Query<BRN_Config>().FirstOrDefault(s =>
            s.Branch.BranchId == machine.LastBranchCode.BranchId && s.DataSection == "RDDTYPE");
          if (branchConfig == null)
          {
            errorMessage = string.Format("No SQL RDD name for branch id {0}- assuming 'DBFNTX'", machine.LastBranchCode.LegacyBranchNum);
            log.Warning(new Exception(errorMessage), methodName);
            rddType = ASSUtils.ASSEncrypt("DBFNTX");
            return (int)General.WCFCallResult.OK;
          }

          rddType = ASSUtils.ASSEncrypt(branchConfig.DataValue);
          // If DBFNTX, we are done...
          if (branchConfig.DataValue == "DBFNTX")
          {
            return (int)General.WCFCallResult.OK;
          }
          #endregion

          #region Get branch connection string
          var settingsDb = unitOfWork.Query<BRN_Config>().Where(s => s.Branch.BranchId == machine.LastBranchCode.BranchId);
          var cryptIV = settingsDb.Where(s => s.DataSection == "A").FirstOrDefault();
          var cryptPass = settingsDb.Where(s => s.DataSection == "B").FirstOrDefault();
          var sqlUsername = settingsDb.Where(s => s.DataSection == "C").FirstOrDefault();
          var sqlPassword = settingsDb.Where(s => s.DataSection == "D").FirstOrDefault();

          if (cryptIV == null || cryptPass == null || sqlUsername == null || sqlPassword == null)
          {
            throw new BadParamException($"Missing SQL ASS_Settings for branch id {machine.LastBranchCode.BranchId}");
          }

          var branchServer = unitOfWork.Query<ASS_BranchServer>().FirstOrDefault(s => s.Branch.BranchId == machine.LastBranchCode.BranchId);
          if (branchServer == null)
          {
            throw new BadParamException($"Branch '{branchNum}' does not have a registered server");
          }

          string serverIP = null;
          var serverIPs = branchServer.Machine.MachineIPAddresses;
          var ips = serverIPs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

          foreach (var ip in ips)
          {
            // All branches should start with 192.168...
            if (ip.StartsWith("192.168."))
            {
              serverIP = ip;
              break;
            }
            // Use as long as not loopback, but keep scanning
            if (ip != "127.0.0.1")
            {
              serverIP = ip;
            }
          }

          if (string.IsNullOrEmpty(serverIP))
          {
            throw new BadParamException($"Branch '{branchNum}' does not have a server with an expected IP");
          }

          var connStr = string.Format("PGS={0};UID={1};PWD={2};DTB=ass;PRT=5432",
            serverIP,
            BasicCrypto.Decrypt(cryptIV.DataValue, cryptPass.DataValue, sqlUsername.DataValue),
            BasicCrypto.Decrypt(cryptIV.DataValue, cryptPass.DataValue, sqlPassword.DataValue));

          log.Information("Machine: {0}, IP: {1}, Fingerprint:{2}, Connection string returned: '{3}'", machineIP, machineName, machineFP, connStr);
          connectionString = ASSUtils.ASSEncrypt(connStr);
          #endregion

          return (int)General.WCFCallResult.OK;
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return (err is BadParamException) ? (int)General.WCFCallResult.BadParams : (int)General.WCFCallResult.ServerError;
      }
    }

  }
}
