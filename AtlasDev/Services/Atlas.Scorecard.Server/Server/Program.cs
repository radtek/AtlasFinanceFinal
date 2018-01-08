using System;
using System.Net;

using Topshelf;
using SimpleInjector;
using Topshelf.SimpleInjector;

using Atlas.Servers.Common.Xpo;
using Atlas.Servers.Common.Logging;
using Atlas.Servers.Common.Config;
using Atlas.Common.Interface;
using Atlas.ThirdParty.CS.Bureau.WCF.WCF_DI;


namespace Atlas.ThirdParty.CS.Bureau
{
  class Program
  {
    static void Main()
    {
      // DI
      RegisterDepedencies();

      // XPO
      XpoUtils.CreateXpoDomain(_config, _log);

      // 2 is default- Gets or sets the maximum number of concurrent connections allowed by a
      // ServicePoint (connection management for HTTP connections) object.
      // Set the maximum number of ServicePoint instances to maintain. If a ServicePoint instance
      // for that host already exists when your application requests a connection to an Internet
      // resource, the ServicePointManager object returns this existing ServicePoint instance.
      // If none exists for that host, it creates a new ServicePoint instance.
      ServicePointManager.DefaultConnectionLimit = 100;

      // ISSUE: System.ServiceModel.Security.SecurityNegotiationException: Could not establish secure channel for SSL/TLS with authority!!??!!
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
      //ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
      //ServicePointManager.CheckCertificateRevocationList = false;

      #region Test
      var client = new NormalSearchServiceClient("NormalSearchServicePort");
      var test = client.PingServer();

      var server = new CS.WCF.Implementation.ScorecardServer(_log);
      //Test with Fraudulent ID
      var result = server.GetScorecardV2("001", "Just", "Goofy", "6601010313087", "M", new DateTime(1966, 1, 1),
        "10 Mars Street", "Mars", "", "", "1234", "", "", "", "", "", false);

      //Console.WriteLine("Press a key. ESC to exit...");
      //var key = Console.ReadKey();
      //if (key.Key == ConsoleKey.Escape)
      //{
      //  return;
      //}
      #endregion

      #region Topshelf service hosting
      HostFactory.Run(hc =>
      {
        hc.UseSerilog();
        hc.UseSimpleInjector(_container);

        hc.RunAsLocalSystem();
        hc.StartAutomatically();
        hc.SetServiceName("Atlas_Scorecard_V1");
        hc.SetDisplayName("Atlas Scorecard Server");
        hc.SetDescription("This service exposes basic CompuScan scorecard functionality, via " +
                "HTTP SOAP and .NET binary WCF services. If this service is stopped, this functionality will not be available to Atlas clients.");

        hc.Service<MainService>(sc =>
        {
          sc.ConstructUsingSimpleInjector();
          sc.WhenStarted((service, control) => service.Start());
          sc.WhenStopped((service, control) => service.Stop());
        });

      });
      #endregion Topshelf service hosting  
    }


    private static void RegisterDepedencies()
    {
      // Infrastructure 
      // ---------------------------------------
      _container.RegisterSingleton(_log);   
      _container.RegisterSingleton(_config);

      // WCF
      // ---------------------------------------
      _container.Register<IScorecardServiceHost>(() => new ScorecardServiceHost(_container));
    }


    // *Cross-cutting concerns*  we need instances upfront, so create here and register as singletons
    private static readonly ILogging _log = new SerilogLogging("Atlas.Scorecard.Server", true, typeof(Program));
    private static readonly IConfigSettings _config = new ConfigFileSettings();

    // DI
    private static readonly Container _container = new Container();

  }
}
