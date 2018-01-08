using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Configuration
{
  public class AtlasConfig
  {
    protected static AtlasOnlineConfigurationSection _config =
        ConfigurationManager.GetSection("atlasOnline") as AtlasOnlineConfigurationSection;

    public static AtlasOnlineConfigurationCollection Application
    {
      get { return _config.Application; }
    }

    public static AtlasOnlineConfigurationCollection General
    {
      get { return _config.General; }
    }
  }
}