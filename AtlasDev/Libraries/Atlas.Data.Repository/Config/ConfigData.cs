#region Using

using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;

#endregion


namespace Atlas.Data.Repository
{
  public static class ConfigData
  {    
    /// <summary>
    /// Get specific value from [AtlasGeneral].[dbo].[Config]
    /// </summary>
    /// <param name="dataType">[DataType] field value (required)</param>
    /// <param name="entity">[DataEntity] field value (optional- branch)</param>
    /// <param name="section">[DataSection] field value (optional- use like an INI section)</param>
    /// <returns>First match in table, empty string if not found</returns>
    public static string GetConfigValue(int dataType, string entity = null, string section = null)
    {
      var result = string.Empty;

      using (var unitOfWork = new UnitOfWork())
      {
        Config config = null;

        if (!string.IsNullOrEmpty(entity) && !string.IsNullOrEmpty(section))
        {
          config = unitOfWork.Query<Config>().FirstOrDefault(s => s.DataType == dataType && s.DataEntity == entity && s.DataSection == section);
        }
        else if (!string.IsNullOrEmpty(entity) && string.IsNullOrEmpty(section))
        {
          config = unitOfWork.Query<Config>().FirstOrDefault(s => s.DataType == dataType && s.DataEntity == entity);
        }
        else if (string.IsNullOrEmpty(entity) && !string.IsNullOrEmpty(section))
        {
          config = unitOfWork.Query<Config>().FirstOrDefault(s => s.DataType == dataType && s.DataSection == section);
        }
        else
        {
          config = unitOfWork.Query<Config>().FirstOrDefault(s => s.DataType == dataType);
        }

        if (config != null && config.DataType == dataType)
        {
          result = config.DataValue;
        }
      }

      return result;
    }
     
  }
}
