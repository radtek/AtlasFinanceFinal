using System;
using System.Net;
using System.ServiceModel;
using System.Configuration;

using Serilog;
using Quartz;
using Quartz.Impl;
using Topshelf;

using Atlas.Server.Training.QuartzTasks;


namespace Atlas.Server.Training
{
  internal class MainService : IDisposable
  {
    public bool Start(HostControl hostControl)
    {
      try
      {
        #region WCF services

        #region Start mock TCC admin web service
        try
        {
          _tccServer = new ServiceHost(typeof(TermRCSoap_Impl));
          _tccServer.Open();
          _log.Information("Successfully loaded 'TermRCSoap' service");
          EnumerateEndpointsActive(_tccServer.Description);
        }
        catch (Exception err)
        {
          _log.Error(err, "Start");
          _tccServer = null;
        }
        #endregion

        #region Start mock WCF Altech AEDO admin web service
        try
        {
          _aedoAdminServer = new ServiceHost(typeof(AEDOAdmin_Impl));
          _aedoAdminServer.Open();
          _log.Information("Successfully loaded 'AEDOAdmin' service");
          EnumerateEndpointsActive(_aedoAdminServer.Description);
        }
        catch (Exception err)
        {
          _log.Error(err, "Start");
          _aedoAdminServer = null;
        }
        #endregion

        #region Start mock Altech NAEDO admin web service
        try
        {
          _naedoAdminServer = new ServiceHost(typeof(NAEDOAdmin_Impl));
          _naedoAdminServer.Open();
          _log.Information("Successfully loaded 'NAEDOAdmin' service");
          EnumerateEndpointsActive(_naedoAdminServer.Description);
        }
        catch (Exception err)
        {
          _log.Error(err, "Start");
          _naedoAdminServer = null;
        }
        #endregion

        #region Start mock Atlas bank verification web service
        try
        {
          _bankVerificationServer = new ServiceHost(typeof(BankVerification_Impl));
          _bankVerificationServer.Open();
          _log.Information("Successfully loaded 'VerificationServer' service");
          EnumerateEndpointsActive(_bankVerificationServer.Description);
        }
        catch (Exception err)
        {
          _log.Error(err, "Start");
          _bankVerificationServer = null;
        }
        #endregion

        #region Start mock FP comms
        try
        {
          _fpCommsServer = new ServiceHost(typeof(FPComms));
          _fpCommsServer.Open();
          _log.Information("Successfully loaded 'FPComms' service");
          EnumerateEndpointsActive(_fpCommsServer.Description);
        }
        catch (Exception err)
        {
          _log.Error(err, "Start");
          _fpCommsServer = null;
        }
        #endregion

        #region Start mock V2 scorecard server
        try
        {
          _scorecardServer = new ServiceHost(typeof(ScorecardServer));
          _scorecardServer.Open();
          _log.Information("Successfully loaded 'ScorecardServer' service");
          EnumerateEndpointsActive(_scorecardServer.Description);
        }
        catch (Exception err)
        {
          _log.Error(err, "Start");
          _scorecardServer = null;
        }
        #endregion

        //#region Start mock ID Photo server        
        //try
        //{
        //  _idPhotoServer = new ServiceHost(typeof(CSIDPhotoServer));
        //  _idPhotoServer.Open();
        //  _log.Information("Successfully loaded 'IdPhotoServer' service");
        //  EnumerateEndpointsActive(_idPhotoServer.Description);
        //}
        //catch (Exception err)
        //{
        //  _log.Error(err, "Start");
        //  _idPhotoServer = null;
        //}
        //#endregion

        #endregion

        #region Start mock XML-RPC NuCard server
        _xmlRpcServer = new System.Threading.Thread(() =>
          {
            var log = Log.ForContext<System.Threading.Thread>();
            log.Information("Server thread starting");
            try
            {
              // netsh http add urlacl url=http://127.0.0.1:8000/ user="Local Service"
              // netsh http add urlacl url=http://+:4567/ user="NT AUTHORITY\Local Service"
              var listener = new HttpListener();
              listener.Prefixes.Add(ConfigurationManager.AppSettings["XMLRPCAddress"] ?? "http://127.0.0.1:9000/");
              listener.Start();
              log.Information("NuCard XML-RPC listening on: {@XMLRPC}", listener.Prefixes);
              while (true)
              {
                // Sync HTTP server
                var context = listener.GetContext();
                log.Information("Processing request: {@Headers}", context.Request.Headers);
                CookComputing.XmlRpc.XmlRpcListenerService svc = new Atlas.Server.Training.XMLRPCServer.NuCardService();
                svc.ProcessRequest(context);
                log.Information("Processed request {@StatusCode}", context.Response.StatusCode);
              }
            }
            catch (Exception err)
            {
              log.Error(err, "Thread");
            }
          });

        _xmlRpcServer.IsBackground = true; // !!!!!!
        _xmlRpcServer.Start();
        #endregion

        #region Quartz

        #region Quartz Scheduler engine config
        _log.Information("Starting scheduler engine");
        // Construct standard scheduler factory
        _schedFact = new StdSchedulerFactory(/*props*/);

        // Get a scheduler
        var sched = _schedFact.GetScheduler();
        #endregion

        // Dbf generation starts at 20:00 and takes 90mins
        #region Stop all local Atlas services at 20:00
        var stopAtlasServicesTask = JobBuilder.Create<StopAtlasServices>().WithIdentity("StopAtlasServices", "General").Build();
        var stopAtlasServicesSched = (ISimpleTrigger)TriggerBuilder.Create()
              .WithIdentity("StopAtlasDaily", "General")
              .WithDescription("At 20:00")
              .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInHours(24).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
              .StartAt(DateBuilder.TodayAt(20, 0, 0))
              .Build();
        sched.ScheduleJob(stopAtlasServicesTask, stopAtlasServicesSched);
        _log.Information("Stop Atlas Services- next fire: {NextScheduleUTC}", stopAtlasServicesSched.GetNextFireTimeUtc());
        #endregion

        #region 'atlas_core' database copy @ 20:05
        var copyAtlasCore = JobBuilder.Create<CopyAtlasCore>().WithIdentity("CopyAtlasCoreData", "General").Build();
        var copyAtlasCoreSched = (ISimpleTrigger)TriggerBuilder.Create()
              .WithIdentity("CopyAtlasCoreOnceADay", "General")
              .WithDescription("At 20:05")
              .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInHours(24).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
              .StartAt(DateBuilder.TodayAt(20, 05, 0))
              .Build();
        sched.ScheduleJob(copyAtlasCore, copyAtlasCoreSched);
        _log.Information("Copy atlas core data- next fire: {NextScheduleUTC}", copyAtlasCoreSched.GetNextFireTimeUtc());
        #endregion

        #region brXXX 'ass' database copy @ 21:00 (no company)
        var copyBranchData = JobBuilder.Create<CopyBranchData>().WithIdentity("CopyBranchData", "General").Build();
        var copyBranchDataSched = (ISimpleTrigger)TriggerBuilder.Create()
              .WithIdentity("CopyAssDataOnceADay", "General")
              .WithDescription("At 21:00")
              .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInHours(24).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
              .StartAt(DateBuilder.TodayAt(21, 0, 0))
              .Build();
        sched.ScheduleJob(copyBranchData, copyBranchDataSched);
        _log.Information("Copy ASS branch data- next fire: {NextScheduleUTC}", copyBranchDataSched.GetNextFireTimeUtc());
        #endregion

        #region 'ass_ro' creation- Make read-only version of ass database @ 03:00
        var makeBranchROCopy = JobBuilder.Create<MakeBranchROCopy>().WithIdentity("MakeBranchROCopy", "General").Build();
        var makeBranchROCopySched = (ISimpleTrigger)TriggerBuilder.Create()
              .WithIdentity("MakeBranchROCopyOnceADay", "General")
              .WithDescription("At 03:00")
              .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInHours(24).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
              .StartAt(DateBuilder.TomorrowAt(3, 0, 0))
              .Build();
        sched.ScheduleJob(makeBranchROCopy, makeBranchROCopySched);
        _log.Information("Copy ASS read-only branch data- next fire: {NextScheduleUTC}", makeBranchROCopySched.GetNextFireTimeUtc());
        #endregion

        #region Start all Atlas services at 06:00
        var startAtlasServicesTask = JobBuilder.Create<StartAtlasServices>().WithIdentity("StartAtlasServices", "General").Build();
        var startAtlasServicesSched = (ISimpleTrigger)TriggerBuilder.Create()
              .WithIdentity("StartAtlasDaily", "General")
              .WithDescription("At 00:00")
              .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInHours(24).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
              .StartAt(DateBuilder.TomorrowAt(6, 0, 0))
              .Build();
        sched.ScheduleJob(startAtlasServicesTask, startAtlasServicesSched);
        _log.Information("Start Atlas Services- next fire: {NextScheduleUTC}", startAtlasServicesSched.GetNextFireTimeUtc());
        #endregion

        #region Schedule Mongo copy- ID Photos and clear and copy Celia's FPs for her testing at 05:15
        var copyTestFPsTask = JobBuilder.Create<CopyMongoData>().WithIdentity("CopyTestFPs", "General").Build();
        var copyTestFPsSched = (ISimpleTrigger)TriggerBuilder.Create()
              .WithIdentity("CopyTestFPsDaily", "General")
              .WithDescription("At 05:15")
              .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInHours(24).RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires())
              .StartAt(DateBuilder.TomorrowAt(5, 15, 0))
              .Build();
        sched.ScheduleJob(copyTestFPsTask, copyTestFPsSched);
        _log.Information("Start FP daily copy- next fire: {NextScheduleUTC}", copyTestFPsSched.GetNextFireTimeUtc());
        #endregion

        sched.Start();

        #endregion

        //#region Manual tasks

        //var copyNow = false;
        //if (bool.TryParse(ConfigurationManager.AppSettings["CopyAtlasCoreDbNow"] ?? "false", out copyNow) && copyNow)
        //{
        //  Task.Run(() =>
        //     {
        //       _log.Information("MANUAL App.config: Copy Atlas Core DB now...");
        //       var copy = new CopyAtlasCore();
        //       copy.Execute(null);
        //     });
        //}

        bool copyNow = false;
        if (bool.TryParse(ConfigurationManager.AppSettings["CopyAssDbNow"] ?? "false", out copyNow) && copyNow)
        {
        //  Task.Run(() =>
        //    {
              _log.Information("MANUAL App.config: Copy ASS branch DB now...");
              var copy = new CopyBranchData();
              copy.Execute(null);

              _log.Information("MANUAL App.config: Copy ASS read-only branch DB now...");
              var copyRO = new MakeBranchROCopy();
              copyRO.Execute(null);
        //    });
        }

        //var clearFPsNow = false;
        //if (bool.TryParse(ConfigurationManager.AppSettings["CopyMongoNow"] ?? "false", out clearFPsNow) && clearFPsNow)
        //{
        //  Task.Run(() =>
        //    {
        //      _log.Information("MANUAL App.config: Copy sample MongoDB data now...");
        //      var clearFPs = new CopyMongoData();
        //      clearFPs.Execute(null);
        //    });
        //}
        //#endregion

        return true;
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Start()");
        return false;
      }
    }


