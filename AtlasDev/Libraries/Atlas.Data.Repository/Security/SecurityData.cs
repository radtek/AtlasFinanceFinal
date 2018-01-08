
#region Using

using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Security;
using Atlas.Domain.Model;
using Atlas.Domain.DTO;
using Atlas.Common.Extensions;

#endregion


namespace Atlas.Data.Repository
{
  public static class SecurityData
  {
    /// <summary>
    /// Find or create the COR_Software, using given unit of work
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="appName"></param>
    /// <param name="appVersion"></param>
    /// <returns></returns>
    public static COR_SoftwareDTO FindOrAddSoftware(UnitOfWork unitOfWork, string appName, string appVersion, string machineName)
    {
      var app = appName.FromStringToEnum<Atlas.Enumerators.General.ApplicationIdentifiers>();
      if (app == Enumerators.General.ApplicationIdentifiers.NotSet)
      {
        throw new ArgumentOutOfRangeException("appName", string.Format("'{0}'- unregistered application"));
      }

      var software = unitOfWork.Query<COR_Software>().FirstOrDefault(s => s.AtlasApplication == app && s.AppVersion == appVersion);

      if (software == null)
      {
        software = new COR_Software(unitOfWork)
        {
          AtlasApplication = app,
          AppVersion = appVersion,
          ReleasedDT = DateTime.Now,
          Comments = string.Format("Auto created version by machine '{0}'", machineName)
        };

        unitOfWork.CommitChanges();
      }

      return AutoMapper.Mapper.Map<COR_SoftwareDTO>(software);
    }


    /// <summary>
    /// Finds or adds a COR_Machine to the DB
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="machineFingerprint"></param>
    /// <param name="machineIpAddresses"></param>
    /// <param name="machineName"></param>
    /// <param name="branchId"></param>
    /// <returns></returns>
    public static COR_MachineDTO FindOrAddMachine(UnitOfWork unitOfWork, string machineFingerprint, string machineIpAddresses, string machineName, Int64 branchId = 0)
    {
      var atlasIPAddress = machineIpAddresses.Split(",".ToCharArray())
        .FirstOrDefault(s => s.StartsWith("192.168") // Internal branches
          || s.StartsWith("10.141.") // APN
          || s.StartsWith("10.0.0.")); // Head office

      if (string.IsNullOrEmpty(atlasIPAddress))
      {
        throw new ArgumentOutOfRangeException("machineIpAddresses", string.Format("No Atlas IP addresses found: '{0}'", machineIpAddresses));
      }

      var machine = unitOfWork.Query<COR_Machine>()
        .FirstOrDefault(s => s.HardwareKey == machineFingerprint &&
          s.MachineIPAddresses.Contains(atlasIPAddress) &&
          s.MachineName == machineName);

      if (machine == null)
      {
        machine = new COR_Machine(unitOfWork)
        {
          MachineIPAddresses = machineIpAddresses,
          MachineName = machineName,
          HardwareKey = machineFingerprint,
          LastAccessDT = DateTime.Now,
          //LastBranchCode = branchId > 0 ? unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branchId) : null // No- LastBranchCode must be set manually as indicates authorised branch db
        };

        unitOfWork.CommitChanges();
      }
      else if (machine.LastBranchCode == null && branchId > 0)
      {
        // No- LastBranchCode must be set manually as indicates authorised branch db
        //result.LastBranchCode = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branchId);
        //unitOfWork.CommitChanges();
      }

      return AutoMapper.Mapper.Map<COR_MachineDTO>(machine);
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
    public static COR_AppUsageDTO FindOrAddAppUsage(UnitOfWork unitOfWork, Int64 machineId, Int64 atlasSoftwareId, Int64 securityId, Int64 branchId = 0)
    {
      var machine = unitOfWork.Query<COR_Machine>().First(s => s.MachineId == machineId);

      PER_Security security = null;
      if (securityId > 0)
      {
        security = unitOfWork.Query<PER_Security>().First(s => s.SecurityId == securityId);
      }

      COR_Software software = null;
      if (atlasSoftwareId > 0)
      {
        software = unitOfWork.Query<COR_Software>().First(s => s.AtlasSoftwareId == atlasSoftwareId);
      }
            
      var appUsage = unitOfWork.Query<COR_AppUsage>().FirstOrDefault(s => s.Machine == machine && s.User == security && s.Application == software);
      if (appUsage == null)
      {
        appUsage = new COR_AppUsage(unitOfWork)
        {
          Machine = machine,
          User = security,
          Application = software,
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


    public static PER_SecurityDTO FindByIdOrLegacyOperator(string idOrPassportOrLegacyOperatorId, UnitOfWork unitOfWork = null)
    {
      if (unitOfWork == null)
      {
        using (var unitOfWorkInt = new UnitOfWork())
        {
          return _FindByIdOrLegacyOperator(idOrPassportOrLegacyOperatorId, unitOfWorkInt);
        }
      }
      else
      {
        return _FindByIdOrLegacyOperator(idOrPassportOrLegacyOperatorId, unitOfWork);
      }      
    }


    private static PER_SecurityDTO _FindByIdOrLegacyOperator(string idOrPassportOrLegacyOperatorId, UnitOfWork unitOfWork)
    {
      PER_Security user = null;
      if (idOrPassportOrLegacyOperatorId.Length > 4)
      {
        user = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.Person.IdNum == idOrPassportOrLegacyOperatorId);
        if (user != null)
        {
          return AutoMapper.Mapper.Map<PER_SecurityDTO>(user);
        }
      }

      user = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.LegacyOperatorId == idOrPassportOrLegacyOperatorId);
      if (user != null)
      {
        return AutoMapper.Mapper.Map<PER_SecurityDTO>(user);
      }

      return null;
    }


    public static PER_Security FindByPersonId(UnitOfWork unitOfWork, Int64 personId)
    {
      return unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.Person.PersonId == personId);
    }

  }
}
