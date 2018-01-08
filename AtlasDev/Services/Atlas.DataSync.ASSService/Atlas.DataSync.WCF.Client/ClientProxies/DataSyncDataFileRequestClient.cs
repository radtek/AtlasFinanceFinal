using System;
using System.ServiceModel;

using Atlas.DataSync.WCF.Interface;


namespace Atlas.DataSync.WCF.Client.ClientProxies
{
  public class DataSyncDataFileRequestClient : IDataRequestServer, IDisposable
  {
    #region Public constructor

    public DataSyncDataFileRequestClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.
    {
      var binding = new NetTcpBinding
        {
          CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
          OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
          SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(180),
          TransferMode = TransferMode.Buffered,
          MaxReceivedMessageSize = 100000,
        };
            
      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<IDataRequestServer>(binding, new EndpointAddress(Config.DATASYNC_SERVER_ADDRESS + "/LMSData/Sync/DataFile/Request"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods


    public ProcessStatus StartGetBranchDBFs(SourceRequest sourceRequest)
    {
      return _channel.StartGetBranchDBFs(sourceRequest);
    }


    public ProcessStatus StartGetBranchPSQL(SourceRequest sourceRequest)
    {
      return _channel.StartGetBranchPSQL(sourceRequest);
    }


    public ProcessStatus GetProcessStatus(SourceRequest sourceRequest, string transactionId)
    {
      return _channel.GetProcessStatus(sourceRequest, transactionId);
    }


    public ProcessStatus ProcessUploadedBranchZIPDBF(SourceRequest sourceRequest, string clientTransactionId, string fileName)
    {
      return _channel.ProcessUploadedBranchZIPDBF(sourceRequest, clientTransactionId, fileName);
    }

    #endregion
    

    #region IDisposable implementation

    /// <summary>
    /// IDisposable.Dispose implementation, calls Dispose(true).
    /// </summary>
    void IDisposable.Dispose()
    {
      Dispose(true);
    }

    /// <summary>
    /// Dispose worker method. Handles graceful shutdown of the
    /// client even if it is an faulted state.
    /// </summary>
    /// <param name="disposing">Are we disposing (alternative
    /// is to be finalizing)</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        try
        {
          if (_factory.State != CommunicationState.Faulted)
          {
            _factory.Close();
          }
        }
        finally
        {
          if (_factory.State != CommunicationState.Closed)
          {
            _factory.Abort();
          }
        }
      }
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~DataSyncDataFileRequestClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IDataRequestServer> _factory;
    private readonly IDataRequestServer _channel = null;

    #endregion
    
  }
}
