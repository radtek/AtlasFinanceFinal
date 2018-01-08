using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Security;
using Atlas.Enumerators;

using Atlas.Cache.Interfaces;
using Atlas.Common.Interface;
using AtlasServer.WCF.Interface;
using Atlas.Server.Utils;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Server.Classes.CustomException;
using Atlas.Data.Repository;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class CheckSoftware_Impl
  {
    internal static int Execute(ILogging log, ICacheServer cache,
      SourceRequest sourceRequest, out string serverAppVer, out string errorMessage)
    {
      errorMessage = string.Empty;
      serverAppVer = string.Empty;
      var methodName = "CheckSoftware";
      try
      {
        #region Check params
        if (string.IsNullOrEmpty(sourceRequest.BranchCode))
        {
          throw new BadParamException("Missing branch number");
        }

        if (string.IsNullOrEmpty(sourceRequest.UserIDOrPassport))
        {
          errorMessage = "Missing operator ID";
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(sourceRequest.MachineUniqueID))
        {
          errorMessage = "Missing machine fingerprint";
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(sourceRequest.AppVer))
        {
          errorMessage = "Missing application version";
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        using (var unitOfWork = new UnitOfWork())
        {          
          var software = SecurityData.FindOrAddSoftware(unitOfWork, sourceRequest.AppName, sourceRequest.AppVer, sourceRequest.MachineName);
          
          // TODO: Single cache item      
          // Return the most recent version of ASS, using release date for sort... (version number?)
          serverAppVer = cache.GetAll<COR_Software_Cached>()
            .Where(s => s.AtlasApplication == (int)General.ApplicationIdentifiers.ASS)
            .OrderByDescending(s => s.ReleaseDT)
            .Select(s => s.AppVersion).First();

          var dbBranch = AtlasData.FindBranch(sourceRequest.BranchCode);
          if (dbBranch == null)
          {
            errorMessage = "Invalid branch number";
            return (int)General.WCFCallResult.BadParams;
          }
          var machine = SecurityData.FindOrAddMachine(unitOfWork, sourceRequest.MachineUniqueID,
            sourceRequest.MachineIPAddresses, sourceRequest.MachineName/*, dbBranch.BranchId*/);

          var userInDb = DbGeneralUtils.FindByIdOrLegacyOperator(sourceRequest.UserIDOrPassport);
          if (userInDb == null)
          {
            errorMessage = "Unknown operator ID";
            return (int)General.WCFCallResult.BadParams;
          }

          var appUsage = SecurityData.FindOrAddAppUsage(unitOfWork, machine.MachineId, software.AtlasSoftwareId, userInDb.SecurityId, dbBranch.BranchId);

          new COR_LogAppUsage(unitOfWork)
          {
            AppUsageId = unitOfWork.Query<COR_AppUsage>().First(s => s.AppUsageId == appUsage.AppUsageId),
            EventDT = DateTime.Now,
            AtlasEvent = General.AuditStatusType.Authorised
          };

          unitOfWork.CommitChanges();
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return (err is BadParamException) ? (int)General.WCFCallResult.BadParams : (int)General.WCFCallResult.ServerError;
      }

      return (int)General.WCFCallResult.OK;
    }
  }
}
