using System.ServiceModel;

using Atlas.Common.Interface;


namespace Atlas.Servers.Common.WCF
{
  /// <summary>
  /// Base implementation for WCF ServiceHost
  /// </summary>
  public interface IBaseServiceHost
  {
    void Open();

    void Close();

    void LogEndpoints(ILogging log);
  }


  /// <summary>
  /// Abstract class to provide common IBaseServiceHost implementation
  /// </summary>
  public abstract class BaseServiceHost
  {
    public void Open()
    {
      _serviceHost.Open();
    }


    public void Close()
    {
      if (_serviceHost != null)
      {
        _serviceHost.Close();
        _serviceHost = null;
      }
    }


    public void LogEndpoints(ILogging log)
    {
      WcfUtils.EnumerateEndpointsActive(_serviceHost.Description, log);
    }


    protected ServiceHost _serviceHost;

  }
}