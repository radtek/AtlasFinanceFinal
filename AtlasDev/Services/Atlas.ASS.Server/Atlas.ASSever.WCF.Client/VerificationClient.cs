using System;
using System.ServiceModel;

using Atlas.Enumerators;
using Atlas.WCF.Interface;


namespace Atlas.ASSever.WCF.Client
{
  public class VerificationClient : IVerificationServer, IDisposable
  {
    #region Public constructor
    public VerificationClient(NetTcpBinding binding, EndpointAddress endpoint) 
    {
      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<IVerificationServer>(binding, endpoint);
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods

    public bool IsCDV(long bankId, long bankAccountTypeId, string bankAccountNo)
    {
      return _channel.IsCDV(bankId, bankAccountTypeId, bankAccountNo);
    }

    public bool IsCDVWithBranch(long bankId, long bankAccountTypeId, string bankAccountNo, string branchCode)
    {
      return _channel.IsCDVWithBranch(bankId, bankAccountTypeId, bankAccountNo, branchCode);
    }

    public AVSReply DoAVSEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode)
    {
      return _channel.DoAVSEnquiry(initials, lastName, idNo, accountNo, bankName, branchCode);
    }

    public AVSReply DoEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, 
      string branchCode, bool forceCheck)
    {
      return _channel.DoEnquiry(initials,  lastName,  idNo,  accountNo, bankName,  branchCode,  forceCheck);
    }

    public AVSReply DoLegacyEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, 
      string branchCode, string legacyBranchCode, bool forceCheck)
    {
      return _channel.DoLegacyEnquiry(initials, lastName, idNo, accountNo, bankName, branchCode, legacyBranchCode, forceCheck);
    }

    public AVSResponse GetAVSResponse(long transactionId)
    {
      return _channel.GetAVSResponse(transactionId);
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
    /// Dispose worker method. Handles graceful shutdown of the client even if it is an faulted state.
    /// </summary>
    /// <param name="disposing">Are we disposing (alternative is to be finalizing)</param>
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

    public AVSReply DoAVSEnquiryWithHost(long? personId, string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode, General.Host host, General.BankPeriod bankPeriod)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Finalizer
    /// </summary>
    ~VerificationClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IVerificationServer> _factory;
    private readonly IVerificationServer _channel;

    #endregion

  }
}
