/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2016 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Main entry- Scheduler Server
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2016-03-22 Keith Blows
 *          Modified for SimpleInjector
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using AutoMapper;
using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Servers.Common.Logging;
using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Xpo;
using SchedulerServer.QuartzTasks;


namespace SchedulerServer
{
  class Program
  {
    static void Main()
    {
      //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
      // DI
      RegisterDepedencies();

      // XPO
      XpoUtils.CreateXpoDomain(_config, _log);

      #region AutoMapper
      Atlas.Domain.DomainMapper.Map();

      Mapper.CreateMap<AEDOReportBatch, AEDOReportBatchDTO>();
      Mapper.CreateMap<AEDOReportCancelled, AEDOReportCancelledDTO>();
      Mapper.CreateMap<AEDOReportFailed, AEDOReportFailedDTO>();
      Mapper.CreateMap<AEDOReportFuture, AEDOReportFutureDTO>();
      Mapper.CreateMap<AEDOReportNewTransaction, AEDOReportNewTransactionDTO>();
      Mapper.CreateMap<AEDOReportRetry, AEDOReportRetryDTO>();
      Mapper.CreateMap<AEDOReportSettled, AEDOReportSettledDTO>();
      Mapper.CreateMap<AEDOReportSuccess, AEDOReportSuccessDTO>();
      Mapper.CreateMap<AEDOReportUnmatched, AEDOReportUnmatchedDTO>();
      Mapper.CreateMap<AEDOReportUnsettled, AEDOReportUnsettledDTO>();

      Mapper.CreateMap<NAEDOReportBatch, NAEDOReportBatchDTO>();
      Mapper.CreateMap<NAEDOReportCancelled, NAEDOReportCancelledDTO>();
      Mapper.CreateMap<NAEDOReportDisputed, NAEDOReportDisputedDTO>();
      Mapper.CreateMap<NAEDOReportFailed, NAEDOReportFailedDTO>();
      Mapper.CreateMap<NAEDOReportFuture, NAEDOReportFutureDTO>();
      Mapper.CreateMap<NAEDOReportInProcess, NAEDOReportInProcessDTO>();
      Mapper.CreateMap<NAEDOReportSuccess, NAEDOReportSuccessDTO>();
      Mapper.CreateMap<NAEDOReportTransactionUploaded, NAEDOReportTransactionUploadedDTO>();

      _log.Information("Domain Mapped");

      #endregion

      #region Topshelf service hosting
      HostFactory.Run(hc =>
      {
        // DI config
        hc.UseSerilog();
        hc.UseSimpleInjector(_container);
        hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

        hc.RunAsLocalSystem();
        hc.StartAutomaticallyDelayed();

        hc.SetServiceName("Atlas_Scheduler");
        hc.SetDisplayName("Atlas Scheduler");
        hc.SetDescription("Atlas scheduling server for running Quartz tasks");

        hc.Service<MainService>(sc =>
        {
          sc.ConstructUsingSimpleInjector();

          sc.WhenStarted((service, control) => service.Start());
          sc.WhenStopped((service, control) => service.Stop());

          #region Quartz tasks
          //AEDO Reports Import
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<AltechAEDOStoreReports>("0 1 06,12,14,16 ? * MON-SAT", "AEDOReportsImport"));
          
          // AEDO Future Report Import
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<AltechAEDOStoreReports_Future>("0 0 05,17 ? * MON-SAT", "FetchAEDOFutureReportImport"));
          
          // NAEDO & TSP Reports Import
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<AltechNAEDOTSPStoreReports>("15 5 06,12,14,16 ? * MON-SAT", "FetchNAEDOTSPReportsAndImport"));

          // NAEDO Future Report Import
          sc.ScheduleQuartzJob(config => config.WithCronSchedule<AltechNAEDOTSPStoreReports_Future>("0 0 05,17 ? * MON-SAT", "FetchNAEDOTSPFutureReportAndImport"));
          #endregion
        });        
      });
      #endregion
    }


    private static void RegisterDepedencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      // TODO: Create a config server and use to get connection/?WCF?/?app? settings
      _container.RegisterSingleton(_config);
    }
    

    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.Scheduler.Server");
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();


  }
}