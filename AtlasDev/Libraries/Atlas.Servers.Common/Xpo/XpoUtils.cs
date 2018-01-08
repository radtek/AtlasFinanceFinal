using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;
using DevExpress.Xpo.DB;

using Atlas.Common.Interface;


namespace Atlas.Servers.Common.Xpo
{
  public static class XpoUtils
  {
    /// <summary>
    /// Create the XPO domain
    /// </summary>
    /// <param name="log"></param>
    /// <param name="xpoConnectionStringName"></param>
    public static void CreateXpoDomain(IConfigSettings config, ILogging log, IEnumerable<System.Type> types = null)
    {
      log.Information("Attempting to validate and build/load domain...");

      var connStr = config.GetAtlasCoreXpoConnectionString() ?? config.GetCustomSetting(null, "XPO.NPGSQL.ATLAS_CORE", true);
      var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
      using (var dataLayer = new SimpleDataLayer(dataStore))
      {
        using (var session = new Session(dataLayer))
        {
          if (types != null)
          {
            var assemblies = types.Select(s => s.Assembly).Distinct().ToArray();
            session.UpdateSchema(assemblies);
            session.UpdateSchema(types.ToArray());
          }

          XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
        }
      }
      XpoDefault.Session = null;

      log.Information("Successfully loaded and validated domain");
    }

  }
}
