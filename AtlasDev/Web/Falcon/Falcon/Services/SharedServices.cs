using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;

using Falcon.Gyrkin.Library.Service;


namespace Falcon.Services
{
  public sealed class SharedServices : IDisposable
  {
    //private UnitOfWork unitOfWork = null;
    private bool disposing = false;
    private readonly object _sessionLock = new object();
    private FalconServiceClient webServiceClient = null;

    //public UnitOfWork UnitOfwork
    //{
    //  get
    //  {
    //    if (disposing)
    //      throw new InvalidOperationException("Attepting to access UnitOfWork while disposing controller.");

    //    lock (_sessionLock)
    //    {

    //      if (unitOfWork == null)
    //        unitOfWork = Xpo.GetUnitOfWork();

    //      return unitOfWork;
    //    }
    //  }
    //}

    public FalconServiceClient WebServiceClient
    {
      get
      {
        if (webServiceClient == null)
        {
          webServiceClient = new FalconServiceClient("FalconService.NET");
        }

        if(webServiceClient.State == CommunicationState.Closed || webServiceClient.State == CommunicationState.Closing)
          webServiceClient = new FalconServiceClient("FalconService.NET"); 

        return webServiceClient;
      }
    }

    ///// <summary>
    ///// Fetches the current client based on the logged in user
    ///// </summary>
    //public Client CurrentClient
    //{
    //  get
    //  {
    //    int currentUserId = WebSecurity.CurrentUserId;
    //    if (currentUserId < 0)
    //    {
    //      return null;
    //    }

    //    if (currentClient == null || currentClient.UserId != currentUserId)
    //    {
    //      return currentClient = new XPQuery<Client>(XpoUnitOfWork).First(c => c.UserId == currentUserId);
    //    }

    //    return currentClient;
    //  }
    //}

    [SuppressMessage("Microsoft.Usage", "CA2213:Diposing of webserviceClient is managed by the calling controller")]
    public void Dispose()
    {
      disposing = true;

      //if (unitOfWork != null)
      //{
      //  unitOfWork.Dispose();
      //  unitOfWork = null;
      //}
    }
  }
}
