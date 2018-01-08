using System.Collections.Generic;
using Falcon.Service.Business;
using Falcon.Common.Structures;
using Falcon.Common.Structures.Avs;
using Serilog;
using XSockets.Core.Common.Socket.Event.Arguments;
using XSockets.Core.XSocket;
using XSockets.Core.XSocket.Helpers;

namespace Falcon.Service.Controllers
{
  public class AccountDetail : XSocketController
  {
    private static readonly ILogger _log = Log.Logger.ForContext<AccountDetail>();
    private long _accountId { get; set; }
    public long AccountId
    {
      get
      {
        return _accountId;
      }
      set
      {
        _accountId = value;
      }
    }

    public AccountDetail()
    {
      OnOpen += AccountDetail_OnOpen;
      OnClose += AccountDetail_OnClose;
    }

    void AccountDetail_OnClose(object sender, OnClientDisconnectArgs e)
    {
      _log.Information("closed connection");
    }

    void AccountDetail_OnOpen(object sender, OnClientConnectArgs e)
    {
      _log.Information("opened connection");
    }

    public void SendAvsUpdate(long accountId)
    {
      if (HasSubscribers(accountId))
        SendUpdate(accountId, GetAvsTransactions(accountId), "avs");
    }

    public void SendNoteUpdate(long accountId, AccountNote note)
    {
      if (HasSubscribers(accountId))
        SendUpdate(accountId, note, "note");
    }

    public void SendNaedoUpdate(long accountId)
    {
      if (HasSubscribers(accountId))
        SendUpdate(accountId, AccountUtility.GetNaedoTransactions(accountId), "naedo");
    }

    public void SendPayoutUpdate(long accountId)
    {
      if (HasSubscribers(accountId))
        SendUpdate(accountId, AccountUtility.GetPayouts(accountId), "payout");
    }

    private bool HasSubscribers(long accountId)
    {
      var subscribers = GetSubscribers<AccountDetail>("update_data");

      foreach (var subscriber in subscribers)
      {
        if (subscriber.AccountId == accountId)
          return true;
      }
      return false;
    }

    private void SendUpdate(long accountId, dynamic data, string updateType)
    {
      this.SendTo(a => a._accountId == accountId, new { updateType, data }, "update_data");
    }

    private List<AvsTransactions> GetAvsTransactions(long accountId)
    {
      return new AvsUtility().GetAvsTransactions(null, null, null, null, null, null, accountId);
    }
  }
}