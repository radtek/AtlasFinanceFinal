using System;
using System.ServiceModel;

using Atlas.DataSync.WCF.Interface;


namespace Atlas.DataSync.WCF.Client.ClientProxies
{
  public class DataSyncAdminClient : IAdminServer, IDisposable
  {
    #region Public constructor

    public DataSyncAdminClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.
    {
      var binding = new NetTcpBinding
        {
          CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
          OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
          SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(60),
          TransferMode = TransferMode.Buffered,
          MaxReceivedMessageSize = 6250000
        };

      binding.ReaderQuotas.MaxArrayLength = 6250000;
      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<IAdminServer>(binding, new EndpointAddress(Config.DATASYNC_SERVER_ADDRESS + "/LMSData/Sync/Admin"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods

    public byte[] GetMasterTableData(SourceRequest sourceRequest, string masterTableName)
    {
      return _channel.GetMasterTableData(sourceRequest, masterTableName);
    }

    public bool UploadMasterTableRow(SourceRequest sourceRequest, string masterTableName, byte[] dataTable)
    {
      return _channel.UploadMasterTableRow(sourceRequest, masterTableName, dataTable);
    }

    public bool XHarbourConvertedAMasterTable(SourceRequest sourceRequest, string tableName)
    {
      return _channel.XHarbourConvertedAMasterTable(sourceRequest, tableName);
    }

    public bool UpgradeDatabase(SourceRequest sourceRequest, string version, string description, string sqlUpdateScript)
    {
      return _channel.UpgradeDatabase(sourceRequest, version, description, sqlUpdateScript);
    }

  
    public void UpdatedBranchServer(SourceRequest sourceRequest, string legacyBranchNum)
    {
      _channel.UpdatedBranchServer(sourceRequest, legacyBranchNum);
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
    ~DataSyncAdminClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IAdminServer> _factory;
    private readonly IAdminServer _channel = null;

    #endregion

  }
}
