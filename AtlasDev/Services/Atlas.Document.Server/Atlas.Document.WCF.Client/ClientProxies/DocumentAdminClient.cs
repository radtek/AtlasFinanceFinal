using System;
using System.ServiceModel;

using Atlas.DocServer.WCF.Interface;
using System.Collections.Generic;


namespace Atlas.Document.WCF.Client.ClientProxies
{
  /// <summary>
  /// Configless, IDisposable WCF client class for Document Admin
  /// </summary>
  public class DocumentAdminClient : IDocumentAdminServer, IDisposable
  {
    #region Public constructor

    public DocumentAdminClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.
    {
      var binding = new NetTcpBinding
        {
          CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
          OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
          SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(240),
          TransferMode = TransferMode.Buffered,
          MaxReceivedMessageSize = 18250000
        };

      binding.ReaderQuotas.MaxArrayLength = 18250000;
      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<IDocumentAdminServer>(binding, new EndpointAddress(Config.DocServerAddress + "/Doc/Admin"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods
    
    public string ChunkStartUpload()
    {
      return _channel.ChunkStartUpload();
    }

    public long ChunkGetFileSize(string fileCorrelationId)
    {
      return _channel.ChunkGetFileSize(fileCorrelationId);
    }

    public long ChunkAppendBytes(string fileCorrelationId, byte[] data)
    {
      return _channel.ChunkAppendBytes(fileCorrelationId, data);
    }

    
    public long StoreFileChunked(StorageInfo info, string fileCorrelationId, bool documentIsCompressed, byte[] sourceData, bool sourceDataIsCompressed)
    {
      return _channel.StoreFileChunked(info, fileCorrelationId, documentIsCompressed, sourceData, sourceDataIsCompressed);
    }


    public List<StorageInfo> StoreScannedFileChunked(string fileCorrelationId, FileFormatEnums.FormatType fileFormat, bool documentIsCompressed)
    {
      return _channel.StoreScannedFileChunked(fileCorrelationId, fileFormat, documentIsCompressed);
    }


    public List<StorageInfo> StoreScannedFile(byte[] data,  FileFormatEnums.FormatType fileFormat, bool documentIsCompressed)
    {
      return _channel.StoreScannedFile(data, fileFormat, documentIsCompressed);
    }


    public long StoreFile(StorageInfo info, byte[] document, bool documentIsCompressed, byte[] sourceData, bool sourceDataIsCompressed)
    {
      return _channel.StoreFile(info, document, documentIsCompressed, sourceData, sourceDataIsCompressed);
    }


    public long PrepareStore(StorageInfo info, byte[] sourceData, bool sourceDataIsCompressed)
    {
      return _channel.PrepareStore(info, sourceData, sourceDataIsCompressed);
    }


    public byte[] GetStorageFile(long storageId, bool destCompress)
    {
      return _channel.GetStorageFile(storageId, destCompress);
    }


    public StorageInfo GetStorageInfo(long storageId)
    {
      return _channel.GetStorageInfo(storageId);
    }


    public void AddFileToPreparedStore(StorageInfo info, byte[] document, bool documentIsCompressed)
    {
      _channel.AddFileToPreparedStore(info, document, documentIsCompressed);
    }


    public StorageInfo[] ListStorageInfoFor(string reference, GeneratorEnums.Generators generator,
      TemplateEnums.TemplateTypes template, Int64 sourceStorageId)
    {
      return _channel.ListStorageInfoFor(reference, generator, template, sourceStorageId);
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
    ~DocumentAdminClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IDocumentAdminServer> _factory;
    private readonly IDocumentAdminServer _channel = null;

    #endregion


  }
}
