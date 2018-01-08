using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Common.Interface;


namespace ASSServer.Cache
{
  /// <summary>
  /// Class to compensate for Atlas XPO domain's shortcomings- the below are way too expensive with XPO queries.
  /// We load them from the Redis cache (to avoid XPO or adding Dapper) into RAM and refresh them every 60 seconds.
  /// The cache load takes about 270ms and only occurs once per minute.
  /// </summary>
  static class DataCache
  {
    internal static void Init(ICacheServer cache, ILogging log)
    {
      _cache = cache;
      _log = log;
      _log.Information("Cache initialized");
      OnRefreshTimer(null);
    }


    internal static COR_Machine_Cached GetMachine(string machineUniqueId, string machineIpAddresses, string machineName)
    {
      if (_cache == null || _log == null)
      {
        return null;
      }

      try
      {
        var atlasIPAddress = machineIpAddresses.Split(",".ToCharArray())
            .FirstOrDefault(s => s.StartsWith("192.168") // Internal branches
              || s.StartsWith("10.141.12.")              // APN VSAT
              || s.StartsWith("10.0.0."));               // Head office

        lock (_locker)
        {
          if (!string.IsNullOrEmpty(atlasIPAddress))
          {
            var machine = _machines.FirstOrDefault(s =>
               s.HardwareKey == machineUniqueId &&
               s.MachineIPAddresses.Contains(atlasIPAddress) &&
               s.MachineName == machineName);

            if (machine != null)
            {
              return _cache.Get<COR_Machine_Cached>(machine.MachineId);
            }
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "GetMachine()");
      }
      return null;
    }


    internal static ASS_BranchServer_Cached GetBranchServerViaMachineDetails(string machineName, string machineUniqueID, string machineIPAddresses)
    {
      if (_cache == null || _log == null)
      {
        return null;
      }
      
      try
      {
        var atlasIPAddress = machineIPAddresses.Split(",".ToCharArray())
            .FirstOrDefault(s => s.StartsWith("192.168") // Internal branches
              || s.StartsWith("10.141.12.")              // APN VSAT
              || s.StartsWith("10.0.0."));               // Head office

        lock (_locker)
        {
          if (!string.IsNullOrEmpty(atlasIPAddress))
          {
            var machineId = _machines.Where(s =>
                s.HardwareKey == machineUniqueID &&
                s.MachineIPAddresses.Contains(atlasIPAddress) &&
                s.MachineName == machineName).Select(s => s.MachineId).FirstOrDefault();
                    
            if (machineId > 0)
            {              
              var server = _branchServers.FirstOrDefault(s => s.Machine != null && s.Machine.Value == machineId);
              if (server != null)
              {
                return _cache.Get<ASS_BranchServer_Cached>(server.BranchServerId);
              }
            }
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "GetBranchServerViaMachineDetails()");
      }

      return null;
    }


    internal static ASS_BranchServer_Cached GetBranchServerViaBranchNum(string branchCode)
    {
      if (_cache == null || _log == null)
      {
        return null;
      }

      try
      {
        branchCode = branchCode.PadLeft(3, '0');
        lock (_locker)
        {
          var branch = _branches.FirstOrDefault(s => s.LegacyBranchNum == branchCode);
          if (branch != null)
          {
            var server = _branchServers.FirstOrDefault(s => s.Branch == branch.BranchId);
            if (server != null)
            {
              return _cache.Get<ASS_BranchServer_Cached>(server.BranchServerId);
            }
          }
        }

      }
      catch (Exception err)
      {
        _log.Error(err, "GetBranchServerViaBranchNum()");
      }

      return null;
    }


    internal static BRN_Branch_Cached GetBranch(string branchCode)
    {
      branchCode = branchCode.PadLeft(3, '0');
      var branch = _branches.FirstOrDefault(s => s.LegacyBranchNum == branchCode);
      if (branch != null)
      {
        return _cache.Get<BRN_Branch_Cached>(branch.BranchId);
      }
      
      return null;
    }


    #region Private methods

    private static void OnRefreshTimer(object state)
    {
      if (_cache == null || _log == null)
      {
        return;
      }

      var refreshTimer = System.Diagnostics.Stopwatch.StartNew();
      var machinesCount = 0;
      var branchServerCount = 0;
      var branchCount = 0;
      try
      {
        var machines = _cache.GetAll<COR_Machine_Cached>().ToList();
        machinesCount = machines.Count;

        var branchServers = _cache.GetAll<ASS_BranchServer_Cached>().ToList();
        branchServerCount = branchServers.Count;

        var branches = _cache.GetAll<BRN_Branch_Cached>().ToList();
        branchCount = branches.Count;

        branches.ForEach(s => s.LegacyBranchNum = s.LegacyBranchNum.PadLeft(3, '0'));

        lock (_locker)
        {
          _machines = machines;
          _branchServers = branchServers;
          _branches = branches;
        }

        refreshTimer.Stop();
        _log.Information("Updated local cache: Machines: {Machines}, Servers: {Servers}, Branches: {Branches} in {Elapsed}ms",
          machinesCount, branchServerCount, branchCount, refreshTimer.ElapsedMilliseconds);
      }
      catch (Exception err)
      {
        _log.Error(err, "OnRefreshTimer");
      }
    }

    #endregion


    #region Private fields

    private static object _locker = new object();
    private static List<COR_Machine_Cached> _machines = new List<COR_Machine_Cached>();
    private static List<ASS_BranchServer_Cached> _branchServers = new List<ASS_BranchServer_Cached>();
    private static List<BRN_Branch_Cached> _branches = new List<BRN_Branch_Cached>();

    private static ICacheServer _cache = null;
    private static Timer _refreshCache = new Timer(OnRefreshTimer, null, 60000, 60000);
    private static ILogging _log = null;

    #endregion

  }
}
