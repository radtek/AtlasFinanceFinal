using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Common.Interface;
using Atlas.DocServer.WCF.Interface;
using Atlas.Domain.Model;


namespace Atlas.DocServer.WCF.Implementation.Generator
{
  internal class GetTemplateById_Impl
  {
    internal static DocTemplate Execute(ILogging log, Int64 templateStoreId)
    {
      var methodName = "Execute";
      DocTemplate result = null;
      try
      {
        log.Information("{MethodName} started, {templateStoreId}", methodName, templateStoreId);

        #region Check params
        if (templateStoreId <= 0)
        {
          log.Error(new ArgumentNullException("templateStoreId"), methodName);
          return null;
        }
        #endregion

        using (var unitOfWork = new UnitOfWork())
        {
          result = AutoMapper.Mapper.Map<DocTemplate>(unitOfWork.Query<DOC_TemplateStore>().FirstOrDefault(s => s.TemplateId == templateStoreId));
        }

        if (result == null)
        {
          log.Error("{MethodName} failed to locate template: {templateStoreId}", methodName, templateStoreId);
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);        
      }

      return result;
    }
  }
}
