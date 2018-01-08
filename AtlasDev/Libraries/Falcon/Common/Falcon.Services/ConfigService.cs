using Falcon.Common.Interfaces.Services;
using System.Configuration;

namespace Falcon.Common.Services
{
  public class ConfigService : IConfigService
  {
    private readonly string _atlasCoreConnectionString =
      ConfigurationManager.ConnectionStrings["DefaultConnection"] != null
        ? ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString.Replace("XpoProvider=Postgres;",
          "")
        : string.Empty;

    private readonly string _assConnectionString =
      ConfigurationManager.ConnectionStrings["AssConnection"] != null
        ? ConfigurationManager.ConnectionStrings["AssConnection"].ConnectionString
        : string.Empty;

    public string AssConnection
    {
      get { return _assConnectionString; }
    }

    public string AtlasCoreConnection
    {
      get { return _atlasCoreConnectionString; }
    }
  }
}