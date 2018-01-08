using System;

using DevExpress.Xpo;
using Newtonsoft.Json.Linq;

using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Common.Utils;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class FPC_UploadClientFPStatus_Impl
  {
    public static void Execute(ILogging log, ICacheServer cache, SourceRequest sourceRequest, byte[] fpInfo, out string errorMessage)
    {
      var methodName = "FPC_UploadClientFPStatus";
      //_log.Information("{MethodName} starting: {FPInfo}", methodName, fpInfo);
      errorMessage = null;
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

        if (fpInfo == null || fpInfo.Length == 0)
        {
          errorMessage = "fpInfo cannot be empty";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return;
        }

        var unzippedJson = GZipUtils.DecompressString(fpInfo);
        JObject hwInfo;
        try
        {
          hwInfo = JObject.Parse(unzippedJson);
        }
        catch (Exception err)
        {
          log.Warning(err, "{MethodName}- {@Request}", methodName, sourceRequest);
          errorMessage = "Invalid fpInfo value";
          return;
        }
        #endregion

        var fpDevices = hwInfo["FPDevices"] as JArray;
        if (fpDevices == null)
        {
          errorMessage = "Missing JSON array element 'FPDevices'";
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return;
        }

        var count = 0;
        foreach (var fpDevice in fpDevices)
        {
          count++;
          var serial = (string)fpDevice["USN"];

          // TODO: Log this serial number with machine.... this gets called regularly, so need to update DB with some
          // kind of batching update process, else we'll get lock violations like TCC/Sync servers...
        }

        ClientState.LMSGuiState.LmsGuiUpdateHWStatus(sourceRequest.MachineUniqueID, count, DateTime.Now);
        //log.Information("{MethodName} completed successfully", methodName);

        using (var uow = new UnitOfWork())
        {
          var machineDb = Data.Repository.SecurityData.FindOrAddMachine(uow, sourceRequest.MachineUniqueID,
            sourceRequest.MachineIPAddresses, sourceRequest.MachineName);

          cache.GetAndUpdateLocked<COR_Machine_Cached>(machineDb.MachineId, machineCache =>
          {
            if (machineCache == null) // newly added machine- add to cache...
            {
              machineCache = new COR_Machine_Cached()
              {
                MachineId = machineDb.MachineId,             
                LastBranchId = machineDb.LastBranchCode?.BranchId ?? 0,
                HardwareKey = machineDb.HardwareKey,
                MachineIPAddresses = machineDb.MachineIPAddresses,
                MachineName = machineDb.MachineName
              };
            }
           
            machineCache.LastAccessDT = DateTime.Now;

            return machineCache;
          });
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }
    }
  }
}
