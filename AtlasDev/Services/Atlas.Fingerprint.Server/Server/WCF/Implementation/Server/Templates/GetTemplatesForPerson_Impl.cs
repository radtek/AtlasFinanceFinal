using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.Comms;
using Atlas.Common.Interface;

namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Templates
{
  public static class GetTemplatesForPerson_Impl
  {
    /// <summary>
    /// Gets all templates for a person
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="personId">Person.PersonId</param>
    /// <param name="templates">Templates for this person, null if none found</param>
    /// <param name="errorMessage">Any server error message</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    /// <tested>2012-12-11</tested>
    public static int GetTemplatesForPerson(ILogging log, SourceRequest sourceRequest, Int64 personId, out List<FPTemplateDTO> templates, out string errorMessage)
    {
      var methodName = "GetTemplatesForPerson";
      templates = null;
      errorMessage = null;

      try
      {
        //log.Information("{MethodName} starting: {@Request}, {PersonId}", methodName, sourceRequest, personId);

        #region Check request parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        if (personId <= 0)
        {
          errorMessage = "Invalid personId parameter";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        using (var unitOfWork = new UnitOfWork())
        {
          if (!unitOfWork.Query<PER_Person>().Any(s => s.PersonId == personId))
          {
            errorMessage = "PersonId does not exist";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)Enumerators.General.WCFCallResult.BadParams;
          }
        }
        #endregion

        var dbTemplates = DistCommUtils.GetTemplates(personId);
        if (dbTemplates == null || dbTemplates.Count == 0)
        {
          errorMessage = "No templates found for person";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        templates = new List<FPTemplateDTO>();
        foreach (var dbTemplate in dbTemplates)
        {
          templates.Add(new FPTemplateDTO()
          {
            FingerId = dbTemplate.Item1,
            PersonId = personId,
            TemplateBuffer = dbTemplate.Item2
          });
        }

        //log.Information("{MethodName} completed successfully", methodName);
        return (int)Enumerators.General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
        return (int)Enumerators.General.WCFCallResult.ServerError;
      }
    }

  }
}
