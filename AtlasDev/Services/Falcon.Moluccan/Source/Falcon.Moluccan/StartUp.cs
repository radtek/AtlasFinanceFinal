using System;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.SignalR;
using Falcon.Moluccan.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using Serilog;
using Serilog.Events;

namespace Falcon.Moluccan
{
  public class StartUp
  {
    public void Configuration(IAppBuilder appBuilder)
    {

      #region Owin

      HttpConfiguration config = new HttpConfiguration();

      #endregion

      #region Autofac Registrations

      var builder = new ContainerBuilder();

      builder.RegisterHubs(Assembly.GetAssembly(typeof(StreamHub))).ExternallyOwned();
      builder.Register(c => SerilogConfig.CreateLogger()).As<ILogger>().SingleInstance();

      #endregion

      var container = builder.Build();

      #region SignalR

      appBuilder.Map("/signalr", map =>
      {
        map.UseCors(CorsOptions.AllowAll);

        var hubConfiguration = new HubConfiguration
        {
          EnableDetailedErrors = true, // debugging only,
          Resolver = new AutofacDependencyResolver(container)
        };

        map.RunSignalR(hubConfiguration);
      });

      #endregion

      #region Middlware Wiring

      appBuilder.UseCors(CorsOptions.AllowAll);

      appBuilder.UseAutofacMiddleware(container);

      appBuilder.UseWebApi(config);     


      #endregion
    }
    public class SerilogConfig
    {
      public static ILogger CreateLogger()
      {
        var config = new LoggerConfiguration().
            MinimumLevel.Debug().
            WriteTo.ColoredConsole(LogEventLevel.Debug);
        InitialiseGlobalContext(config);

        return config.CreateLogger();
      }

      public static LoggerConfiguration InitialiseGlobalContext(LoggerConfiguration configuration)
      {
        return configuration.Enrich.WithMachineName()
                            .Enrich.WithProperty("ApplicationName", typeof(SerilogConfig).Assembly.GetName().Name)
                            .Enrich.WithProperty("UserName", Environment.UserName)
                            .Enrich.WithProperty("AppDomain", AppDomain.CurrentDomain)
                            .Enrich.WithProperty("RuntimeVersion", Environment.Version)
                            .Enrich.FromLogContext();
      }
    }
  }
}