using System;
using System.ServiceModel;

using Atlas.DataSync.WCF.Interface;
using System.Collections.Generic;

namespace Atlas.DataSync.WCF.Client.ClientProxies
{
  public class DataSyncDataClient : IDataSyncServer, IDisposable
  {
    #region Public constructor

    public DataSyncDataClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.
    {
      var binding = new NetTcpBinding
        {
          CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
          OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
          SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(60),
          TransferMode = TransferMode.Buffered,
          MaxReceivedMessageSize = 500000
        };

      binding.ReaderQuotas.MaxArrayLength = 500000;
      binding.ReaderQuotas.MaxStringContentLength = 500000; // SQL Scripts
      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<IDataSyncServer>(binding, new EndpointAddress(Config.DATASYNC_SERVER_ADDRESS + "/LMSData/Sync/DataServices/Request"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods

    public bool ResetLastClientSyncId(SourceRequest sourceRequest)
    {
      return _channel.ResetLastClientSyncId(sourceRequest);
    }


    public void UploadCurrentBranchRecId(SourceRequest sourceRequest, long recId)
    {
      _channel.UploadCurrentBranchRecId(sourceRequest, recId);
    }


    public bool SetBranchDataStoreType(SourceRequest sourceRequest, bool sql)
    {
      return _channel.SetBranchDataStoreType(sourceRequest, sql);
    }


    public bool SetBranchDatabaseVersion(SourceRequest sourceRequest, string clientDbVersion)
    {
      return _channel.SetBranchDatabaseVersion(sourceRequest, clientDbVersion);
    }


    public long GetServerMasterSyncId(SourceRequest sourceRequest)
    {
      return _channel.GetServerMasterSyncId(sourceRequest);
    }


    public System.Collections.Generic.List<VerUpdateScripts> GetDbUpdateScript(SourceRequest sourceRequest, string clientDbVersion)
    {
      return _channel.GetDbUpdateScript(sourceRequest, clientDbVersion);
    }


    public System.Collections.Generic.List<KeyValueItem> GetBranchSettings(SourceRequest sourceRequest)
    {
      return _channel.GetBranchSettings(sourceRequest);
    }


    public PingResult Ping(SourceRequest sourceRequest)
    {
      return _channel.Ping(sourceRequest);
    }

    public System.Collections.Generic.List<string> GetMasterTableNames(SourceRequest sourceRequest)
    {
      return _channel.GetMasterTableNames(sourceRequest);
    }


    public string GetServerDatabaseVersion(SourceRequest sourceRequest)
    {
      return _channel.GetServerDatabaseVersion(sourceRequest);
    }


    public bool UploadBranchRowChanges(SourceRequest sourceRequest, string clientDbVersion, long lastClientRecId, byte[] binDataSet)
    {
      return _channel.UploadBranchRowChanges(sourceRequest, clientDbVersion, lastClientRecId, binDataSet);
    }


    public MasterTableRowChanges GetMasterRowChangesSince(SourceRequest sourceRequest, string clientDbVersion, long lastClientRecIdProcessed)
    {
      return _channel.GetMasterRowChangesSince(sourceRequest, clientDbVersion, lastClientRecIdProcessed);
    }


    public long BranchLastRecId(SourceRequest sourceRequest)
    {
      return _channel.BranchLastRecId(sourceRequest);
    }


    public void LogEvents(SourceRequest sourceRequest, System.Collections.Generic.List<LogEvent> events)
    {
      _channel.LogEvents(sourceRequest, events);
    }


    public string GetBranchServerIP(string branchCode)
    {
      return _channel.GetBranchServerIP(branchCode);
    }


    public DateTime GetServerDateTime()
    {
      return _channel.GetServerDateTime();
    }
    

    public long BranchLastAuditRecId(SourceRequest sourceRequest)
    {
      return _channel.BranchLastAuditRecId(sourceRequest);
    }


    public bool UploadBranchAuditChanges(SourceRequest sourceRequest, string clientDbVersion, long lastClientAuditRecId, List<lrep_audit> audit)
    {
      return _channel.UploadBranchAuditChanges(sourceRequest, clientDbVersion, lastClientAuditRecId, audit);
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
    ~DataSyncDataClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IDataSyncServer> _factory;
    private readonly IDataSyncServer _channel = null;

    #endregion
  }
}
