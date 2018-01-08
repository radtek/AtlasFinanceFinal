using System;
using Atlas.RabbitMQ.Messages.Push;
using Newtonsoft.Json.Linq;
using Serilog;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.Common.Socket.Event.Interface;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;

namespace Falcon.Service.Controllers
{
  public class FingerPrint : XSocketController
  {
    #region Push Response Structure

    public sealed class PushResponse
    {
      public bool Authenticated { get; set; }
      public bool HasError { get; set; }
      public string ErrorMessage { get; set; }
      public string Redirect { get; set; }
    }

    #endregion

    private static readonly ILogger _log = Log.Logger.ForContext<FingerPrint>();
    
    private string _request { get; set; }
    public string Request
    {
      get
      {
        return _request;
      }
      set
      {
        _request = value;
      }
    }

    public FingerPrint()
    {
      OnOpen += FingerPrint_OnOpen;
      OnClose += FingerPrint_OnClose;
    }

    void FingerPrint_OnClose(object sender, OnClientDisconnectArgs e)
    {
      _log.Information("[FingerPrint_OnClose] - Closed connection");
    }

    void FingerPrint_OnOpen(object sender, OnClientConnectArgs e)
    {
      _log.Information("[FingerPrint_OnOpen] - Opened connection");
    }

    public void Verify(ITextArgs textArgs)
    {
      JObject request = JObject.Parse(textArgs.data);

      //_log.Info(string.Format("[FingerPrint_Request] - Received Guid {0}", request));

      string update_type = "AUTH";
      dynamic data = "OK";

      this.SendTo(p => p.Request == Request, new { update_type, data }, "finger_check"  );
    }

    public void SendPushMessageResult(Guid trackingId, PushMessage msg)
    {
      PushResponse response = new PushResponse()
      {
        Authenticated = Boolean.Parse(msg.Parameters["Authenticated"].ToString()),
        HasError = Boolean.Parse(msg.Parameters["HasError"].ToString()),
        ErrorMessage = msg.Parameters["Error"] == null ? string.Empty : msg.Parameters["Error"].ToString(),
        Redirect = ""
      };

      this.SendTo(p => p.ClientGuid == trackingId, new { response }, "finger_check");
    }
  }
}