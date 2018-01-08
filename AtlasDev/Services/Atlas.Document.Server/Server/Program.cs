using System.Net;
using System;

using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;
using AutoMapper;
using GdPicture12;

using Atlas.DocServer.WCF.Interface;
using Atlas.Servers.Common.Logging;
using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;
using Atlas.DocServer.WCF.DI;
using Atlas.Domain.Model;


namespace Atlas.DocServer
{
  class Program
  {
    static void Main()
    {
      try
      {
        #region Register GDPicture licenses
        var license = new LicenseManager();
        if (!license.RegisterKEY("13293665993739579111911142381496679811")) //132965496977096821115122879353532"))
        {
          throw new Exception("Failed to register key for GdPicture.NET Document Imaging SDK V12");
        }

        if (!license.RegisterKEY("72631698992742977151914149793665937433")) //718318999959219831319163658204273"))
        {
          throw new Exception("Failed to register key for GdPicture.NET Managed PDF Plugin V12");
        }
        #endregion

        // DI
        RegisterDependencies();

        // XPO
        Servers.Common.Xpo.XpoUtils.CreateXpoDomain(_config, _log, new[] { typeof(DOC_TemplateStore) });

        #region Automapper
        Atlas.Domain.DomainMapper.Map();

        // WCF mapping
        Mapper.CreateMap<GeneratorEnums.Generators, Enumerators.Document.Generator>();
        Mapper.CreateMap<DocCategoryEnums.Categories, Enumerators.Document.Category>();
        Mapper.CreateMap<FileFormatEnums.FormatType, Enumerators.Document.FileFormat>();
        Mapper.CreateMap<TemplateEnums.TemplateTypes, Enumerators.Document.DocumentTemplate>();
        Mapper.CreateMap<LanguageEnums.Language, Enumerators.General.Language>();

        Mapper.CreateMap<DocTemplate, DOC_TemplateStore>();
        Mapper.CreateMap<DOC_TemplateStore, DocTemplate>()
          .ForMember(dest => dest.TemplateType, opts => opts.MapFrom(src => src.TemplateType.Type))
          .ForMember(dest => dest.Category, opts => opts.MapFrom(src => src.TemplateType.Category.Type))
          .ForMember(dest => dest.Language, opts => opts.MapFrom(src => src.Language.Type));

        Mapper.CreateMap<DOC_FileStore, StorageInfo>()
          .ForMember(dest => dest.Category, opts => opts.MapFrom(src => src.Category != null ? src.Category.Type : Enumerators.Document.Category.NotSet))
          .ForMember(dest => dest.FileFormatType, opts => opts.MapFrom(src => src.FileFormatType.Type))
          .ForMember(dest => dest.SourceTemplateId, opts => opts.MapFrom(src => src.SourceTemplate != null ? src.SourceTemplate.TemplateId : 0))
          .ForMember(dest => dest.SourceDocumentId, opts => opts.MapFrom(src => src.SourceDocument != null ? src.SourceDocument.StorageId : 0));
        #endregion

        // 2 is default- Gets or sets the maximum number of concurrent connections allowed by a 
        // ServicePoint (connection management for HTTP connections) object.
        // Set the maximum number of ServicePoint instances to maintain. If a ServicePoint instance 
        // for that host already exists when your application requests a connection to an Internet 
        // resource, the ServicePointManager object returns this existing ServicePoint instance. 
        // If none exists for that host, it creates a new ServicePoint instance.
        ServicePointManager.DefaultConnectionLimit = 100;

        #region Topshelf service hosting
        HostFactory.Run(hc =>
        {
          // Config DI
          hc.UseSerilog();
          hc.UseSimpleInjector(_container);
          hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

          hc.RunAsLocalSystem();
          hc.StartAutomatically();
          hc.SetServiceName("Atlas_DocServer_V1");
          hc.SetDisplayName("Atlas Document Server");
          hc.SetDescription("Atlas Document Server. This service provides document generation, recognition & storage facilities. " +
                            "If this service is stopped, this functionality will not be available to Atlas clients.");

          hc.Service<MainService>(sc =>
          {
            sc.ConstructUsingSimpleInjector();

            sc.WhenStarted((service, control) => service.Start());
            sc.WhenStopped((service, control) => service.Stop());

            var random = new Random();
            sc.ScheduleQuartzJob(config => config.WithCronSchedule<QuartzTasks.DeleteOldChunkedFiles>(
              string.Format("0 0 3 1/1 * ? *", random.Next(60), random.Next(60)), "DeleteOldChunkedFiles"));
          });

        });
        #endregion
      }
      catch (Exception err)
      {
        Console.WriteLine("{0} @\r\n{1}", err.Message, err.StackTrace);
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

      // WCF
      // ---------------------------------------
      _container.Register<IDocumentAdminServiceHost>(() => new DocumentAdminServiceHost(_container));
      _container.Register<IDocumentConvertServiceHost>(() => new DocumentConvertServiceHost(_container));
      _container.Register<IDocumentGeneratorServiceHost>(() => new DocumentGeneratorServiceHost(_container));
      _container.Register<IDocumentRecognitionServiceHost>(() => new DocumentRecognitionServiceHost(_container));

    }


    // *Cross-cutting concerns*-  we need instances up front, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.ASS.Server", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();

  }
}
