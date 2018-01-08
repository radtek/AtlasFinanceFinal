/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013-2016 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *     WCF support utilities- validate source request values and cache values to avoid hitting the DB
 *   
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */


using System;
using System.Linq;
using System.Net;

using Atlas.DataSync.WCF.Interface;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Common.Interface;
using ASSServer.Cache;


namespace ASSServer.WCF.Implementation
{
  public static class Checks
  {
    #region Public methods

    /// <summary>
    /// Verify branch server
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="server">The server found</param>
    /// <param name="errorMessage">Any error messages</param>
    /// <returns></returns>   
    public static bool VerifyBranchServerRequest(ILogging log,
      SourceRequest request, out ASS_BranchServer_Cached server, out string errorMessage)
    {
      server = null;
      errorMessage = null;     
      try
      {
        #region Check basics
        if (!CheckBasics(request, out errorMessage))
        {
          return false;
        }

        if (string.IsNullOrEmpty(request.BranchCode))
        {
          errorMessage = "Branch code cannot be empty";
          return false;
        }
        var legacyBranchNum = request.BranchCode.PadLeft(3, '0');
        #endregion

        #region Check IP addresses
        var ipAddresses = request.MachineIPAddresses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        foreach (var ipAddress in ipAddresses)
        {
          IPAddress address;
          if (!IPAddress.TryParse(ipAddress, out address))
          {
            errorMessage = string.Format("Invalid IP address: {0}", ipAddress);
            return false;
          }
        }
        ipAddresses.Sort();
        var sortedIPAddresses = string.Join(",", ipAddresses);
        #endregion

        // Admin system- do not process any other server checks
        if (request.BranchCode == "000")
        {
          return true;
        }
               
        var thisServer = DataCache.GetBranchServerViaMachineDetails(request.MachineName, request.MachineUniqueID, request.MachineIPAddresses);
        if (thisServer == null)
        {
          errorMessage = $"Unable to locate server: {request.MachineName} - {request.MachineUniqueID} - {request.MachineIPAddresses}";
          return false;
        }
      
        var branchServer = DataCache.GetBranchServerViaBranchNum(legacyBranchNum);
        if (branchServer == null)
        {
          errorMessage = "No server allocated to branch";
          return false;
        }
               
        // Ensure this branch is not currently serviced by another server- old server must be de-commissioned (clear ASS_BranchSever.Branch) first
        if (branchServer.BranchServerId != thisServer.BranchServerId)
        {
          errorMessage = string.Format("Branch '{0}' is currently served by another machine", legacyBranchNum);
          return false;
        }

        if (branchServer.Branch != thisServer.Branch)
        {
          errorMessage = string.Format("Server branch mismatch. Branch {0} is serviced by machine {1}, while this machine '{2}' is linked to branch {3}",
            branchServer.Branch, branchServer.Machine, thisServer.Machine, thisServer.Branch);

          return false;
        }

        if (!branchServer.MachineAuthorised)
        {
          errorMessage = "This Machine/server is not authorised to use Atlas data sync services";
          return false;
        }

        server = branchServer;
        return true;        
      }
      catch (Exception err)
      {
        errorMessage = $"CheckBasics: {err.Message}";
        return false;
      }
    }

    #endregion


    #region Private methods

    /// <summary>
    /// Performs generic, basic validation of the source request parameters
    /// </summary>
    /// <param name="request"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private static bool CheckBasics(SourceRequest request, out string errorMessage)
    {
      errorMessage = null;
      if (request == null)
      {
        errorMessage = "'Request' cannot be empty";
        return false;
      }

      if (string.IsNullOrWhiteSpace(request.AppName))
      {
        errorMessage = "'AppName' cannot be empty";
        return false;
      }

      if (string.IsNullOrWhiteSpace(request.AppVer))
      {
        errorMessage = "'AppVer' cannot be empty";
        return false;
      }

      if (request.MachineDateTime == DateTime.MinValue)
      {
        errorMessage = "'MachineDateTime' cannot be empty";
        return false;
      }

      if (string.IsNullOrWhiteSpace(request.MachineIPAddresses))
      {
        errorMessage = "'MachineIPAddresses' cannot be empty";
        return false;
      }

      if (string.IsNullOrWhiteSpace(request.MachineName))
      {
        errorMessage = "'MachineName' cannot be empty";
        return false;
      }

      if (string.IsNullOrWhiteSpace(request.MachineUniqueID))
      {
        errorMessage = "'MachineUniqueID' cannot be empty";
        return false;
      }

      var minutesOut = (int)Math.Abs(request.MachineDateTime.Subtract(DateTime.Now).TotalMinutes);
      if (minutesOut > 10)
      {
        errorMessage = string.Format("Invalid system time- your system is {0} minutes out- server time: {1:f}", minutesOut, DateTime.Now);
        return false;
      }

      return true;
    }

    #endregion

  }
}