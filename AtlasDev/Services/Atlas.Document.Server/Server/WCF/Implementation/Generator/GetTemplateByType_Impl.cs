using System;
using System.Linq;

using AutoMapper;
using DevExpress.Xpo;

using Atlas.Common.Interface;
using Atlas.DocServer.WCF.Interface;
using Atlas.Domain.Model;


namespace Atlas.DocServer.WCF.Implementation.Generator
{
  internal class GetTemplateByType_Impl
  {
    internal static DocTemplate Execute(ILogging log, TemplateEnums.TemplateTypes templateType, int revision,
      LanguageEnums.Language language, bool wantTemplateFileBytes)
    {
      var methodName = "Execute";
      DocTemplate result = null;

      try
      {
        log.Information("{MethodName} started, {templateType}, {Revision}, {Language}, {WwantTemplateFileBytes} ", methodName, templateType, revision,
          language, wantTemplateFileBytes);

        #region Check params
        if (templateType == TemplateEnums.TemplateTypes.NotSet)
        {
          log.Error(new ArgumentNullException("templateType"), methodName);
          return null;
        }        
        #endregion

        using (var unitOfWork = new UnitOfWork())
        {
          var templateTypeDb = unitOfWork.Query<DOC_TemplateType>().First(s => s.Type == Mapper.Map<Enumerators.Document.DocumentTemplate>(templateType));
          var languageDb = unitOfWork.Query<LNG_Language>().FirstOrDefault(s => s.LanguageId == (int)language);
          DOC_TemplateStore templateStore = null;

          if (revision <= 0 && language == LanguageEnums.Language.NotSet)
          { // No, revision, no language- get most recent
            templateStore = unitOfWork.Query<DOC_TemplateStore>()
              .Where(s => s.TemplateType == templateTypeDb)
              .OrderByDescending(s => s.Revision)
              .ThenBy(s => s.Language)
              .FirstOrDefault();
          }
          else if (revision > 0 && language == LanguageEnums.Language.NotSet)
          { // Specific revision, default language
            templateStore = unitOfWork.Query<DOC_TemplateStore>()
              .Where(s => s.TemplateType == templateTypeDb && s.Revision == revision)
              .OrderBy(s => s.Language)
              .FirstOrDefault();
          }
          else
          { // Specific revision and language
            templateStore = unitOfWork.Query<DOC_TemplateStore>()
              .Where(s => s.TemplateType == templateTypeDb && s.Revision == revision && s.Language == languageDb)
              .FirstOrDefault();
          }

          if (templateStore != null)
          {
            result = Mapper.Map<DocTemplate>(templateStore);
            if (!wantTemplateFileBytes)
            {
              result.FileBytes = null;
            }
          }
          else
          {
            log.Error("{MethodName} failed to locate a matching template: {templateType}, {Revision}, {Language}, {WwantTemplateFileBytes} ", 
              methodName, templateType, revision, language, wantTemplateFileBytes);
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      log.Information("{MethodName} result: {@Template}", methodName, result);

      return result;
    }
  }
}
