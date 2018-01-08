using System;

using Atlas.WCF.Implementation;
using Atlas.Common.Interface;
using Atlas.Server.NuCard.WCF.DI;
using Atlas.Servers.Common.WCF;


namespace Atlas.Server.NuCard
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
  public class MainService
  {
    /// <summary>
    /// Public constructor- perform any system initialization
    /// </summary>
    public MainService(ILogging log, INuCardAdminServiceHost nucardAdmin, INuCardStockServiceHost nucardStock)
    {
      _log = log;
      _serviceHost_NuCardStock = nucardStock;
      _serviceHost_NuCardAdmin = nucardAdmin;
      _log.Information("Service is initializing");
    }


    /// <summary>
    /// Called when service is started- start WCF and Quartz tasks
    /// </summary>
    public bool Start()
    {
      try
      {
        _log.Information("Service is starting");

        #region Start WCF services
        try
        {
          _serviceHost_NuCardAdmin.Open();
          _log.Information("Successfully loaded 'NuCardAdmin' service");
          _serviceHost_NuCardAdmin.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _log.Fatal(err, "Failed to load 'NuCard' administration service");
          throw;
        }

        try
        {
          _serviceHost_NuCardStock.Open();
          _log.Information("Successfully loaded 'NuCardStock' service");
          _serviceHost_NuCardStock.LogEndpoints(_log);
        }
        catch (Exception err)
        {
          _log.Fatal(err, "Failed to load 'NuCard' stock service");
          throw;
        }
        #endregion
                   
        _log.Information("Starting message bus....");
        MessagingBus.StartBus();
        _log.Information("Message bus started");

        return true;   
      }
      catch (Exception err)
      {
        _log.Error(err, "Start()");
        return false;
      }
    }


    /// <summary>
    /// Called when service is stopped
    /// </summary>
    public bool Stop()
    {
      try
      {
        #region Close WCF services
        _log.Information("Terminating WCF services");

        _serviceHost_NuCardAdmin.Close();
        _log.Information("NuCard SP administration service closed");

        _serviceHost_NuCardStock.Close();
        _log.Information("NuCard stock service closed");

        _log.Information("WCF services terminated");
        #endregion

        _log.Information("Shutting down message bus....");
        MessagingBus.ShutdownBus();
        _log.Information("Message bus stopped");

        _log.Information("Service was stopped");
      }
      catch (Exception err)
      {
        _log.Error(err, "Stop()");
      }

      return true;
    }


    /// <summary>
    /// WCF NuCard admin- Tutuka services- funds, card activate, etc.
    /// </summary>
    private readonly IBaseServiceHost _serviceHost_NuCardAdmin;

    /// <summary>
    /// WCF NuCard stock admin- manage Atlas NuCard stock control
    /// </summary>
    private readonly IBaseServiceHost _serviceHost_NuCardStock;
           
    private readonly ILogging _log;
 
  }
}
