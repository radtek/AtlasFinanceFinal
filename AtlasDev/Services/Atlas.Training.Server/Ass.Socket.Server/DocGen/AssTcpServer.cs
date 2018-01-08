/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Loopback threaded TCP server connector for Ass
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *    Oct/Nov 2014 - Started
 *    
 *    
 * 
 * ----------------------------------------------------------------------------------------------------------------- */
 
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Serilog;


namespace Atlas.Desktop.AssComms
{
  static internal class AssTcpServer
  {
    /// <summary>
    /// ASS TCP server for socket comms with Ass- handles print-preview by passing request to ClientPrint desktop app via Named pipes
    /// </summary>
    public static void StartServer()
    {
      (new Thread(async () =>
        {
          var listener = new TcpListener(IPAddress.Loopback, 9090);
          listener.Start();

          while (!_cts.IsCancellationRequested)
          {
            try
            {
              var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
              (new Thread(() => new AssTcpClientComms(client)) { IsBackground = true }).Start();
            }
            catch (Exception err)
            {
              Log.Error(err, "StartServer");
              System.Media.SystemSounds.Asterisk.Play();
            }
          }
        }) { IsBackground = true }).Start();
    }


    public static void StopServer()
    {
      _cts.Cancel();
    }

   

    #region Private fields

    /// <summary>
    /// Set when main server is to be terminated
    /// </summary>
    private readonly static CancellationTokenSource _cts = new CancellationTokenSource();
        
    #endregion
    
  }
}