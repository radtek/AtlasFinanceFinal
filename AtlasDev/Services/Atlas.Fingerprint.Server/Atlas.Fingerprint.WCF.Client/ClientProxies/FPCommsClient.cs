using System;
using System.ServiceModel;

using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;


namespace Atlas.Fingerprint.WCF.Client.ClientProxies
{
  public class FPCommsClient : IFPComms, IDisposable
  {
    #region Public constructor

    public FPCommsClient(
      TimeSpan? openTimeout = null,  // The interval of time provided for an open operation to complete including security handshakes
      TimeSpan? sendTimeout = null)  // The interval of time provided for an entire operation to complete. This includes both sending of message and receiving reply! The default is 00:01:00.)
    {
      var binding = new NetTcpBinding
      {
        CloseTimeout = TimeSpan.FromMinutes(1),                   // The interval of time provided for a close operation to complete. The default is 00:01:00.
        OpenTimeout = openTimeout ?? TimeSpan.FromSeconds(10),
        SendTimeout = sendTimeout ?? TimeSpan.FromSeconds(30),
        TransferMode = TransferMode.Buffered,
        MaxReceivedMessageSize = 6250000
      };

      binding.ReaderQuotas.MaxArrayLength = 6250000;
      binding.Security.Mode = SecurityMode.None;

      _factory = new ChannelFactory<IFPComms>(binding, new EndpointAddress(Config.FPServerAddress + "/FPComms"));
      _channel = _factory.CreateChannel();
    }


    #endregion


    #region Public methods

    public int LMS_MachineToUseFP(SourceRequest sourceRequest, out bool isEnabled, out string errorMessage)
    {
      return _channel.LMS_MachineToUseFP(sourceRequest, out isEnabled, out errorMessage);
    }


    public int LMS_MachineToUseFPForClients(SourceRequest sourceRequest, out bool isEnabled, out string errorMessage)
    {
      return _channel.LMS_MachineToUseFPForClients(sourceRequest, out isEnabled, out errorMessage);
    }


    public int LMS_IsMachineFPReady(SourceRequest sourceRequest, out bool isReady, out string errorMessage)
    {
      return _channel.LMS_IsMachineFPReady(sourceRequest, out isReady, out errorMessage);
    }


    public int CreatePerson(SourceRequest sourceRequest, BasicPersonDetailsDTO personDetails, out long personId, out string errorMessage)
    {
      return _channel.CreatePerson(sourceRequest, personDetails, out personId, out errorMessage);
    }


    public int GetPersonViaIdOrPassport(SourceRequest sourceRequest, string idOrPassport, out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      return _channel.GetPersonViaIdOrPassport(sourceRequest, idOrPassport, out personDetails, out errorMessage);
    }


    public int GetPersonViaOperatorId(SourceRequest sourceRequest, string personOperatorId, out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      return _channel.GetPersonViaOperatorId(sourceRequest, personOperatorId, out personDetails, out errorMessage);
    }


    public int GetPersonEnrolledFingers(SourceRequest sourceRequest, long personId, out System.Collections.Generic.List<int> enrolledFingers, out string errorMessage)
    {
      return _channel.GetPersonEnrolledFingers(sourceRequest, personId, out enrolledFingers, out errorMessage);
    }


    public int GetPersonViaPersonId(SourceRequest sourceRequest, long personId, out BasicPersonDetailsDTO personDetails, out string errorMessage)
    {
      return _channel.GetPersonViaPersonId(sourceRequest, personId, out personDetails, out errorMessage);
    }


    public int FPC_AddRequest(SourceRequest sourceRequest, FPRequestType requestType, string trackingId, long personId, long userPersonId, long adminPersonId, string message1, string message2, string message3, int timeoutSecs)
    {
      return _channel.FPC_AddRequest(sourceRequest, requestType, trackingId, personId, userPersonId, adminPersonId, message1, message2, message3, timeoutSecs);
    }


    public void FPC_UploadClientHWSWStatus(SourceRequest sourceRequest, byte[] machineInfo, out string errorMessage)
    {
      _channel.FPC_UploadClientHWSWStatus(sourceRequest, machineInfo, out errorMessage);
    }


    public void FPC_UploadClientFPStatus(SourceRequest sourceRequest, byte[] fpInfo, out string errorMessage)
    {
      _channel.FPC_UploadClientFPStatus(sourceRequest, fpInfo, out errorMessage);
    }


    public DateTime GetServerDateTime()
    {
      return _channel.GetServerDateTime();
    }


    public RecoveryInfoDTO FPC_CheckForOpenSession(SourceRequest sourceRequest)
    {
      return _channel.FPC_CheckForOpenSession(sourceRequest);
    }


    public int LMS_AddRequest(SourceRequest sourceRequest, FPRequestType requestType, long personId, long userPersonId, long adminPersonId, string message1, string message2, string message3, int timeoutSecs, out string requestId, out string errorMessage)
    {
      return _channel.LMS_AddRequest(sourceRequest, requestType, personId, userPersonId, adminPersonId,
        message1, message2, message3, timeoutSecs, out requestId, out errorMessage);
    }


    public int FPC_SetRequestDone(SourceRequest sourceRequest, string requestId, FPRequestStatus status, byte[] capturedImage, long personId, long userPersonId, long adminPersonId, int fingerId, out string errorMessage)
    {
      return _channel.FPC_SetRequestDone(sourceRequest, requestId, status, capturedImage, personId, userPersonId, adminPersonId, fingerId, out  errorMessage);
    }

    public int LMS_CheckRequestStatus(SourceRequest sourceRequest, string requestId, out FPRequestStatus status, out BasicPersonDetailsDTO person, out string errorMessage)
    {
      return _channel.LMS_CheckRequestStatus(sourceRequest, requestId, out  status, out  person, out  errorMessage);
    }

    public int LMS_UserCancelled(SourceRequest sourceRequest, string requestId, out string errorMessage)
    {
      return _channel.LMS_UserCancelled(sourceRequest, requestId, out  errorMessage);
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
    /// Finalizer
    /// </summary>
    ~FPCommsClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IFPComms> _factory;
    private readonly IFPComms _channel;

    #endregion

  }
}
