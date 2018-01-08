using System;
using System.Threading;

using AvsEngineLight.AVSProviders.Handlers;

using AvsEngineLight.Handlers.NuCard;
using Atlas.Common.Interface;


namespace AvsEngineLight
{
  class MainService
  {
    public MainService(ILogging log, AvsEngineLight.EasyNetQ.IAVSServiceBus avsServiceBus)
    {
      _log = log;
      _avsServiceBus = avsServiceBus;
    }


    public bool Start()
    {
      try
      {
        _log.Information("Service starting...");
                
        // Start threads to handle AVS provider comms
        _log.Information("Starting AVS handler threads...");

        // TODO: DI these?
        _xdsThread = new Thread(() => { _xdsHandler = new XDSHandler(_log); _xdsHandler.Start(); });
        _xdsThread.Start();

        _ncThread = new Thread(() => { _nucardHandler = new NuCardHandler(_log); _nucardHandler.Start(); });
        _ncThread.Start();       
        
        _log.Information("AVS handlers started");

        // Start EasyNetQ RabbitMQ service bus
        _avsServiceBus.Start();
                
        _log.Information("Service started");

        return true;
      }
      catch (Exception err)
      {
        _log.Fatal(err, "Start()");
        return false;
      }
    }


    public bool Stop()
    {
      try
      {
        _log.Information("Service stopping..");

        #region Close service bus
        _log.Information("Service bus stopping...");
        if (_avsServiceBus != null)
        {
          _avsServiceBus.Stop();
          _avsServiceBus = null;
        }

        _log.Information("Service bus stopped");
        #endregion
               
        _log.Information("Signaling AVS handlers to terminate..");
      
        if (_xdsHandler != null)
        {
          _xdsHandler.Stop();
          _xdsHandler = null;
        }

        if (_nucardHandler != null)
        {
          _nucardHandler.Stop();
          _nucardHandler = null;
        }

        _log.Information("AVS handlers signaled");

        // Give AVS handlers time to shut-down
        _log.Information("Waiting for AVS handlers to cleanly close...");
        Thread.Sleep(5000);

        _log.Information("Service stopped");

      }
      catch (Exception err)
      {
        _log.Error(err, "Stop()");
      }

      return true;
    }


    /// <summary>
    /// Logging
    /// </summary>
    private readonly ILogging _log;

    /// <summary>
    /// MassTransit RabbitMQ service bus
    /// </summary>
    private AvsEngineLight.EasyNetQ.IAVSServiceBus _avsServiceBus;

    /// <summary>
    /// XDS AVS handler
    /// </summary>
    private XDSHandler _xdsHandler;    
    private NuCardHandler _nucardHandler;

    private Thread _xdsThread;
    private Thread _ncThread;
  }
}
