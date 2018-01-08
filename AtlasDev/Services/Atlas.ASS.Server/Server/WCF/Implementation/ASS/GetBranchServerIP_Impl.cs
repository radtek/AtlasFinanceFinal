using System;
using System.Linq;

using Atlas.Common.Interface;
using DevExpress.Xpo;

using Atlas.Enumerators;
using AtlasServer.WCF.Interface;

using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Server.Classes.CustomException;
using Atlas.Data.Repository;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class GetBranchServerIP_Impl
  {
    internal static int Execute(ILogging log, IConfigSettings config, ICacheServer cache, 
      SourceRequest sourceRequest, out string serverIP, out string errorMessage)
    {
      errorMessage = string.Empty;
      serverIP = string.Empty;

      var methodName = "GetBranchServerIP";
      try
      {
        #region Check params
        if (sourceRequest == null)
        {
          throw new BadParamException($"Missing sourceRequest value");       
        }
        if (string.IsNullOrEmpty(sourceRequest.MachineName))
        {
          throw new BadParamException($"Missing machine name");
        }
        if (string.IsNullOrEmpty(sourceRequest.MachineUniqueID))
        {
          throw new BadParamException($"Missing machine fingerprint");
        }
        #endregion

        using (var unitOfWork = new UnitOfWork())
        {
          var thisMachine = SecurityData.FindOrAddMachine(unitOfWork, sourceRequest.MachineUniqueID,
            sourceRequest.MachineIPAddresses, sourceRequest.MachineName);

          if (thisMachine?.LastBranchCode == null)
          {
            throw new BadParamException($"Machine '{sourceRequest.MachineName}' is not linked to a branch");
          }

          var branchServer = cache.GetAll<ASS_BranchServer_Cached>().FirstOrDefault(s => s.Branch == thisMachine.LastBranchCode.BranchId);
          if (branchServer?.Machine == null)
          {
            throw new BadParamException($"Branch id '{thisMachine.LastBranchCode.BranchId}' does not have a registered server");
          }

          var serverMachine = cache.Get<COR_Machine_Cached>(branchServer.Machine.Value);
          if (string.IsNullOrEmpty(serverMachine?.MachineIPAddresses))
          {
            throw new BadParamException($"Machine id '{branchServer.Machine.Value}' cannot be found or does not have an IP address");
          }

          serverIP = serverMachine.MachineIPAddresses;
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
