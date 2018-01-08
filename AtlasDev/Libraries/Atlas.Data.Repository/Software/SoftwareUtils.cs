using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Enumerators;
using Atlas.Cache.DomainMapper;
using Atlas.Domain.Security;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Common.Extensions;


namespace Atlas.Data.Repository.Software
{
  public static class SoftwareUtils
  {
    /// <summary>
    /// Find or create the COR_Software, using given unit of work
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="appName"></param>
    /// <param name="appVersion"></param>
    /// <returns></returns>
    public static COR_Software_Cached FindOrAddSoftware(ICacheServer cache, UnitOfWork unitOfWork,
      string appName, string appVersion, string machineName)
    {
      var app = appName.FromStringToEnum<General.ApplicationIdentifiers>();
      if (app == General.ApplicationIdentifiers.NotSet)
      {
        throw new ArgumentOutOfRangeException("appName", string.Format("'{0}'- unregistered application", appName));
      }

      var cached = CacheDomainMapper.COR_Software_Mapper(
        unitOfWork.Query<COR_Software>().FirstOrDefault(s => s.AtlasApplication == app && s.AppVersion == appVersion));
      //var cached = cache.GetAll<COR_Software_Cached>().FirstOrDefault(s => s.AtlasApplication == (int)app && s.AppVersion == appVersion);
      if (cached == null)
      {
        var db = new COR_Software(unitOfWork)
        {
          AtlasApplication = app,
          AppVersion = appVersion,
          ReleasedDT = DateTime.Now,
          Comments = string.Format("Auto created version by machine '{0}'", machineName)
        };

        unitOfWork.CommitChanges();
        cached = CacheDomainMapper.COR_Software_Mapper(db);
        cache.Set(new List<COR_Software_Cached> { cached });
      }

      return cached;
    }



    /// <summary>
    /// Finds or add a COR_AppUsage
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="machineId"></param>
    /// <param name="atlasSoftwareId"></param>
    /// <param name="securityId"></param>
    /// <param name="branchId"></param>
    /// <returns></returns>
    public static COR_AppUsageDTO FindOrAddAppUsage(ICacheServer cache, UnitOfWork unitOfWork,
      Int64 machineId, Int64 atlasSoftwareId, long securityId, long branchId = 0)
    {
      var atlasSoftware = (General.ApplicationIdentifiers)atlasSoftwareId;
      var appUsage = unitOfWork.Query<COR_AppUsage>()
        .FirstOrDefault(s => s.Machine.MachineId == machineId &&
          s.User.SecurityId == securityId &&
          s.Application.AtlasApplication == atlasSoftware);

      if (appUsage == null)
      {
        appUsage = new COR_AppUsage(unitOfWork)
        {
          Machine = unitOfWork.Query<COR_Machine>().FirstOrDefault(s => s.MachineId == machineId),
          User = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.SecurityId == securityId),
          Application = unitOfWork.Query<COR_Software>().FirstOrDefault(s => s.AtlasSoftwareId == atlasSoftwareId),
          BranchCode = branchId > 0 ? unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branchId) : null
        };

        unitOfWork.CommitChanges();
      }
      else if (appUsage.BranchCode == null && branchId > 0)
      {
        appUsage.BranchCode = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branchId);
        unitOfWork.CommitChanges();
      }

      return AutoMapper.Mapper.Map<COR_AppUsageDTO>(appUsage);
    }

  }
}
