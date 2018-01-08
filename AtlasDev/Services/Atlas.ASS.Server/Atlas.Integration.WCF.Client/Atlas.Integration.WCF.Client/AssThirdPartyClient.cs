using System;
using System.ServiceModel;

using Atlas.WCF.Interface;


namespace Atlas.Integration.WCF.Client
{
  public class AssThirdPartyClient : IAssThirdParty, IDisposable
  {
    #region Public constructor
    public AssThirdPartyClient(NetTcpBinding binding = null, EndpointAddress endpoint = null)   // 
    {
      if (binding == null)
      {
        binding = new NetTcpBinding
        {
          TransferMode = TransferMode.Buffered,

        };
      }      
      binding.Security.Mode = SecurityMode.None;

      if (endpoint == null)
      {
        endpoint = new EndpointAddress("net.tcp://172.31.75.38:8200/AssThirdParty");
      }
                  
      _factory = new ChannelFactory<IAssThirdParty>(binding, endpoint);
      _channel = _factory.CreateChannel();
    }

    #endregion


    public long AddUserOverride(AddUserOverrideArgs addUser)
    {
      return _channel.AddUserOverride(addUser);
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


    /// <summary>
    /// Finalizer
    /// </summary>
    ~AssThirdPartyClient()
    {
      Dispose(false);
    }

    #endregion


    #region Private fields

    private readonly ChannelFactory<IAssThirdParty> _factory;
    private readonly IAssThirdParty _channel;

    #endregion
  }
}
