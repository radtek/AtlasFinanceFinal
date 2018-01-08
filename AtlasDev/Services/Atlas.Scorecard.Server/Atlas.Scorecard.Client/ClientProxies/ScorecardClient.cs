using System;
using Atlas.ThirdParty.CS.WCF.Interface;
using System.ServiceModel;

namespace Atlas.ThirdParty.CS.Bureau.Client.ClientProxies
{
  public class ScorecardClient : IScorecardServer, IDisposable
  {
    public ScorecardClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.
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

      _factory = new ChannelFactory<IScorecardServer>(binding, new EndpointAddress(Config.ScorecardServerAddress));
      _channel = _factory.CreateChannel();
    }


    public ScorecardV2Result GetScorecardV2(string legacyBranchNum, string firstName, string surname, string idNumber,
      string gender, DateTime dateOfBirth, string addressLine1, string addressLine2, string addressLine3, string addressLine4,
      string postalCode, string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo, bool isIdPassportNo)
    {
      return _channel.GetScorecardV2(legacyBranchNum, firstName, surname, idNumber, gender, dateOfBirth, addressLine1,
        addressLine2, addressLine3, addressLine4, postalCode, homeTelCode, homeTelNo, workTelCode, workTelNo, cellNo, isIdPassportNo);
    }


    public ScorecardSimpleResult GetSimpleScorecard(string legacyBranchNum, string firstName, string surname, string idNumber, bool isIdPassportNo)
    {
      return _channel.GetSimpleScorecard(legacyBranchNum, firstName, surname, idNumber, isIdPassportNo);
    }


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
    /// Finalizer
    /// </summary>
    ~ScorecardClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IScorecardServer> _factory;
    private readonly IScorecardServer _channel;

    #endregion




  }
}
