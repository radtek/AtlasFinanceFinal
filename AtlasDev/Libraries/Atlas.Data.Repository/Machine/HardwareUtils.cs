using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Cache.DomainMapper;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Domain.Security;
using Atlas.Common.Interface;
using Atlas.Domain.Model;


namespace Atlas.Data.Repository.Machine
{
  public static class HardwareUtils
  {
    /// <summary>
    /// Finds or adds a COR_Machine to the DB and updates cache
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <param name="machineFingerprint"></param>
    /// <param name="machineIpAddresses"></param>
    /// <param name="machineName"></param>
    /// <param name="branchId"></param>
    /// <returns></returns>
    public static COR_Machine_Cached GetOrAddMachine(ICacheServer cache, UnitOfWork unitOfWork,
      string machineFingerprint, string machineIpAddresses, string machineName)
    {
      var atlasIPAddress = machineIpAddresses.Split(",".ToCharArray())
        .FirstOrDefault(s => s.StartsWith("192.168") // Internal branches
          || s.StartsWith("10.141.") // APN
          || s.StartsWith("10.0.0.")); // Head office

      if (string.IsNullOrEmpty(atlasIPAddress))
      {
        throw new ArgumentOutOfRangeException("machineIpAddresses", string.Format("No Atlas IP addresses found: '{0}'", machineIpAddresses));
      }

      // There are thousands of machines, PSQL will be quicker than: a Redis fetch, deserialization of all machines and then a LINQ filter...
      // TODO: If we used the L1.Redis cache (https://github.com/johnnycardy/StackRedis.L1), then that would be faster than PSQL
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
          //LastBranchCode = No- LastBranchCode must be set manually as indicates authorised branch db
        };

        unitOfWork.CommitChanges();

        cache.Set(new List<COR_Machine_Cached> { CacheDomainMapper.COR_Machine_Mapper(machine) });
      }

      COR_Machine_Cached machineCache = null;
      cache.GetAndUpdateLocked<COR_Machine_Cached>(machine.MachineId, (cached) =>
      {
        if (cached == null)
        {
          cached = new COR_Machine_Cached
          {
            HardwareKey = machine.HardwareKey,
            LastBranchId = machine.LastBranchCode?.BranchId ?? 0,
            MachineId = machine.MachineId,
            MachineIPAddresses = machine.MachineIPAddresses,
            MachineName = machine.MachineName
          };
        }

        cached.LastAccessDT = DateTime.Now;
        machineCache = cached;
        return cached;
      });

      return machineCache;
    }

  }
}
