/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2016 Atlas Finance (Pty() Ltd.
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
 *
 *
 *
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Diagnostics;

using Atlas.Common.Interface;
using Atlas.Server.WCF_DI;
using Atlas.Servers.Common.WCF;
using Atlas.Server.MessageBus.Notification;
using Atlas.Server.MessageBus.Avs;


namespace Atlas.Server
{
  public class MainService
  {
    public MainService(ILogging log, IConfigSettings config,
      ITCCTerminalServiceHost tccServiceHost, IASSServiceHost assServiceHost,
      IAssCdvServiceHost assCdvServiceHost, IAssAvsServiceHost assAvsServiceHost, 
      IThirdPartyServiceHost thirdPartyServiceHost)
    {
      _config = config;
      _serviceHostTcc = tccServiceHost;
      _serviceHostAss = assServiceHost;
      _serviceHostAssCdv = assCdvServiceHost;
      _serviceHostAssAvs = assAvsServiceHost;
      _serviceHostThirdParty = thirdPartyServiceHost;

      _log = log;
      _log.Information("Service is starting");
    }


    public bool Start()
    {
      #region Start WCF services
      // TCC control
      try
      {
        _serviceHostTcc.Open();
        _serviceHostTcc.LogEndpoints(_log);
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Failed to load 'NPTerminalRC' service");
      }

      // Basic ASS services- PSQL connection strings, SMS, OTP
      try
      {
        _serviceHostAss.Open();
        _serviceHostAss.LogEndpoints(_log);
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Failed to load 'ASS' service");
      }

      // CDV 
      try
      {
        _serviceHostAssCdv.Open();
        _serviceHostAssCdv.LogEndpoints(_log);
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Failed to load 'CDV' service");
      }

      // AVS
      try
      {
        _serviceHostAssAvs.Open();
        _serviceHostAssAvs.LogEndpoints(_log);
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Failed to load 'BankVerification' service");
      }

      // Third party integrations- '1111' over-ride
      try
      {
        _serviceHostThirdParty.Open();
        _serviceHostThirdParty.LogEndpoints(_log);
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Failed to load 'AssThirdParty' service");
      }
      #endregion

      #region Message bus
      try
      {
        _log.Information("Starting EasyNetQ bus...");
        // AVS message handler
        AvsDistCommUtils.Start(_log, _config);

        // Notification server handler (SMS, e-mail)
        NotificationDistCommUtils.Start(_log, _config);
      }
      catch (Exception err)
      {
        _log.Error(err, "Start");
      }
      #endregion

      _log.Information("Service start completed");

      return true;
    }


    public bool Stop()
    {
      #region Close WCF services
      _log.Information("Closing WCF services...");

      if (_serviceHostTcc != null)
      {
        _serviceHostTcc.Close();
        _serviceHostTcc = null;
        _log.Information("TCC service closed");
      }

      if (_serviceHostAss != null)
      {
        _serviceHostAss.Close();
        _serviceHostAss = null;
        _log.Information("ASS service closed");
      }

      if (_serviceHostAssAvs != null)
      {
        _serviceHostAssAvs.Close();
        _serviceHostAssAvs = null;
        _log.Information("AVS service closed");
      }

      if (_serviceHostAssCdv != null)
      {
        _serviceHostAssCdv.Close();
        _serviceHostAssCdv = null;
        _log.Information("BankVerification proxyService Closed");
      }

      if (_serviceHostThirdParty != null)
      {
        _serviceHostThirdParty.Close();
        _serviceHostThirdParty = null;
        _log.Information("AssThirdParty service closed");
      }
      //_log.Information("WCF services successfully stopped");

      #endregion Close WCF services

      AvsDistCommUtils.Stop();
      NotificationDistCommUtils.Stop();

      _log.Information("Service was stopped");

      return true;
    }


    #region Private vars

    private readonly static Stopwatch _upTime = Stopwatch.StartNew();
    
    // Injected  
    private readonly ILogging _log;
    private  IBaseServiceHost _serviceHostTcc;
    private  IBaseServiceHost _serviceHostAss;
    private  IBaseServiceHost _serviceHostAssAvs;
    private  IBaseServiceHost _serviceHostAssCdv;
    private  IBaseServiceHost _serviceHostThirdParty;
    private readonly IConfigSettings _config;

    #endregion Private vars

  }

}