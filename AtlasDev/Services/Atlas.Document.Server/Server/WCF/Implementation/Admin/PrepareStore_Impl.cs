using System;
using System.Diagnostics;
using System.Linq;

using AutoMapper;
using DevExpress.Xpo;

using Atlas.Common.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Domain.Model;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  class PrepareStore_Impl
  {
    internal static long Execute(ILogging log, StorageInfo info, byte[] sourceData, bool sourceDataIsCompressed)
    {
      var methodName = "PrepareStore";
      log.Information("{MethodName} started: {@StorageInfo}", methodName, info);

      #region Check parameters
      if (info == null)
      {
        log.Error("{MethodName}- parameter 'info' not specified", methodName);
        return -1;
      }
      if (string.IsNullOrEmpty(info.Reference))
      {
        log.Error("{MethodName}- parameter 'info.Reference' not specified", methodName);
        return -1;
      }

      if (info.Generator == GeneratorEnums.Generators.NotSet)
      {
        log.Error("{MethodName}- parameter 'info.Generator' not specified", methodName);
        return -1;
      }

      if (info.SourceTemplateId <= 0 && info.Category == DocCategoryEnums.Categories.NotSet)
      {
        log.Error("{MethodName}- parameters 'info.SourceTemplateId' and 'info.Category' not specified- at least one must be specified", methodName);
        return -1;
      }

      if (info.FileFormatType == FileFormatEnums.FormatType.NotSet)
      {
        log.Error("{MethodName}- parameter 'info.FileFormatType' not specified", methodName);
        return -1;
      }
      
      #endregion

      if (info.CreateDate == DateTime.MinValue)
      {
        info.CreateDate = DateTime.Now;
      }

      var timer = Stopwatch.StartNew();

      Int64 result = 0;
      try
      {
        var fileFormat = Mapper.Map<Enumerators.Document.FileFormat>(info.FileFormatType);
        using (var unitOfWork = new UnitOfWork())
        {
          var generator = Mapper.Map<Enumerators.Document.Generator>(info.Generator);
          var sourceTemplate = info.SourceTemplateId > 0 ?
            unitOfWork.Query<DOC_TemplateStore>().FirstOrDefault(s => s.TemplateId == info.SourceTemplateId) : null;
          
          var docStorage = new DOC_FileStore(unitOfWork)
          {
            Category = sourceTemplate != null ? sourceTemplate.TemplateType.Category : 
               unitOfWork.Query<DOC_Category>().FirstOrDefault(s => s.Type == Mapper.Map<Enumerators.Document.Category>(info.Category)),
            Client = info.ClientPersonId > 0 ? unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == info.ClientPersonId) : null,
            CreatedBy = info.CreatedByPersonId > 0 ? unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == info.CreatedByPersonId) : null,
            Comment = info.Comment,
            CreateDate = info.CreateDate,
            FileFormatType = unitOfWork.Query<DOC_FileFormatType>().First(s => s.Type == fileFormat),
            Generator = generator,
            Keywords = info.Keywords,
            Reference = info.Reference,
            Revision = info.Revision,
            SourceData = sourceDataIsCompressed ? sourceData : sourceData != null ? Compression.InMemoryCompress(sourceData) : null,
            SourceTemplate = sourceTemplate,
            SourceDocument = info.SourceDocumentId > 0 ? unitOfWork.Query<DOC_FileStore>().FirstOrDefault(s => s.StorageId == info.SourceDocumentId): null            
          };

          unitOfWork.CommitChanges();
          result = docStorage.StorageId;
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      log.Information("{MethodName}- result: {StorageId}, time: {ElapsedMS}ms", methodName, result, timer.ElapsedMilliseconds);

      return result;
    }
  }
}
