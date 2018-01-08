using System;
using System.Timers;
using System.Diagnostics;
using System.Net;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;
using DevExpress.Xpo;
using log4net;
using log4net.Config;
using DevExpress.Xpo.DB;
using Atlas.Domain.Model;
using AccountService.WCF.Implementation;
using Atlas.Domain;

namespace AccountService
{
  public sealed class Engine
  {
    #region Private Members

    private readonly Stopwatch _logIsActive = new Stopwatch();
    private static readonly ILog _log = LogManager.GetLogger(typeof(Engine));

    private ServiceHost _accountServer;

    #endregion

    #region Private utility functions

    private static void EnumerateEndpointsActive(ServiceDescription service)
    {
      _log.Info(string.Format("WCF endpoints active: {0}", service.Endpoints.Count));
      foreach (var endpoint in service.Endpoints)
      {
        string info = string.Format("WCF endpoint is active- Address: '{0}', Contract name: '{1}', Binding name: '{2}'",
            endpoint.Address, endpoint.Contract.Name, endpoint.Binding.Name);

        _log.Info(info);
      }
    }

    #endregion

    public Engine()
    {
      #region Log4Net

      GlobalContext.Properties["HostName"] = Environment.MachineName;
      var fileVer = FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
      GlobalContext.Properties["FileVer"] = fileVer.FileVersion;
      GlobalContext.Properties["ProdVer"] = fileVer.ProductVersion;

      BasicConfigurator.Configure();
      XmlConfigurator.Configure();
      _log.Info("Logging service was created");

      #endregion

      _logIsActive.Start();
      _log.Info("Service is starting");
    }

    public void Start()
    {
      #region Start XPO- Create a thread-safe data layer
      try
      {
        // Create thread-safe- load and build the domain!
        _log.Info("Attempting to validate and build/load domain");
        var connStr = ConfigurationManager.ConnectionStrings["Atlas"].ConnectionString;
        var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {          
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;

        _log.Info("Successfully loaded and validated domain");
      }
      catch (Exception err)
      {
        _log.Error("Error with XPO domain", err);
        throw;
      }

      DomainMapper.Map();

      #endregion

      // 2 is default
      ServicePointManager.DefaultConnectionLimit = 100;

      #region Start WCF services

      try
      {
        _accountServer = new ServiceHost(typeof(AccountServer));
        _accountServer.Open();
        _log.Info("Successfully loaded 'Account' service");
        EnumerateEndpointsActive(_accountServer.Description);
      }
      catch (Exception err)
      {
        _accountServer = null;
        _log.Fatal("Failed to load 'Account' service", err);
      }

      #endregion

      _log.Info("Service was started");
    }

    public void Stop()
    {
      #region Close WCF services

      _log.Info("Closing WCF services");

      if (_accountServer != null)
      {
        _accountServer.Close();
        _accountServer = null;
        _log.Info("Account Service Closed");
      }

      _log.Info("WCF services stopped");

      #endregion

      _log.Info("Service was stopped");
    }
  }
}
