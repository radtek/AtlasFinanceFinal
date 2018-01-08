using System;
using System.ServiceModel;

using Atlas.DocServer.WCF.Interface;


namespace Atlas.Document.WCF.Client.ClientProxies
{
  public class DocumentGeneratorClient : IDocumentGeneratorServer, IDisposable
  {
    #region Public constructor

    public DocumentGeneratorClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.)
    {
      var binding = new NetTcpBinding
      {
        CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
        OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
        SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(240),
        TransferMode = TransferMode.Buffered,
        MaxReceivedMessageSize = 6250000
      };

      binding.ReaderQuotas.MaxArrayLength = 6250000;
      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<IDocumentGeneratorServer>(binding, new EndpointAddress(Config.DocServerAddress + "/Doc/Generate"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods


    public DocTemplate GetTemplateByType(TemplateEnums.TemplateTypes templateType, int revision, LanguageEnums.Language language, bool wantTemplateFileBytes)
    {
      return _channel.GetTemplateByType(templateType, revision, language, wantTemplateFileBytes);
    }


    public DocTemplate GetTemplateById(long templateStoreId)
    {
      return _channel.GetTemplateById(templateStoreId);
    }


    public byte[] RenderDocumentWithJSonDataSet(long templateId, Int64 storageId, byte[] data, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      return _channel.RenderDocumentWithJSonDataSet(templateId, storageId, data, fileFormat, docOptions, renderOptions, destCompress);
    }


    public byte[] RenderDocumentWithDataSet(long templateId, Int64 storageId, byte[] data, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      return _channel.RenderDocumentWithDataSet(templateId, storageId, data, fileFormat, docOptions, renderOptions, destCompress);
    }


    public byte[] RenderDocument(long templateId, Int64 storageId, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      return _channel.RenderDocument(templateId, storageId, fileFormat, docOptions, renderOptions, destCompress);
    }


    public byte[] RenderDocumentWithJson(long templateId, Int64 storageId, string json, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      return _channel.RenderDocumentWithJson(templateId, storageId, json, fileFormat, docOptions, renderOptions, destCompress);
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
    ~DocumentGeneratorClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IDocumentGeneratorServer> _factory;
    private readonly IDocumentGeneratorServer _channel;

    #endregion



  }
}
