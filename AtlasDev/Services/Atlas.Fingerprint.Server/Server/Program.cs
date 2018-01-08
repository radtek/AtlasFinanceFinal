/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Main application- Fingerprint service
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

using AutoMapper;
using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Logging;
using Atlas.Servers.Common.Xpo;
using Atlas.WCF.FPServer.Interface;
using Atlas.Domain.Model;
using Atlas.WCF.FPServer.QuartzTasks;
using Atlas.WCF.FPServer.WCF.DI;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Redis;

namespace Atlas.WCF.FPServer
{
  class Program
  {
    static void Main()
    {
      // DI
      RegisterDependencies();

      // XPO
      XpoUtils.CreateXpoDomain(_config, _log);

      // AutoMapper
      Domain.DomainMapper.Map();

      // WCF special config
      System.Net.ServicePointManager.DefaultConnectionLimit = 1000;

      #region Custom AutoMapper
      Domain.DomainMapper.Map();

      Mapper.CreateMap<PER_Person, BasicPersonDetailsDTO>()
        .ForMember(dest => dest.PersonType, opt => opt.MapFrom(src => src.PersonType.Type));

      Mapper.CreateMap<PER_Person, BasicPersonDetailsDTO>()
        .ForMember(dest => dest.LegacyOperatorId, options => options.MapFrom(source => source.Security != null ? source.Security.LegacyOperatorId : null))
        .ForMember(dest => dest.IDOrPassport, option => option.MapFrom(source => source.IdNum))
        .ForMember(dest => dest.SecurityId, option => option.MapFrom(source => source.Security != null ? source.Security.SecurityId : 0));

      Mapper.CreateMap<PER_Role, PersonRoleDTO>()
        .ForMember(dest => dest.Description, options => options.MapFrom(source => source.RoleType.Description))
        .ForMember(dest => dest.RoleTypeId, options => options.MapFrom(source => source.RoleType.RoleTypeId));
      Mapper.CreateMap<PER_Person, Atlas.WCF.FPServer.WCF.Implementation.User>();

      Mapper.CreateMap<FPRawBufferDTO, FPRawBufferDTO>();
      Mapper.CreateMap<FPTemplateDTO, FPTemplateDTO>();
      #endregion

      try
      {
        #region TopShelf service hosting
        HostFactory.Run(hc =>
        {
          // Config DI
          hc.UseSerilog();
          hc.UseSimpleInjector(_container);
          hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

          hc.RunAsLocalSystem();
          hc.StartAutomaticallyDelayed(); // We need MongoDB/SQL Server- give them time to start

          hc.SetServiceName("Atlas_FP_WCF_Server");
          hc.SetDisplayName("Atlas Fingerprint Server");
          hc.SetDescription("Atlas Finance Fingerprint Server. This service provides core Fingerprint services via " +
                  "HTTP SOAP and .NET binary WCF services. If this service is stopped, Atlas Fingerprint functionality will " +
                  "stop functioning. This service requires PostgreSQL and MongoDB Servers.");

          hc.Service<MainService>(sc =>
          {
            sc.ConstructUsingSimpleInjector();

            sc.WhenStarted((service, control) => service.Start());
            sc.WhenStopped((service, control) => service.Stop());

            // run continuously every 13 min  
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<ExpiredFPUploadSessions>("0 5/13 * ? * *", "ExpiredFPUploadSessions"));

            // Delete old sessions for GUI/LMS communications
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<ExpiredLmsGuiSessions>("0 0/30 * ? * *", "ExpiredComms"));

            // Delete old audits from COR_LogMachineInfo- Sunday at 02:00
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<DeleteOldMachineAudits>("0 0 2 ? * SUN *", "DeleteOldAudits"));
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


    private static void RegisterDependencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);     
      _container.RegisterSingleton(_config);
      _container.RegisterSingleton(_cache);

      // WCF
      // ---------------------------------------
      _container.Register<IFPCoreServiceHost>(() => new FPCoreServiceHost(_container));
      _container.Register<IFPCommsServiceHost>(() => new FPCommsServiceHost(_container));     
    }
    

    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.Fingerprint.Server", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();
    private static readonly ICacheServer _cache = new RedisCacheServer(_config, _log);

    // DI
    private static readonly Container _container = new Container();

  }
}