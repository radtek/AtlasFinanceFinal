/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Main application- streaming WCF service
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     24 May 2013 - Created
 *     6 April 2015 - Improved DI
 *     
 * 
 *  Comments:
 *  ------------------
 *  
 *     
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Logging;
using Atlas.Common.Interface;
using Atlas.Servers.Common.Xpo;
using ASSServer.QuartzTasks;
using ASSServer.WCF.DI;
using Atlas.Cache.Redis;
using Atlas.Cache.Interfaces;
using DevExpress.Xpo;
using System.Linq;

namespace ASSServer
{
  class Program
  {
    /// <summary>
    /// Main entry point
    /// </summary>
    static void Main()
    {
      try
      {
        // DI
        RegisterDependencies();
        _log.Information("Starting");

        #region Testing
        //Cache.DataCache.Init(_cache, _log);
        //var ver = Atlas.Cache.DataUtils.CacheUtils.GetCurrentDbVersion(_cache).DbUpdateScriptId;
        //
        //Atlas.Cache.Interfaces.Classes.ASS_BranchServer_Cached server;
        //string error;
        ////for (int i = 0; i < 5; i++)
        //{
        //  var branchServer = Atlas.Cache.DataUtils.CacheUtils.GetBranchServerViaBranchNum(_cache, "001");
        //  _log.Information("Branch: {@Server}", branchServer);

        //  var machine = Cache.DataCache.GetBranchServerViaMachineDetails("0L7-00-04", "7936a113d8e1c8b899860161f05e60fe", "192.168.189.5");
        //  _log.Information("Machine: {@Machine}", machine);
        //  var request = new Atlas.DataSync.WCF.Interface.SourceRequest
        //  {
        //    AppName = "Atlas Branch DataSync Server",
        //    AppVer = "1.2.0.5",
        //    BranchCode = "71",
        //    MachineName = "071-00-01",
        //    MachineUniqueID = "ba37ee27f2a63eb4975c08360dc8dea7",
        //    MachineDateTime = DateTime.Now,
        //    MachineIPAddresses = "15.15.15.16,192.168.40.2"
        //  };
        // var ok = WCF.Implementation.Checks.VerifyBranchServerRequest(_log, request, out server, out error);
        //  WCF.Implementation.DataSync.UploadCurrentBranchRecId_Impl.Execute(_log, _cache, _config, request, 0);
        //  WCF.Implementation.DataSync.UploadCurrentBranchRecId_Impl.Execute(_log, _cache, _config, request, 1);
        //  System.Threading.Thread.Sleep(100);
        //  _log.Information("Verify: {OK} {@Server}- {Error}", ok, server, error);
        //  var branch = Cache.DataCache.GetBranchServerViaBranchNum("L7");
        //  _log.Information("Branch: {Branch}", branch);
        //}
        //Console.WriteLine("Press a key...");
        //Console.ReadKey();
        //var machine = _cache.Get<Atlas.Cache.Interfaces.Classes.COR_Machine_Cached>(279);
        //machine.MachineIPAddresses = "15.15.15.16,192.168.40.2";
        //machine.HardwareKey = "ba37ee27f2a63eb4975c08360dc8dea7";
        //machine.MachineName = "071-00-01";
        //_cache.Set(new[] { machine });
        //_cache.Delete<Atlas.Cache.Interfaces.Classes.COR_Machine_Cached>(5495);
        //return;
        #endregion

        // XPO
        XpoUtils.CreateXpoDomain(_config, _log);
            
        // Mapping config
        Atlas.Domain.DomainMapper.Map();

        #region Topshelf service hosting
        HostFactory.Run(hc =>
        {
          // Config DI
          hc.UseSerilog();
          hc.UseSimpleInjector(_container);
          hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

          hc.SetServiceName("ASS_Data_Server");
          hc.SetDisplayName("Atlas Legacy LMS Data Sync Server");
          hc.SetDescription("Atlas Legacy LMS Data Sync Server. This service exposes Atlas 'Ass' branch data, via " +
                  ".NET binary WCF services for data synchronization. If this service is stopped, this functionality will " +
                  "not be available to Atlas clients.");

          hc.Service<MainService>(sc =>
          {
            sc.ConstructUsingSimpleInjector();
            sc.WhenStarted((service, control) => service.Start());
            sc.WhenStopped((service, control) => service.Stop());

            // Branches not syncing- e-mail alert  // Weekdays- 08:30 to 17:00 - some branches close on the week-ends, don't bother us....      
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<AssBranchDataSyncIssues>("0 30 8-17/2 ? * MON-FRI *", "Alerting"));

            // Checks for replication 'holes' between server and branches- not implemented
            //sc.ScheduleQuartzJob(config => config.WithCronSchedule<CheckReplication>("0 23 22 ? * MON-SUN *", "CheckReplication"));

            // Check WAL streaming is working to spec every 15 minutes
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<CheckWALReplication>("0 0/15 5-23 ? * MON-SAT *", "CheckWALReplication"));

            // Delete old files
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<CleanUps>("0 15 23 ? * MON-SAT *", "CleanUps"));

            //sc.ScheduleQuartzJob(config => config.WithCronSchedule<CopyMasterToBrXXX>("0 15 23 ? * MON-SUN *", "CopyMasterToBrXXX"));

            // Delete old NLR >3 months @ 19:00 Mon-Sat
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<DeleteOldAssNlrRows>("0 0 19 ? * MON-SAT *", "DeleteAssNlrRows"));

            // Daily DBF downloadable links
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<ExportDBFLinksDaily>("0 0 3 ? * MON-SUN *", "ExportDBFLinksDaily"));

            // Copy master tables from company schema to all brXXX schemas
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<AssCopyMasterDataToBrSchemas>("0 0 5 ? * MON-SUN *", "AssCopyMasterDataToBrSchemas"));
          });
        });
        #endregion
      }
      catch (Exception err)
      {
        Console.WriteLine("Start-up error: '{0}'", err.Message);
        return;
      }
    }


    /// <summary>
    /// DI registration
    /// </summary>
    private static void RegisterDependencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      _container.RegisterSingleton(_config);
      _container.RegisterSingleton(_cache);

      // WCF
      // ---------------------------------------
      _container.Register<IDataSyncServiceHost>(() => new DataSyncServiceHost(_container));
      _container.Register<IDataFileServiceHost>(() => new DataFileServiceHost(_container));
      _container.Register<IAdminServiceHost>(() => new AdminServiceHost(_container));
      _container.Register<IDataRequestServiceHost>(() => new DataRequestServiceHost(_container));
    }


    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.Data.ASSServer", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    private static readonly ICacheServer _cache = new RedisCacheServer(_config, _log);

    // DI
    private static readonly Container _container = new Container();
  }
}