    public void Stop()
    {
      #region Quartz
      if (_schedFact != null)
      {
        var scheduler = _schedFact.GetScheduler();
        if (scheduler != null)
        {
          scheduler.Shutdown();
        }

        _schedFact = null;
      }
      #endregion

      #region WCF
      if (_tccServer != null)
      {
        _tccServer.Close();
        _tccServer = null;
      }

      if (_aedoAdminServer != null)
      {
        _aedoAdminServer.Close();
        _aedoAdminServer = null;
      }

      if (_naedoAdminServer != null)
      {
        _naedoAdminServer.Close();
        _naedoAdminServer = null;
      }

      if (_bankVerificationServer != null)
      {
        _bankVerificationServer.Close();
        _bankVerificationServer = null;
      }

      if (_fpCommsServer != null)
      {
        _fpCommsServer.Close();
        _fpCommsServer = null;
      }

      if (_scorecardServer != null)
      {
        _scorecardServer.Close();
        _scorecardServer = null;
      }

      if (_idPhotoServer != null)
      {
        _idPhotoServer.Close();
        _idPhotoServer = null;
      }
      #endregion
    }


    public void Dispose()
    {
      Stop();
    }

    #region Private fields

    /// <summary>
    /// TCC Server
    /// </summary>
    private ServiceHost _tccServer;

