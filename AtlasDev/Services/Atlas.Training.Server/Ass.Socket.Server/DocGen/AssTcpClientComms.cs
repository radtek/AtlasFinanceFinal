/* -----------------------------------------------------------------------------------------------------------------
 * 
 *  Copyright (C) 2012-2014 Atlas Finance (Pty() Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    Loopback TCP server for the ASS EXE client to call on our functionality (Document generation & admin).
 *    
 *    Uses simple Telnet CSV styled socket comms, with Base-64 parameter encoding.
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
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Pipes;

using Serilog;

using Atlas.Desktop.Utils.Network;


namespace Atlas.Desktop.AssComms
{
  internal class AssTcpClientComms
  {
    /// <summary>
    /// Loopback socket comms with local ASS EXE client- simple telnet styled comms using base-64 encoding
    /// Calls the desktop clients to actually handle the request
    /// </summary>
    /// <param name="tcpClient"></param>
    public AssTcpClientComms(TcpClient tcpClient)
    {
      try
      {
        try
        {
          using (var networkStream = tcpClient.GetStream())
          {
            _log.Information("Obtained new network stream");
            var cmd = networkStream.ReadLine(null, 500);
            if (!string.IsNullOrEmpty(cmd))
            {
              _log.Information("Request: {Length} bytes, {Data}...", cmd.Length, cmd.Substring(0, Math.Min(cmd.Length, 40)));

              var cmdParams = cmd.Split(new char[] { ',' }, StringSplitOptions.None);
              if (cmdParams.Length < 3)
              {
                _log.Error("ERR: Invalid parameter count, expected minimum 3, received: {ParamCount}", cmdParams.Length);
                networkStream.WriteString(string.Format("ERR: Invalid parameter count, expected minimum 4, received: {0}", cmdParams.Length));
                return;
              }

              var result = HandleCommand(cmdParams, cmd);
              if (!string.IsNullOrEmpty(result))
              {
                _log.Information("Response: {Length} bytes, {Data}...", result.Length, result.Substring(0, Math.Min(result.Length, 40)));
              }
              else
              {
                _log.Information("Response: <NULL>");
              }
              networkStream.WriteString(result);
            }
          }

        }
        catch (Exception err)
        {
          _log.Error(err, "AssTcpClientComms");
        }
      }
      finally
      {
        try
        {
          tcpClient.Close();
        }
        catch
        { }
        tcpClient = null;
      }
    }


    private static string HandleCommand(string[] cmdParams, string cmd)
    {
      // Use current desktop session Id as the queue name
      int sessionId;
      if (!int.TryParse(cmdParams[0], out sessionId))
      {
        return string.Format("ERR: Invalid session id: ", sessionId);
      }

      string result = null;

      var responseRecv = new ManualResetEventSlim(false);
      var wait = Task.Factory.StartNew(() =>
        {
          try
          {
            var channelId = string.Format("ClientPrint-{0}", sessionId);
            using (var client = new NamedPipeClientStream(channelId))
            {
              client.Connect();
              using (var reader = new StreamReader(client))
              using (var writer = new StreamWriter(client))
              {
                writer.WriteLine(cmd);
                writer.Flush();

                result = reader.ReadLine();
                responseRecv.Set();
              }
            }
          }
          catch (Exception err)
          {
            _log.Error(err, "HandleCommand: {@Params}", cmdParams);
          }
        });

      responseRecv.Wait(20000);

      return string.IsNullOrEmpty(result) ? "ERR: No response received from Client system" :  result;      
    }


    private static readonly ILogger _log = Log.ForContext<AssTcpClientComms>();
    
  }
}
