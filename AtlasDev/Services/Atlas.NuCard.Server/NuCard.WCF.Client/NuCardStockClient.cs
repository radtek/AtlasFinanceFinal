using System;
using System.ServiceModel;

using Atlas.NuCard.WCF.Interface;


namespace Atlas.NuCard.WCF.Client
{
  public class NuCardStockClient : INuCardStockServer, IDisposable
  {
    #region Public constructor

    public NuCardStockClient(TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.
    {
      var binding = new NetTcpBinding
        {
          CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
          OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
          SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(60),
          TransferMode = TransferMode.Buffered,
        };

      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<INuCardStockServer>(binding, new EndpointAddress(Config.NUCARD_SERVER_ADDRESS + "/NuCardStock"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods

    public int BranchAcceptInTransitCards(SourceRequest sourceRequest, System.Collections.Generic.List<NuCardStockItem> cards, out System.Collections.Generic.List<NuCardStockItem> cardsImported, out string errorMessage)
    {
      return _channel.BranchAcceptInTransitCards(sourceRequest, cards, out  cardsImported, out  errorMessage);
    }

    public int BranchImportUnknownCards(SourceRequest sourceRequest, System.Collections.Generic.List<NuCardStockItem> cards, out System.Collections.Generic.List<NuCardStockItem> cardsImported, out string errorMessage)
    {
      return _channel.BranchImportUnknownCards(sourceRequest, cards, out  cardsImported, out  errorMessage);
    }


    public int GetCardStatus(SourceRequest sourceRequest, NuCardStockItem card, out int cardStatus, out string errorMessage)
    {
      return _channel.GetCardStatus(sourceRequest, card, out cardStatus, out  errorMessage);
    }


    public int GetCardsInTransitForBranch(SourceRequest sourceRequest, out System.Collections.Generic.List<NuCardStockItem> cards, out string errorMessage)
    {
      return _channel.GetCardsInTransitForBranch(sourceRequest, out  cards, out errorMessage);
    }

    public int SendCardsToBranch(SourceRequest sourceRequest, NuCardBatchToDispatch batchDetails,
      out System.Collections.Generic.List<NuCardStockItem> cardsTransferred, out string errorMessage)
    {
      return _channel.SendCardsToBranch(sourceRequest, batchDetails, out cardsTransferred, out errorMessage);
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
    ~NuCardStockClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private ChannelFactory<INuCardStockServer> _factory;
    private INuCardStockServer _channel;

    #endregion



  }
}
