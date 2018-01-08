using System;
using System.ServiceModel;

using Atlas.DocServer.WCF.Interface;


namespace Atlas.Document.WCF.Client.ClientProxies
{
  public class DocumentRecognitionClient : IDocumentRecognition, IDisposable
  {
    #region Public constructor
    
    public DocumentRecognitionClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.)
    {
      var binding = new NetTcpBinding
      {
        CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
        OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
        SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(60),
        TransferMode = TransferMode.Buffered,
        MaxReceivedMessageSize = 100000
      };

      binding.ReaderQuotas.MaxArrayLength = 100000;
      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<IDocumentRecognition>(binding, new EndpointAddress(Config.DocServerAddress + "/Doc/Recognition"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods

    public System.Collections.Generic.List<BarcodeFound> FindBarcodes(FileFormatEnums.FormatType sourceFileFormat, 
      byte[] source, string openPassword, bool isCompressed, System.Collections.Generic.List<BarcodeEnums.BarcodeDirections> searchOrientation, 
      System.Collections.Generic.List<BarcodeEnums.BarcodeTypes> searchForTypes)
    {
      return _channel.FindBarcodes(sourceFileFormat, source, openPassword, isCompressed, searchOrientation, searchForTypes);
    }


    public DocumentFound DetermineDocumentCategory(FileFormatEnums.FormatType sourceFileFormat, byte[] source, 
      string openPassword, bool isCompressed)
    {
      return _channel.DetermineDocumentCategory(sourceFileFormat, source, openPassword, isCompressed);
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
    ~DocumentRecognitionClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IDocumentRecognition> _factory;
    private readonly IDocumentRecognition _channel;

    #endregion

  }
}