using System.Configuration;
using Microsoft.Owin.Hosting;

namespace Falcon.Gyrkin
{
  public class Engine
  {
    public void Start()
    {
      var options = new StartOptions();
      var bindingHosts = ConfigurationManager.AppSettings["bindingHosts"];
      if(bindingHosts.Contains(";"))
      {
        string[] hosts = bindingHosts.Split(';');
        foreach(var host in hosts)
        {
          options.Urls.Add(host);
        }
      }
      else
      {
        options.Urls.Add(bindingHosts);
      }
      
      
      WebApp.Start<StartUp>(options);
    }

    public void Stop()
    {
      
    }

  }
}
