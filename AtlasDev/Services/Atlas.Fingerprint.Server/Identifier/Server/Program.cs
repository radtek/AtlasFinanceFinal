using SimpleInjector;
using Topshelf;
using Topshelf.SimpleInjector;
using Topshelf.SimpleInjector.Quartz;

using Atlas.Common.Interface;
using Atlas.Servers.Common.Config;
using Atlas.Servers.Common.Logging;


namespace Atlas.FP.Identifier
{
  class Program
  {
    static void Main()
    {
      _log.Information("Starting...");
      // DI
      RegisterDepedencies();
         
      #region TopShelf service hosting
      HostFactory.Run(hc =>
      {
        // Config DI
        hc.UseSerilog();
        hc.UseSimpleInjector(_container);
        hc.UseQuartzSimpleInjector(_container); // can't specify delayed start?

        hc.RunAsLocalSystem();
        hc.StartAutomaticallyDelayed();

        hc.SetServiceName("Atlas_FP_Identifier");
        hc.SetDisplayName("Atlas Fingerprint Identifier");
        hc.SetDescription("Atlas Fingerprint Identifier. This service provides scalable, distributed fingerprint identification services via RabbitMQ Request/Response and Redis PubSub channel");
        
        hc.Service<MainService>(sc =>
        {
          sc.ConstructUsingSimpleInjector();
          sc.WhenStarted((service, control) => service.Start(control));
          sc.WhenStopped((service, control) => service.Stop());
        });
      });
      #endregion
    }


    /// <summary>
    /// DI registration
    /// </summary>
    private static void RegisterDepedencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);
      // TODO: Create a config server and use to get connection/?WCF?/?app? settings
      _container.RegisterSingleton(_config);     
    }


    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.FP.Identifier", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();

  }
}
