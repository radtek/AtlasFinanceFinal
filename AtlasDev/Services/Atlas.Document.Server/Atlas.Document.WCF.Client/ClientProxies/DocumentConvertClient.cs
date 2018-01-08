using System;
using System.ServiceModel;

using Atlas.DocServer.WCF.Interface;


namespace Atlas.Document.WCF.Client.ClientProxies
{
  public class DocumentConvertClient : IDocumentConvertServer, IDisposable
  {
    #region Public constructor

    public DocumentConvertClient(
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

      _factory = new ChannelFactory<IDocumentConvertServer>(binding, new EndpointAddress(Config.DocServerAddress + "/Doc/Convert"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods

    public byte[] Convert(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, FileFormatEnums.FormatType destFileFormat, string openSourcePassword, DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      return _channel.Convert(source, isCompressed, sourceFileFormat, destFileFormat, openSourcePassword, docOptions, renderOptions, destCompress);
    }


    public byte[] PDFAddPassword(byte[] source, bool isCompressed, string openPassword, DocOptions docOptions, bool destCompress)
    {
      return _channel.PDFAddPassword(source, isCompressed, openPassword, docOptions, destCompress);
    }


    public bool IsValidDocument(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, string openPassword)
    {
      return _channel.IsValidDocument(source, isCompressed, sourceFileFormat, openPassword);
    }


    public bool DocumentContainsData(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, string openPassword)
    {
      return _channel.DocumentContainsData(source, isCompressed, sourceFileFormat, openPassword);
    }


    public byte[] PDFOptimize(byte[] source, bool isCompressed, string openPassword, bool flattenFields, bool removeAllBookmarks, bool removeAllSignatures, bool removeAllLinks, bool removeAllAnnotations, byte setImagesBitDepth, int jp2KQuality, DocOptions options, bool destCompress)
    {
      return _channel.PDFOptimize(source, isCompressed, openPassword, flattenFields, removeAllBookmarks, removeAllSignatures, removeAllLinks,
        removeAllAnnotations, setImagesBitDepth, jp2KQuality, options, destCompress);
    }


    public byte[] CleanUpScan(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, string openPassword, CleanUpOptions cleanUpOptions, DocOptions docOptions, RenderOptions renderOptions, FileFormatEnums.FormatType destFileFormat, bool destCompress)
    {
      return _channel.CleanUpScan(source, isCompressed, sourceFileFormat, openPassword, cleanUpOptions, docOptions, renderOptions,
        destFileFormat, destCompress);
    }


    public byte[] PDFAddSignature(byte[] source, bool isCompressed, string openPassword, DocOptions options, bool destCompress)
    {
      return _channel.PDFAddSignature(source, isCompressed, openPassword, options, destCompress);
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
    ~DocumentConvertClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IDocumentConvertServer> _factory;
    private readonly IDocumentConvertServer _channel;

    #endregion

  }
}
