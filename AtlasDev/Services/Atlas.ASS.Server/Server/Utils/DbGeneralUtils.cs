using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Server.Classes;


namespace Atlas.Server.Utils
{
  /// <summary>
  /// General DB utilities
  /// </summary>
  internal static class DbGeneralUtils
  {
    /// <summary>
    /// Get specific value from [AtlasGeneral].[dbo].[Config]
    /// </summary>
    /// <param name="dataType">[DataType] field value (required)</param>
    /// <param name="entity">[DataEntity] field value (optional- branch)</param>
    /// <param name="section">[DataSection] field value (optional- use like an INI section)</param>
    /// <returns>First match in table, empty string if not found</returns>
    internal static string GetConfigValue(int dataType, string entity = null, string section = null)
    {
      var result = string.Empty;

      using (var unitOfWork = new UnitOfWork())
      {
        Domain.Model.Config config = null;

        if (!string.IsNullOrEmpty(entity) && !string.IsNullOrEmpty(section))
        {
          config = unitOfWork.Query<Domain.Model.Config>().FirstOrDefault(s => s.DataType == dataType && s.DataEntity == entity && s.DataSection == section);
        }
        else if (!string.IsNullOrEmpty(entity) && string.IsNullOrEmpty(section))
        {
          config = unitOfWork.Query<Domain.Model.Config>().FirstOrDefault(s => s.DataType == dataType && s.DataEntity == entity);
        }
        else if (string.IsNullOrEmpty(entity) && !string.IsNullOrEmpty(section))
        {
          config = unitOfWork.Query<Domain.Model.Config>().FirstOrDefault(s => s.DataType == dataType && s.DataSection == section);
        }
        else
        {
          config = unitOfWork.Query<Domain.Model.Config>().FirstOrDefault(s => s.DataType == dataType);
        }

        if (config != null && config.DataType == dataType)
        {
          result = config.DataValue;
        }
      }

      return result;
    }


    #region TCC
      
    /// <summary>
    /// Logs a TCC request
    /// </summary>
    /// <param name="terminalId">The terminal ID</param>
    /// <param name="requestType">The TCC request type</param>
    /// <param name="requestStartDT">Request started date/time</param>
    /// <param name="requestParams">Request parameters</param>
    /// <param name="requestResult">Result of the request</param>
    /// <param name="resultMessage">Message result</param>
    /// <param name="requestEndDT">Rrequest ended date/time</param>
    internal static void LogTCCRequest(long terminalId, General.TCCLogRequestType requestType, DateTime requestStartDT, string requestParams,
        General.TCCLogRequestResult requestResult, string resultMessage, DateTime requestEndDT)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var terminal = unitOfWork.Query<TCCTerminal>().FirstOrDefault(s => s.TerminalId == terminalId);       
        var requestLog = new LogTCCTerminal(unitOfWork)
        {
          StartDT = requestStartDT,
          RequestParam = requestParams,
          RequestType = requestType,
          ResultMessage = resultMessage,
          ResultType = requestResult,
          Terminal = terminal,
          EndDT = requestEndDT
        };

        unitOfWork.CommitChanges();
      }
    }

    #endregion


    #region Person/security

    internal static PersonSecurity FindByIdOrLegacyOperator(string idOrPassportOrLegacyOperatorId, UnitOfWork unitOfWork = null)
    {
      if (unitOfWork == null)
      {
        using (var uow = new UnitOfWork())
        {
          return _FindByIdOrLegacyOperator(idOrPassportOrLegacyOperatorId, uow);
        }
      }
      else
      {
        return _FindByIdOrLegacyOperator(idOrPassportOrLegacyOperatorId, unitOfWork);
      }
    }


    private static PersonSecurity _FindByIdOrLegacyOperator(string idOrPassportOrLegacyOperatorId, UnitOfWork unitOfWork)
    {
      PER_Security user = null;
      if (idOrPassportOrLegacyOperatorId.Length > 4)
      {
        user = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.Person.IdNum == idOrPassportOrLegacyOperatorId);
        if (user != null)
        {
          return Classes.Mapping.BasicMapping.MapPersonSecurity(user);
        }
      }

      user = unitOfWork.Query<PER_Security>().FirstOrDefault(s => s.LegacyOperatorId == idOrPassportOrLegacyOperatorId);
      if (user != null)
      {
        return Classes.Mapping.BasicMapping.MapPersonSecurity(user);
      }

      return null;
    }

    #endregion

  }
}
