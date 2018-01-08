using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Xpo;
using AutoMapper;

using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Data.Repository;
using System.Collections.Concurrent;
using Atlas.Enumerators;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation
{
  public static class WCFUtils
  {
    /// <summary>
    /// Performs minimal checks on request and returns the user/machine which matches
    /// </summary>
    /// <param name="request">Source request parameters to be checked</param>
    /// <param name="branchId">The BRN_Branch.BranchId</param>
    /// <param name="machine">The COR_Machine.MachineId</param>
    /// <param name="user">The PER_Person.Security and PER_Person info</param>
    /// <param name="errorMessage">Any error message</param>
    /// <returns>true if successfully processed the request and contained minimum values</returns>
    public static bool CheckSourceRequest(ILogging log, SourceRequest request, out Int64 branchId, out Machine machine, out User user, out string errorMessage)
    {
      branchId = -1;
      user = null;
      machine = null;

      #region Check minimum values are there
      if (string.IsNullOrEmpty(request.AppName))
      {
        errorMessage = "'AppName' cannot be empty";
        return false;
      }

      if (string.IsNullOrEmpty(request.AppVer))
      {
        errorMessage = "'AppVer' cannot be empty";
        return false;
      }

      if (request.MachineDateTime == DateTime.MinValue)
      {
        errorMessage = "'MachineDateTime' cannot be empty";
        return false;
      }

      if (string.IsNullOrEmpty(request.MachineIPAddresses))
      {
        errorMessage = "'MachineIPAddresses' cannot be empty";
        return false;
      }

      if (string.IsNullOrEmpty(request.MachineName))
      {
        errorMessage = "'MachineName' cannot be empty";
        return false;
      }

      if (string.IsNullOrEmpty(request.MachineUniqueID))
      {
        errorMessage = "'MachineUniqueID' cannot be empty";
        return false;
      }

      var minutesOut = (int)Math.Abs(request.MachineDateTime.Subtract(DateTime.Now).TotalMinutes);
      if (minutesOut > 10)
      {
        errorMessage = string.Format("Invalid system time- your system is {0} minutes out", minutesOut);
        return false;
      }
      #endregion

      #region If ASS, check branch code/user
      var application = request.AppName.ToUpper().Contains("ASS") ? General.ApplicationIdentifiers.ASS : General.ApplicationIdentifiers.ASSFingerprintClient;
      if (application == General.ApplicationIdentifiers.ASS)
      {
        if (string.IsNullOrEmpty(request.BranchCode))
        {
          errorMessage = "Branch code cannot be empty";
          return false;
        }

        /*if (string.IsNullOrEmpty(request.UserIDOrPassport) || request.UserIDOrPassport.Length < 4)
        {
          errorMessage = "User ID cannot be empty";
          return false;
        }
        */
        var branch = request.BranchCode.PadLeft(3, '0');

        if (!_branches.TryGetValue(branch, out branchId) || branchId <= 0)
        {
          using (var unitOfWork = new UnitOfWork())
          {
            var dbBranch = unitOfWork.Query<BRN_Branch>().FirstOrDefault(s =>
              s.LegacyBranchNum.PadLeft(3, '0') == request.BranchCode.Trim().PadLeft(3, '0'));
            if (dbBranch == null)
            {
              errorMessage = string.Format("Unknown branch code {0}", request.BranchCode);
              return false;
            }
            branchId = dbBranch.BranchId;
            _branches.TryAdd(branch, branchId);
          }
        }
      }
      #endregion

      #region Check IP addresses
      var ipAddresses = request.MachineIPAddresses.Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();
      var foundAtlasInternal = false;
      foreach (var ipAddress in ipAddresses)
      {
        // Check validity of IP address
        System.Net.IPAddress address;
        if (!System.Net.IPAddress.TryParse(ipAddress, out address))
        {
          errorMessage = string.Format("Invalid IP address: {0}", ipAddress);
          return false;
        }

        // Check if internal IP range
        if (ipAddress.StartsWith("192.168") || // Internal branches
           ipAddress.StartsWith("10.141.") || // APN/3G fail-over
           ipAddress.StartsWith("10.0.")) // HO/SDC/CC
        {
          foundAtlasInternal = true;
        }
      }
      if (!foundAtlasInternal)
      {
        errorMessage = string.Format("Could not locate the internal Atlas IP Address: '{0}'", request.MachineIPAddresses);
        return false;
      }

      ipAddresses.Sort();
      var sortedIPAddresses = string.Join(",", ipAddresses);
      #endregion

      // Try find machine in thread-safe list and return a copy
      lock (_machineLock)
      {
        var tsMachine = _machines.FirstOrDefault(s =>
          (s.IPAddresses == request.MachineIPAddresses || s.IPAddresses == sortedIPAddresses) &&
          s.Name == request.MachineName &&
          s.HardwareKey == request.MachineUniqueID);

        if (tsMachine != null)
        {
          tsMachine.LastAccessDT = DateTime.Now;
          machine = Mapper.Map<Machine>(tsMachine);
        }
      }

      #region Check DB if not found
      if ((user == null && !string.IsNullOrEmpty(request.UserIDOrPassport)) ||
        (user == null && request.UserPersonId > 0) || machine == null)
      {
        using (var unitOfWork = new UnitOfWork())
        {
          #region User
          if (user == null && !string.IsNullOrEmpty(request.UserIDOrPassport) ||
             (user == null && request.UserPersonId > 0))
          {
            PER_Person personDb = null;
            PER_Security securityDb = null;

            if (request.UserPersonId > 0)
            {
              personDb = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == request.UserPersonId);
              securityDb = personDb?.Security;
            }
            else if (request.UserIDOrPassport.Length < 6)
            {
              securityDb = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.LegacyOperatorId == request.UserIDOrPassport.Trim() && s.IsActive && s.Person != null);
              personDb = securityDb?.Person;
            }
            else if (securityDb == null)
            {
              personDb = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.IdNum == request.UserIDOrPassport.Trim());
              securityDb = personDb?.Security;
            }

            if (securityDb != null && personDb != null)
            {
              // Add to thread safe list
              user = new User(securityDb.SecurityId, personDb.PersonId, securityDb.LegacyOperatorId, personDb.IdNum);
              lock (_userLock)
              {
                _users.Add(user);
                user = Mapper.Map<User>(user);
              }
            }
            else
            {
              log.Error("Failed to locate {UserId}", request.UserIDOrPassport);

            }
          }
          #endregion

          #region Machine
          if (machine == null)
          {
            var machineDb = SecurityData.FindOrAddMachine(unitOfWork, request.MachineUniqueID, sortedIPAddresses, request.MachineName);

            // Add to thread-safe list
            var tsMachine = new Machine(machineDb.MachineId, request.MachineName, sortedIPAddresses, request.MachineUniqueID,
              DateTime.Now, machineDb.LastBranchCode != null ? machineDb.LastBranchCode.LegacyBranchNum : "");
            lock (_machineLock)
            {
              _machines.Add(tsMachine);
              machine = Mapper.Map<Machine>(tsMachine);
            }
          }
          #endregion
        }
      }
      #endregion

      errorMessage = string.Empty;

      return true;
    }


    #region Private vars

    /// <summary>
    /// Lock for _machines
    /// </summary>
    private static readonly object _machineLock = new object();

    /// <summary>
    /// Holds information about machines
    /// </summary>
    private static readonly List<Machine> _machines = new List<Machine>();

    /// <summary>
    /// Lock of _users
    /// </summary>
    private static readonly object _userLock = new object();

    /// <summary>
    /// Holds information about users
    /// </summary>
    private static readonly List<User> _users = new List<User>();

    /// <summary>
    /// List of Atlas branches- key is branch code and value is BRN_Branch.BranchId
    /// </summary>
    private static readonly ConcurrentDictionary<string, Int64> _branches = new ConcurrentDictionary<string, Int64>();

    #endregion
  }
}