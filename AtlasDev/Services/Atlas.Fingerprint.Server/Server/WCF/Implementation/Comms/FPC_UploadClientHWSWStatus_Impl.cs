using System;
using System.Linq;

using DevExpress.Xpo;
using Newtonsoft.Json;

using Atlas.WCF.FPServer.Common;
using Atlas.Domain.Security;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class FPC_UploadClientHWSWStatus_Impl
  {
    public static void Execute(ILogging log, SourceRequest sourceRequest, byte[] machineInfo, out string errorMessage)
    {
      var methodName = "FPC_UploadClientHWSWStatus";
      log.Information("{MethodName} starting: {@Request}, Audit bytes: {Bytes}", methodName, sourceRequest, machineInfo != null ? machineInfo.Length : 0);
      errorMessage = string.Empty;
      try
      {
        #region Check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return;
        }

        if (machineInfo == null || machineInfo.Length == 0)
        {
          errorMessage = "machineInfo cannot be empty";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return;
        }

        var unzippedJson = Shared.Audit.AuditUtils.ZipUtils.DecompressString(machineInfo);

        if (string.IsNullOrEmpty(unzippedJson))
        {
          errorMessage = "Unzipped Json empty";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return;
        }

        #region Check is valid JSON and has members we expect
        var shortJson = unzippedJson.Length > 1000 ? unzippedJson.Substring(0, 1000) : unzippedJson;
        Atlas.Shared.Audit.AuditClasses.SysInfo hwInfo = null;
        try
        {
          hwInfo = JsonConvert.DeserializeObject<Atlas.Shared.Audit.AuditClasses.SysInfo>(unzippedJson);
          if (hwInfo == null)
          {
            throw new Exception("Failed to decode Json");
          }
        }
        catch (Exception err)
        {
          errorMessage = "Invalid machineInfo value";
          log.Warning(err, "{MethodName}- {@unzippedJson}", methodName, shortJson);
          return;
        }

        if (hwInfo.Created == DateTime.MinValue)
        {
          errorMessage = "Missing JSON 'Created'";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, hwInfo);
          return;
        }

        if (string.IsNullOrEmpty(hwInfo.MachineName))
        {
          errorMessage = "Missing JSON 'MachineName'";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, hwInfo);
          return;
        }

        if (hwInfo.Windows == null)
        {
          errorMessage = "Missing JSON 'Windows'";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, shortJson);
          return;
        }

        if (hwInfo.Drives == null)
        {
          errorMessage = "Missing JSON 'Drives'";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, hwInfo);
          return;
        }

        if (hwInfo.IP4Addresses == null)
        {
          errorMessage = "Missing JSON 'IP4Addresses'";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, hwInfo);
          return;
        }
                
        #endregion

        #endregion

        #region Save audit to DB
        using (var unitOfWork = new UnitOfWork())
        {
          new COR_LogMachineInfo(unitOfWork)
          {
            Machine = unitOfWork.Query<COR_Machine>().First(s => s.MachineId == machine.Id),
            CreatedDT = DateTime.Now,
            AuditJson = unzippedJson
          };

          unitOfWork.CommitChanges();
        }
        #endregion

        log.Information("{MethodName} completed successfully", methodName);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
      }
    }

  }
}
