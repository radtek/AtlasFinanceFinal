using Atlas.Common.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Xds
{
  public class Connect : IDisposable
  {
    private XDSConnect.XDSConnectWSSoapClient _xdsConnect = null;
    private string _ticket = string.Empty;
    private string _username = string.Empty;
    private string _password = string.Empty;

    public enum Destination
    {
      [Description("https://www.web.xds.co.za/xdsConnect/xdsconnectws.asmx")]
      PROD = 0,
			[Description("http://www.web.xds.co.za/uatxdsconnect/XDSConnectWS.asmx")]
      TEST = 1
    }

    public Connect(string username, string password, bool live = true)
    {
      _username = username;
      _password = password;
      if (live)
        _xdsConnect = new XDSConnect.XDSConnectWSSoapClient("XDSConnectWSSoap", Destination.PROD.ToStringEnum());
      else
        _xdsConnect = new XDSConnect.XDSConnectWSSoapClient("XDSConnectWSSoap", Destination.TEST.ToStringEnum());
    }

    public XDSConnect.XDSConnectWSSoapClient Client
    {
      get
      {
        if (IsTicketValid())
          return _xdsConnect;
        else
          throw new Exception("Cannot Log into XDS Connect");
      }
    }

    public string Ticket
    {
      get
      {
        return _ticket;
      }
    }

    private bool Login()
    {
      lock (_ticket)
      {
        _ticket = _xdsConnect.Login(_username, _password);
        return _xdsConnect.IsTicketValid(_ticket);
      }
    }

    private bool IsTicketValid()
    {
      if (!string.IsNullOrEmpty(_ticket) && _xdsConnect.IsTicketValid(_ticket))
        return true;
      else
        return Login();
    }

    public void Dispose()
    {
      _xdsConnect.Close();
      _xdsConnect = null;
    }
  }
}