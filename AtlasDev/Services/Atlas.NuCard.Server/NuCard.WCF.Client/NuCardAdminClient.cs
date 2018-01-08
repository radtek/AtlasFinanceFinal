using System;
using System.ServiceModel;

using Atlas.NuCard.WCF.Interface;


namespace Atlas.NuCard.WCF.Client
{
  public class NuCardAdminClient : INuCardAdminServer, IDisposable
  {
    #region Public constructor

    public NuCardAdminClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.
    {
      var binding = new NetTcpBinding
        {
          CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
          OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
          SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(60),
          TransferMode = TransferMode.Buffered
        };

      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<INuCardAdminServer>(binding, new EndpointAddress(Config.NUCARD_SERVER_ADDRESS + "/NuCardAdmin"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods

    public int LinkCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage)
    {
      return _channel.LinkCard(sourceRequest, cardNumber, transactionID, out serverTransactionID, out errorMessage);
    }

    public int DeLinkCard(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out string serverTransactionID, out string errorMessage)
    {
      return _channel.DeLinkCard(sourceRequest, cardNumber, transactionID, out serverTransactionID, out errorMessage);
    }

    public int AllocateCard(SourceRequest sourceRequest, string cardNumber, string firstName, string lastName,
      string idOrPassportNumber, string cellPhoneNumber, string transactionID,
      out string serverTransactionID, out string errorMessage)
    {
      return _channel.AllocateCard(sourceRequest, cardNumber, firstName, lastName, idOrPassportNumber, cellPhoneNumber,
        transactionID, out serverTransactionID, out errorMessage);
    }

    public int CardBalance(SourceRequest sourceRequest, string cardNumber, string transactionID,
      out BalanceResult balanceResult, out string serverTransactionID, out string errorMessage)
    {
      return _channel.CardBalance(sourceRequest, cardNumber, transactionID, out balanceResult, out serverTransactionID, out errorMessage);
    }

    public int DeductFromProfileLoadCard(SourceRequest sourceRequest, string cardNumber, int amountInCents, string transactionID, out string serverTransactionID, out string errorMessage)
    {
      return _channel.DeductFromProfileLoadCard(sourceRequest, cardNumber, amountInCents, transactionID, out serverTransactionID, out errorMessage);
    }

    public int DeductFromCardLoadProfile(SourceRequest sourceRequest, string cardNumber, int amountInCents, string transactionID, out int transferredAmountInCents, out string serverTransactionID, out string errorMessage)
    {
      return _channel.DeductFromCardLoadProfile(sourceRequest, cardNumber, amountInCents, transactionID, out transferredAmountInCents, out serverTransactionID, out errorMessage);
    }

    public int TransferFundsBetweenCards(SourceRequest sourceRequest, string cardNumberFrom, string cardNumberTo, int amountInCents, string transactionID, bool stopTheFromCard, int stopReasonCodeID, out int transferredAmountInCents, out string serverTransactionID, out string errorMessage)
    {
      return _channel.TransferFundsBetweenCards(sourceRequest, cardNumberFrom, cardNumberTo, amountInCents, transactionID, stopTheFromCard, stopReasonCodeID, out  transferredAmountInCents, out  serverTransactionID, out  errorMessage);
    }

    public int CardStatement(SourceRequest sourceRequest, string cardNumber, out StatementResult statementResult, out string serverTransactionID, out string errorMessage)
    {
      return _channel.CardStatement(sourceRequest, cardNumber, out  statementResult, out  serverTransactionID, out  errorMessage);
    }

    public int StopCard(SourceRequest sourceRequest, string cardNumber, int stopReasonCodeID, string transactionID, out int transferredAmountInCents, out string serverTransactionID, out string errorMessage)
    {
      return _channel.StopCard(sourceRequest, cardNumber, stopReasonCodeID, transactionID, out  transferredAmountInCents, out  serverTransactionID, out  errorMessage);
    }

    public int GetCardStatus(SourceRequest sourceRequest, string cardNumber, string transactionID, out CardStatus cardStatus, out string serverTransactionID, out string errorMessage)
    {
      return _channel.GetCardStatus(sourceRequest, cardNumber, transactionID, out cardStatus, out  serverTransactionID, out  errorMessage);
    }

    public int CancelStopCard(SourceRequest sourceRequest, string cardNumber, string transactionID, out string serverTransactionID, out string errorMessage)
    {
      return _channel.CancelStopCard(sourceRequest, cardNumber, transactionID, out  serverTransactionID, out  errorMessage);
    }

    public int ResetPin(SourceRequest sourceRequest, string cardNumber, out string serverTransactionID, out string errorMessage)
    {
      return _channel.ResetPin(sourceRequest, cardNumber, out  serverTransactionID, out  errorMessage);
    }

    public int UpdateAllocatedCard(SourceRequest sourceRequest, string cardNumber, string cellphoneNumber, string transactionID, out string serverTransactionID, out string errorMessage)
    {
      return _channel.UpdateAllocatedCard(sourceRequest, cardNumber, cellphoneNumber, transactionID, out  serverTransactionID, out  errorMessage);
    }

    public int TryPingNuCard(SourceRequest sourceRequest, out string errorMessage)
    {
      return _channel.TryPingNuCard(sourceRequest, out errorMessage);
    }

    public int IsBranchConfigured(SourceRequest sourceRequest, out string errorMessage)
    {
      return _channel.IsBranchConfigured(sourceRequest, out  errorMessage);
    }

    public int AllocateAndDeductFromProfileLoadCard(SourceRequest sourceRequest, string cardNumber, string firstName, string lastName, string idOrPassportNumber, string cellPhoneNumber, int amountInCents, string transactionID, out string serverTransactionID, out string errorMessage)
    {
      return _channel.AllocateAndDeductFromProfileLoadCard(sourceRequest, cardNumber, firstName, lastName,
  idOrPassportNumber, cellPhoneNumber, amountInCents, transactionID, out  serverTransactionID, out  errorMessage);
    }

    public int TransferFundsToNewCard(SourceRequest sourceRequest, string cardNumberFrom, string cardNumberTo, int stopReasonCodeID, string firstName, string lastName, string idOrPassportNumber, string cellPhoneNumber, string transactionID, out int transferredAmountInCents, out string serverTransactionID, out string errorMessage)
    {
      return _channel.TransferFundsToNewCard(sourceRequest, cardNumberFrom, cardNumberTo, stopReasonCodeID, firstName,
   lastName, idOrPassportNumber, cellPhoneNumber, transactionID, out  transferredAmountInCents, out  serverTransactionID, out  errorMessage);
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
    ~NuCardAdminClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private ChannelFactory<INuCardAdminServer> _factory;
    private INuCardAdminServer _channel;

    #endregion

  }
}
