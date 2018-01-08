using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.WebService;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Helpers
{
  public class SharedServices : IDisposable
  {
    private Client currentClient = null;

    private UnitOfWork unitOfWork = null;
    private bool disposing = false;
    private WebServiceClient webServiceClient = null;

    private object _sessionLock = new object();

    public UnitOfWork XpoUnitOfWork
    {
      get
      {
        if (disposing)
        {
          throw new InvalidOperationException("Attepting to access UnitOfWork while disposing controller.");
        }

        lock (_sessionLock)
        {

          if (unitOfWork == null)
          {
            unitOfWork = XpoHelper.GetNewUnitOfWork();
          }

          return unitOfWork;
        }
      }
    }

    public WebServiceClient WebServiceClient
    {
      get
      {
        if (webServiceClient == null)
        {
          webServiceClient = new WebServiceClient("WebServer.NET");
        }

        return webServiceClient;
      }
    }

    /// <summary>
    /// Fetches the current client based on the logged in user
    /// </summary>
    public Client CurrentClient
    {
      get
      {
        int currentUserId = WebSecurity.CurrentUserId;
        if (currentUserId < 0)
        {
          return null;
        }

        if (currentClient == null || currentClient.UserId != currentUserId)
        {
          return currentClient = new XPQuery<Client>(XpoUnitOfWork).FirstOrDefault(c => c.UserId == currentUserId);
        }

        return currentClient;
      }
    }

    public void Dispose()
    {
      disposing = true;
      if (unitOfWork != null)
      {
        unitOfWork.Dispose();
        unitOfWork = null;
      }
    }
  }
}
