using System;


namespace Atlas.Common.Interface
{
  public interface IConfigSettings
  {
    /// <summary>
    /// Get cache server connection string from config store
    /// </summary>
    /// <returns></returns>
    string GetCacheServerConnectionString();

    /// <summary>
    /// Get atlas_core DB Npgsql connection string from config store
    /// </summary>
    /// <returns></returns>
    string GetAtlasCoreConnectionString();


    /// <summary>
    /// Get atlas_core XPO Npgsql connection string from config store
    /// </summary>
    /// <returns></returns>
    string GetAtlasCoreXpoConnectionString();


    /// <summary>
    /// Get SMTP mail server name/IP
    /// </summary>
    /// <returns></returns>
    string GetSmtpServer();

    string GetRabbitMQServerHost();

    string GetRabbitMQVirtualHost();

    string GetRabbitMQServerUsername();

    string GetRabbitMQServerPassword();


    /// <summary>
    /// Get ass DB Npgsql connection string from config store
    /// </summary>
    /// <returns></returns>
    string GetAssConnectionString();


    /// <summary>
    /// Get custom application setting from config store
    /// </summary>
    /// <param name="entity">Entity (leave blank if want to use appname)</param>
    /// <param name="section">The specific section</param>
    /// <param name="useAppNameForEntity">Use main entry assembly filename instead of 'entity'</param>
    /// <returns></returns>
    string GetCustomSetting(string entity, string section, bool useAppNameForEntity);
    
    int GetCustomSettingInt(string entity, string section, bool useAppNameForEntity, int defaultVal);

    decimal GetCustomSettingDecimal(string entity, string section, bool useAppNameForEntity, decimal defaultVal);
    
    bool GetCustomSettingBool(string entity, string section, bool useAppNameForEntity, bool defaultVal);
    
  }
}
