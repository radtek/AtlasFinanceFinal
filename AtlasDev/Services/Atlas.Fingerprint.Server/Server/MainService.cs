/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Service events
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-11-13- Created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using Atlas.WCF.FPServer.Comms;
using Atlas.Common.Interface;
using Atlas.Servers.Common.WCF;
using Atlas.WCF.FPServer.WCF.DI;


namespace Atlas.WCF.FPServer
{
  public class MainService
  { 
    public MainService(ILogging log, IFPCoreServiceHost serviceHost_FPCore, IFPCommsServiceHost serviceHost_FPComms)
    {
      _log = log;
      _serviceHost_FPCore = serviceHost_FPCore;
      _serviceHost_FPComms = serviceHost_FPComms;    
    }


    /// <summary>
    /// Service starting event
    /// </summary>
    public bool Start()
    {
      try
      {
        _log.Information("FP Service starting up...");

        #region Test
        //long branchId;
        //WCF.Implementation.Machine machine;
        //WCF.Implementation.User user;
        //string errmsg;
        //WCF.Implementation.WCFUtils.CheckSourceRequest(_log, new Security.Interface.SourceRequest
        //{
        //  BranchCode = null,
        //  MachineDateTime = DateTime.Now,
        //  MachineIPAddresses = "15.15.15.16,192.168.34.3",
        //  MachineName = "061-00-02",
        //  MachineUniqueID = "56ebb6ad68ffbd19a8df921a824427ff",
        //  AppName = "Atlas Core Client GUI",
        //  AppVer = "1.2.0.26",
        //  PersonId = 250266,
        //  UserPersonId = 105541
        //}, out branchId, out machine, out user, out errmsg);
        //if (machine == null)
        //{

        //}
        #endregion
              
        _log.Information("Connecting to message bus...");
        DistCommUtils.Start(_log);
        
        #region Start WCF services
        _log.Information("Starting WCF services...");

        // Core FP service- enrollment/verification/identification services
        try
        {
          _serviceHost_FPCore.Open();
          _serviceHost_FPCore.LogEndpoints(_log);
          _log.Information("Successfully loaded core Fingerprint WCF service");
        }
        catch (Exception err)
        {
          _log.Fatal(err, "Failed to load core Fingerprint service");
        }

        // Client/LMS- provide communications between LMS (web/xHarbour) and local WinForms GUI/hardware manager
        try
        {
          _serviceHost_FPComms.Open();
          _serviceHost_FPComms.LogEndpoints(_log);
          _log.Information("Successfully loaded Fingerprint Communications WCF service");
        }
        catch (Exception err)
        {
          _log.Fatal(err, "Failed to load Fingerprint Communications service");
        }

        _log.Information("WCF services started");
        #endregion
                
        _log.Information("Starting fingerprint FP socket server...");
        FPSocketServer.FpTcpServer.StartServer(_log);
             
        _log.Information("FP Service start completed");

        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "Start()");
        return false;
      }     
    }


    /// <summary>
    /// Service stopping event
    /// </summary>
    public bool Stop()
    {
      try
      {
        #region Close WCF services
        _log.Information("Closing FP WCF services...");

        if (_serviceHost_FPCore != null)
        {
          _serviceHost_FPCore.Close();          
          _log.Information("Fingerprint Core service closed");
        }

        if (_serviceHost_FPComms != null)
        {
          _serviceHost_FPComms.Close();          
          _log.Information("Fingerprint Communications service closed");
        }

        _log.Information("All FP WCF services successfully stopped");
        #endregion

        _log.Information("Stopping message bus...");
        DistCommUtils.Stop();
        _log.Information("RabbitMQ bus closed");

        _log.Information("All FP services were successfully stopped");
      }
      catch (Exception err)
      {
        _log.Error(err, "Stop()");
      }

      return true;
    }


    #region Private vars
    
    /// <summary>
    /// FP Core Services- Identification/Verification/etc
    /// </summary>
    private readonly IBaseServiceHost _serviceHost_FPCore;

    /// <summary>
    /// FP Comms Services- Comms gateway between Atlas Client UI and others
    /// </summary>
    private readonly IBaseServiceHost _serviceHost_FPComms;

    /// <summary>
    /// Logging
    /// </summary>
    private readonly ILogging _log;
    
    #endregion
           
  }
}
