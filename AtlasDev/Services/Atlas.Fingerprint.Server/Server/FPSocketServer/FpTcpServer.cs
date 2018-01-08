using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.FPSocketServer
{
  /// <summary>
  /// Fingerprint socket server- used for server activating clients
  /// </summary>
  internal class FpTcpServer
  {
    /// <summary>
    /// Start TCP server on all IP's, port 8210
    /// </summary>
    internal static void StartServer(ILogging log)
    {
      _log = log;

      _fpServer = new Thread(() =>
        {
          var listener = new TcpListener(IPAddress.Any, 8210);
          listener.Start();

          while (true)
          {
            try
            {
              _callbackDone.Reset();
              listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
               _callbackDone.WaitOne();              
            }
            catch (Exception err)
            {
              log.Fatal(err, "StartServer");
            }
          }
        }) { IsBackground = true };

      _fpServer.Start();
    }


    private static void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
      _callbackDone.Set(); // Restart listening

      try
      {
        var listener = (TcpListener)ar.AsyncState;
        var client = listener.EndAcceptTcpClient(ar);
        (new Thread(() => new FpTcpClientComms(client, _log)) { IsBackground = true }).Start();
      }
      catch (Exception err)
      {
        _log.Error(err, "DoAcceptTcpClientCallback");
      }      
    }


    /// <summary>
    /// Fingerprint TCP server
    /// </summary>
    private static Thread _fpServer;

    /// <summary>
    /// Wait for client to connect
    /// </summary>
    private static ManualResetEvent _callbackDone = new ManualResetEvent(false);

    private static ILogging _log;

  }
}