    /// <summary>
    /// AEDO services
    /// </summary>
    private ServiceHost _aedoAdminServer;

    /// <summary>
    /// NAEDO services
    /// </summary>
    private ServiceHost _naedoAdminServer;


    /// <summary>
    /// Bank verification services
    /// </summary>
    private ServiceHost _bankVerificationServer;

    /// <summary>
    /// FP comms services
    /// </summary>
    private ServiceHost _fpCommsServer;


    private ServiceHost _scorecardServer;


    private ServiceHost _idPhotoServer;

    /// <summary>
    /// XML-RPC NuCard server thread
    /// </summary>
    private System.Threading.Thread _xmlRpcServer;

    /// <summary>
    /// Logger
    /// </summary>
    private static readonly ILogger _log = Log.ForContext<MainService>();

    /// <summary>
    /// Quartz scheduler factory
    /// </summary>
    private ISchedulerFactory _schedFact;

    #endregion


    #region Private utility functions

    /// <summary>
    /// Enumerates details of WCF endpoint
    /// </summary>
    /// <param name="service"></param>
    private static void EnumerateEndpointsActive(System.ServiceModel.Description.ServiceDescription service)
    {
      _log.Information("WCF endpoints active: {EndpointCount}", service.Endpoints.Count);
      foreach (var endpoint in service.Endpoints)
      {
        _log.Information("WCF endpoint active- {WCFAddress}, {WCFContractName}, {WCFBindingName}",
            endpoint.Address, endpoint.Contract.Name, endpoint.Binding.Name);
      }
    }

    #endregion Private utility functions

  }
}
