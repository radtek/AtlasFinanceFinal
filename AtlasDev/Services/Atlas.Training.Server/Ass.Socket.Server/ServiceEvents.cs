using Serilog;
using System;
using Topshelf;


namespace Atlas.Ass.Socket.Server
{
  class ServiceEvents
  {
    public ServiceEvents()
    {
      _log.Information("Service created");
      
    }

    public bool Start(HostControl hostControl)
    {
      _log.Information("Service starting");
      // Start DocGen server
      Atlas.Desktop.AssComms.AssTcpServer.StartServer();      
      _log.Information("Service started");
      return true;
    }



    public void Stop()
    {
      _log.Information("Service stopping");
      // Stop DocGen
      Atlas.Desktop.AssComms.AssTcpServer.StopServer();      
      _log.Information("Service stopped");
    }


    /// <summary>
    /// Logger
    /// </summary>
    private static readonly ILogger _log = Log.ForContext<ServiceEvents>();

  }
}
