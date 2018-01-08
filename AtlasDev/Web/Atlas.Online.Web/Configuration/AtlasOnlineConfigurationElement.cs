using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Configuration
{
  public class AtlasOnlineSettingsElement : ConfigurationElement
  {
    [ConfigurationProperty("key", IsRequired=true)]
    public string Key { get { return this["key"].ToString(); } set { this["key"] = value; } }

    [ConfigurationProperty("value", IsRequired = true)]
    public string Value { get { return this["value"].ToString(); } set { this["value"] = value; } }
  }

  [ConfigurationCollection(typeof(AtlasOnlineSettingsElement))]
  public class AtlasOnlineConfigurationCollection : ConfigurationElementCollection
  {
    public new string this[string name]
    {
      get
      {
        foreach (AtlasOnlineSettingsElement item in this)
        {
          if (item.Key.Equals(name)) 
          {
            return item.Value;
          }
        }

        return null;
      }
    }

    protected override ConfigurationElement CreateNewElement()
    {      
      return new AtlasOnlineSettingsElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((AtlasOnlineSettingsElement)element).Key;
    }
  }

  public class AtlasOnlineConfigurationSection : ConfigurationSection
  {
    [ConfigurationProperty("application", IsDefaultCollection = true)]
    public AtlasOnlineConfigurationCollection Application
    {
      get
      {
        return (AtlasOnlineConfigurationCollection)this["application"];
      }
      set
      {
        this["application"] = value;
      }
    }

    [ConfigurationProperty("general", IsDefaultCollection = true)]
    public AtlasOnlineConfigurationCollection General
    {
      get
      {
        return (AtlasOnlineConfigurationCollection)this["general"];
      }
      set
      {
        this["general"] = value;
      }
    }
  }
}