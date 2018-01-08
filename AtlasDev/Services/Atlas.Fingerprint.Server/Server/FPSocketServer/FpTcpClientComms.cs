using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

using Newtonsoft.Json;
using Atlas.Common.Interface;
/*
*  Client comms
* 
* 
* Network packet:
*     BYTES:
*     ______________________________________________________________
*       0     '255' - header indicator                              |
*      1-4     Encrypted packet length                              |  HEADER 
*      5-20    Crypto salt used for encrypted content               |
*     --------------------------------------------------------------
*      21-24   Packet number (first packet = 1)                     |
*      25-26   Function                                             | Encrypted data packet
*      27-     JSON encoded values/params (UTF8- optional)          |
*      -------------------------------------------------------------
*    
*/


namespace Atlas.WCF.FPServer.FPSocketServer
{  
  /// <summary>
  /// Class to handle comms with fingerprint client- used for client activation
  /// </summary>
  internal class FpTcpClientComms
  {
    internal FpTcpClientComms(TcpClient tcpClient, ILogging log)
    {
      Interlocked.Increment(ref _totalConns);
      Interlocked.Increment(ref _currClientCount);
         
      var endPoint = tcpClient.Client.RemoteEndPoint.ToString();
      log.Information("{@Client} connected- count: {CurrentClients}", endPoint, _currClientCount);

      var lastPacketSent = System.Diagnostics.Stopwatch.StartNew();   // Last packet sent to client  
      var lastPacketRecv = System.Diagnostics.Stopwatch.StartNew();   // Last packet received from client

      // Our session sending AES salt
      var sendSalt = new byte[16];
      new Random().NextBytes(sendSalt);
      var recvKey = new byte[] { 0xA0, 0x12, 0x23, 0x44, 0x55, 0x01, 0x98, 0xCC, 0xDA, 0x3F, 0xF0, 0x2F, 0xB1, 0xDA, 0x7F, 0xDE, 0x88, 0x77, 0x9A };
      var sendKey = new byte[] { 0xD0, 0xC1, 0x19, 0xFD, 0x09, 0x26, 0x98, 0xDF, 0x1A, 0x32, 0x20, 0x2F, 0x91, 0xDD, 0xAD, 0xF1, 0xBC, 0x77, 0x3A };

      UInt32 currPacketNumRecv = 0; // Current packet number we are receiving- to ensure no 'out of band' messages
      UInt32 currPacketNumSent = 0; // Current packet number we are sending
      string machineFingerprint = null; // The client's machine fingerprint

      try
      {
        try
        {
          using (var networkStream = tcpClient.GetStream())
          {
            while (tcpClient.Connected)
            {
              if (networkStream.DataAvailable)
              {
                #region Process a packet
                var message = FpTcpUtils.DecodeMessagePacket(networkStream, ++currPacketNumRecv, recvKey);
                lastPacketRecv.Restart();

                switch (message.Item1)
                {
                  case FpTcpUtils.FpFunctions.Ping: // PING                    
                    break;

                  case FpTcpUtils.FpFunctions.Register: // REGISTER
                    var machine = JsonConvert.DeserializeObject<RegisterMachine>(message.Item2);
                    machineFingerprint = machine.MachineFingerprint;
                    break;

                  default:
                    throw new ArgumentException(string.Format("Function number {0} unknown", message.Item1));
                }
                #endregion
              }

              #region Any pending messages for client?
              if (!string.IsNullOrEmpty(machineFingerprint))
              {
                var pending = GetPending(machineFingerprint);
                if (pending != null)
                {
                  foreach (var message in pending)
                  {
                    var packet = FpTcpUtils.CreateMessagePacket(sendSalt, sendKey, message.Item1, ++currPacketNumSent, message.Item2);
                    networkStream.Write(packet, 0, packet.Length);
                    lastPacketSent.Restart();
                  }
                }
              }
              #endregion

              #region Time to Ping client?
              if (lastPacketSent.Elapsed > TimeSpan.FromSeconds(60))
              {
                var ping = FpTcpUtils.CreateMessagePacket(sendSalt, sendKey, FpTcpUtils.FpFunctions.Ping, ++currPacketNumSent);
                networkStream.Write(ping, 0, ping.Length);
                lastPacketSent.Restart();
              }
              #endregion

              #region No ping from client for 90 seconds?
              //if (lastPacketRecv.Elapsed > TimeSpan.FromSeconds(90))
              //{
              //  throw new TimeoutException("No comms for 90 seconds");
              //}
              #endregion

              Thread.Sleep(100);
            }
          }
        }
        catch// (Exception err)
        {
          //log.Warning(err, "FP client comms- {@Client}", endPoint);
        }
      }
      finally
      {
        // Always close the connection to speed recovery...
        try
        {
          if (tcpClient.Connected)
          {
            tcpClient.Close();
          }
          tcpClient = null;
        }
        catch { }
      }

      Interlocked.Decrement(ref _currClientCount);
      log.Warning("Client disconnected- count: {CurrentClients}", _currClientCount);
    }


    /// <summary>
    /// Send a specific client machine a message- thread-safe
    /// </summary>
    /// <param name="clientFingerprint">The client's unique machine fingerprint</param>
    /// <param name="function">The message type</param>
    /// <param name="jsonParams">The message parameters</param>
    internal static void SendClientMessage(string clientFingerprint, FpTcpUtils.FpFunctions function, string jsonParams = null)
    {
      if (!_pending.ContainsKey(clientFingerprint))
      {
        _pending[clientFingerprint] = new ConcurrentQueue<Tuple<DateTime, FpTcpUtils.FpFunctions, string>>();
      }

      _pending[clientFingerprint].Enqueue(new Tuple<DateTime, FpTcpUtils.FpFunctions, string>(DateTime.Now, function, jsonParams));
    }



    /// <summary>
    /// Get all pending messages for a specific client- thread-safe
    /// </summary>
    /// <param name="clientFingerprint"></param>
    /// <returns>null if no messages, else list of pending messages for this client</returns>
    private static List<Tuple<FpTcpUtils.FpFunctions, string>> GetPending(string clientFingerprint, int maxAgeInSeconds = 15)
    {
      if (!_pending.ContainsKey(clientFingerprint))
      {
        _pending[clientFingerprint] = new ConcurrentQueue<Tuple<DateTime, FpTcpUtils.FpFunctions, string>>();
        return null;
      }

      var result = new List<Tuple<FpTcpUtils.FpFunctions, string>>();
      Tuple<DateTime, FpTcpUtils.FpFunctions, string> queueItem;
      while (_pending[clientFingerprint].TryDequeue(out queueItem))
      {
        if (DateTime.Now.Subtract(queueItem.Item1).TotalSeconds <= maxAgeInSeconds)
        {
          result.Add(new Tuple<FpTcpUtils.FpFunctions, string>(queueItem.Item2, queueItem.Item3));
        }
      }

      return result.Count > 0 ? result : null;
    }
    


    #region Private fields

    private static int _totalConns = 0;
    private static int _currClientCount = 0;

    /// <summary>
    /// Pending client requests
    /// </summary>
    private static readonly ConcurrentDictionary<string, ConcurrentQueue<Tuple<DateTime, FpTcpUtils.FpFunctions, string>>> _pending =
      new ConcurrentDictionary<string, ConcurrentQueue<Tuple<DateTime, FpTcpUtils.FpFunctions, string>>>();

    #endregion
    
  }
}
