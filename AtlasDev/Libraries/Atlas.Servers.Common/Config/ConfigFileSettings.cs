using System;
using System.Configuration;
using System.Linq;
using System.Globalization;

using DevExpress.Xpo;

using Atlas.Common.Interface;


namespace Atlas.Servers.Common.Config
{
  /// <summary>
  /// Use config file as primary source of configuration, with Config table as backup...
  /// </summary>
  public class ConfigFileSettings : IConfigSettings
  {
    public string GetAssConnectionString()
    {
      var name = "Ass_NpgSql";
      var conn = ConfigurationManager.ConnectionStrings[name];
      return conn != null ? conn.ConnectionString : "Server=172.31.91.165;Database=ass;User Id=postgres;Password=s1DT81ChqlVkPZMlRO8b";
    }


    /// <summary>
    /// NpgSql connection string for atlas_core database
    /// </summary>
    /// <returns></returns>
    public string GetAtlasCoreConnectionString()
    {
      var name = "Atlas_Core_NpgSql";
      var conn = ConfigurationManager.ConnectionStrings[name];
      return conn != null ? conn.ConnectionString : "Server=172.31.91.165;Database=atlas_core;User Id=postgres;Password=s1DT81ChqlVkPZMlRO8b";
    }


    public string GetAtlasCoreXpoConnectionString()
    {
      var name = "Atlas_Core_Xpo";
      var conn = ConfigurationManager.ConnectionStrings[name];
      return conn != null ? conn.ConnectionString : "XpoProvider=Postgres;Server=172.31.91.165;Database=atlas_core;User Id=postgres;Password=s1DT81ChqlVkPZMlRO8b";
    }


    /// <summary>
    /// Redis cache server(s)
    /// </summary>
    /// <returns></returns>
    public string GetCacheServerConnectionString()
    {
      var name = "CacheServer";
      var conn = ConfigurationManager.ConnectionStrings[name];
      return conn != null ? conn.ConnectionString : "localhost";
    }

    
    /// <summary>
    /// Get custom setting from config file/'Config' table
    /// </summary>
    /// <param name="entity">Entity (leave blank if want to use appname or the local .config file)</param>
    /// <param name="section">The specific section (required)</param>
    /// <param name="useAppNameForEntity">Use main entry assembly filename (no extension, i.e. FPServer) in place of the 'entity' parameter. 
    /// false to use entity parameter as is</param>
    /// <returns>string containing found config value, null if none found</returns>
    public string GetCustomSetting(string entity, string section, bool useAppNameForEntity)
    {
      if (string.IsNullOrEmpty(entity) && useAppNameForEntity)
      {
        entity = _appName.Value;
      }

      if (string.IsNullOrEmpty(entity)) // no entity- try .config settings
      {
        var value = ConfigurationManager.AppSettings[section];
        if (value != null)
        {
          return value;
        }
      }

      using (var uow = new UnitOfWork())
      {
        // Try search for both entity/appname and section to match
        var setting = uow.Query<Domain.Model.Config>().FirstOrDefault(s => s.DataEntity == entity && s.DataSection == section);
        
        // Search only on section
        if (setting == null)
        {
          setting = uow.Query<Domain.Model.Config>().FirstOrDefault(s => s.DataSection == section);
        }

        // Search only on entity(appname)
        if (setting == null)
        {
          setting = uow.Query<Domain.Model.Config>().FirstOrDefault(s => s.DataEntity == _appName.Value);          
        }

        return setting?.DataValue;
      }
    }


    /// <summary>
    /// Get integer value for app custom setting from the config system
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="section"></param>
    /// <param name="useAppNameForEntity"></param>
    /// <param name="defaultVal">Value to use cannot find or parse value</param>
    /// <returns></returns>
    public int GetCustomSettingInt(string entity, string section, bool useAppNameForEntity, int defaultVal)
    {
      var result = GetCustomSetting(entity, section, useAppNameForEntity);
      if (!string.IsNullOrEmpty(result))
      {
        int val;
        if (int.TryParse(result, NumberStyles.None, CultureInfo.InvariantCulture, out val))
        {
          return val;
        }
      }

      return defaultVal;
    }


    public decimal GetCustomSettingDecimal(string entity, string section, bool useAppNameForEntity, decimal defaultVal)
    {
      var result = GetCustomSetting(entity, section, useAppNameForEntity);
      if (!string.IsNullOrEmpty(result))
      {
        decimal val;
        if (decimal.TryParse(result, NumberStyles.None, CultureInfo.InvariantCulture, out val))
        {
          return val;
        }
      }

      return defaultVal;
    }


    public bool GetCustomSettingBool(string entity, string section, bool useAppNameForEntity, bool defaultVal)
    {
      var result = GetCustomSetting(entity, section, useAppNameForEntity);
      if (!string.IsNullOrEmpty(result))
      {        
        // Handles case insensitive, whitespace ignored: True/False/1/0/-1/T/F/Yes/No...
        if (BoolParser.IsFalse(result) || BoolParser.IsTrue(result))
        {
          return BoolParser.IsTrue(result);
        }       
      }

      return defaultVal;
    }


    public string GetSmtpServer()
    {
      var name = "SmtpServerAddress";
      var conn = ConfigurationManager.AppSettings[name];
      return conn ?? "mail.atcorp.co.za";
    }


    public string GetRabbitMQServerHost()
    {
      var name = "rabbitmq.host";
      var conn = ConfigurationManager.AppSettings[name];
      return conn ?? "172.31.91.165";
    }

    public string GetRabbitMQVirtualHost()
    {
      var name = "rabbitmq.vhost";
      var conn = ConfigurationManager.AppSettings[name];
      return conn ?? "test";
    }

    public string GetRabbitMQServerUsername()
    {
      var name = "rabbitmq.username";
      var conn = ConfigurationManager.AppSettings[name];
      return conn ?? "service";
    }

    public string GetRabbitMQServerPassword()
    {
      var name = "rabbitmq.password";
      var conn = ConfigurationManager.AppSettings[name];
      return conn ?? "lkLS2E4egTMV7FZrMUkl";
    }

   
    /// <summary>
    /// Entry assembly name only
    /// </summary>
    private static readonly Lazy<string> _appName =
      new Lazy<string>(() => System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetAssembly(typeof(ConfigFileSettings)).Location));
    
  }

}
