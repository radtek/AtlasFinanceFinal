using System;

using Atlas.WCF.FPServer.Interface;
using System.ServiceModel;
using Atlas.WCF.FPServer.Security.Interface;
using System.Collections.Generic;


namespace Atlas.Fingerprint.WCF.Client.ClientProxies
{
  public class FPServerClient : IFPServer, IDisposable
  {
    #region Public constructor

    public FPServerClient(
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

      _factory = new ChannelFactory<IFPServer>(binding, new EndpointAddress(Config.FPServerAddress + "/FPAdmin"));
      _channel = _factory.CreateChannel();
    }

    #endregion


    #region Public methods

    public int StartEnrollPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions,
      long personId, out long startEnrollRef, out string errorMessage)
    {
      return _channel.StartEnrollPerson(sourceRequest, scanner, scannerOptions, personId, out  startEnrollRef, out  errorMessage);
    }


    public int EnrollFingerprint(SourceRequest sourceRequest, long startEnrollRef, List<FPRawBufferDTO> fpBitmaps, out string errorMessage)
    {
      return _channel.EnrollFingerprint(sourceRequest, startEnrollRef, fpBitmaps, out errorMessage);
    }


    public int EndEnrollPerson(SourceRequest sourceRequest, long startEnrollRef, out string errorMessage)
    {
      return _channel.EndEnrollPerson(sourceRequest, startEnrollRef, out errorMessage);
    }


    public int CancelEnrollPerson(SourceRequest sourceRequest, long startEnrollRef, out string errorMessage)
    {
      return _channel.CancelEnrollPerson(sourceRequest, startEnrollRef, out errorMessage);
    }


    public int EnrollPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions,
      FPRawBufferDTO[] fpBitmaps, bool isStaff, out string errorMessage)
    {
      return _channel.EnrollPerson(sourceRequest, scanner, scannerOptions, fpBitmaps, isStaff, out errorMessage);
    }

   
    public int IdentifyPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions, FPRawBufferDTO[] compressedImages,
      out BasicPersonDetailsDTO person, out string errorMessage)
    {
      return _channel.IdentifyPerson(sourceRequest, scanner, scannerOptions, compressedImages, out person, out errorMessage);
    }


    public int GetTemplatesForPerson(SourceRequest sourceRequest, long personId, out List<FPTemplateDTO> templates, out string errorMessage)
    {
      return _channel.GetTemplatesForPerson(sourceRequest, personId,  out templates, out errorMessage);
    }
    

    public int GetPersonScanOptions(SourceRequest sourceRequest, FPScannerInfoDTO scanner, long personId,
      out FPScannerOptionDTO scanOptions, out string errorMessage)
    {
      return _channel.GetPersonScanOptions(sourceRequest, scanner, personId, out scanOptions, out errorMessage);
    }


    public int GetPersonVerifyOptions(SourceRequest sourceRequest, FPScannerInfoDTO scanner, long personId,
      out FPVerifyOptionsDTO verifyOptions, out string errorMessage)
    {
      return _channel.GetPersonVerifyOptions(sourceRequest, scanner, personId, out verifyOptions, out errorMessage);
    }

    
    public int UnEnrollPerson(SourceRequest sourceRequest, long personId, out string errorMessage)
    {
      // Not implemented here...
      errorMessage = "Not implemented";      
      return 0;
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
    ~FPServerClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IFPServer> _factory;
    private readonly IFPServer _channel;

    #endregion


  }
}
