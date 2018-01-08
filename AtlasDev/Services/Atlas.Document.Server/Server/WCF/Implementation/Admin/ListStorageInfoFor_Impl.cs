using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Xpo;
using AutoMapper;

using Atlas.DocServer.WCF.Interface;
using Atlas.Domain.Model;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  class ListStorageInfoFor_Impl
  {
    internal static StorageInfo[] Execute(ILogging log, string reference,
      GeneratorEnums.Generators generator, TemplateEnums.TemplateTypes template, Int64 sourceStorageId)
    {
      var methodName = "ListStorageInfoFor";
      log.Information("{MethodName} started: {Reference}, {Generator}, {Template}, {sourceStorageId}", methodName, reference, generator, template, sourceStorageId);

      try
      {
        if (sourceStorageId <= 0)
        {
          if (string.IsNullOrEmpty(reference) || template == TemplateEnums.TemplateTypes.NotSet)
          {
            log.Error("{MethodName}- Parameters 'reference' & 'template' cannot be empty when 'sourceStorageId' is empty", methodName);
            return null;
          }
        }

        var result = new List<StorageInfo>();

        using (var unitOfWork = new UnitOfWork())
        {
          IQueryable<DOC_FileStore> docs = null;

          #region Primary filter
          if (sourceStorageId > 0 && string.IsNullOrEmpty(reference))
          {
            docs = unitOfWork.Query<DOC_FileStore>().Where(s => s.SourceDocument.StorageId == sourceStorageId);            
          }
          else if (sourceStorageId <= 0 && !string.IsNullOrEmpty(reference))
          {
            docs = unitOfWork.Query<DOC_FileStore>().Where(s => s.Reference == reference);
          }
          else
          {
            docs = unitOfWork.Query<DOC_FileStore>().Where(s => s.SourceDocument.StorageId == sourceStorageId && s.Reference == reference);
          }

          if (docs == null || !docs.Any())
          {
            log.Information("{MethodName} completed with empty result", methodName);
            return null;
          }
          #endregion

          #region Filter data based on permutations of generator/template (ToList() used to fully materialize before re-assigment of Queryable)
          var enumTemplate = Mapper.Map<Enumerators.Document.DocumentTemplate>(template);
          var enumGenerator = Mapper.Map<Enumerators.Document.Generator>(generator);

          if (enumTemplate != Enumerators.Document.DocumentTemplate.NotSet || enumGenerator != Enumerators.Document.Generator.NotSet)
          {            
            if (enumTemplate == Enumerators.Document.DocumentTemplate.NotSet && enumGenerator != Enumerators.Document.Generator.NotSet)
            {
              docs = docs.ToList().Where(s => s.Generator == enumGenerator).AsQueryable();
            }
            else if (enumTemplate != Enumerators.Document.DocumentTemplate.NotSet && enumGenerator == Enumerators.Document.Generator.NotSet)
            {
              docs = docs.ToList().Where(s => s.SourceTemplate.TemplateType.TemplateTypeId == (int)enumTemplate).AsQueryable();
            }
            else // template is set, generator is set
            {
              docs = docs.ToList().Where(s => s.Generator == enumGenerator &&
                s.SourceTemplate.TemplateType.TemplateTypeId == (int)enumTemplate).AsQueryable();
            }

            if (docs == null || !docs.Any())
            {
              log.Information("{MethodName} returned empty result {Reference}, {Generator}, {Template}, {sourceStorageId}", methodName, reference, generator, template, sourceStorageId);          
              return null;
            }
          }         
          #endregion
       
          foreach (var docStorage in docs)
          {
            result.Add(new StorageInfo
            {
              CreateDate = docStorage.CreateDate,
              Category = docStorage.Category != null ? Mapper.Map<DocCategoryEnums.Categories>(docStorage.Category.Type) : DocCategoryEnums.Categories.NotSet,
              ClientPersonId = docStorage.Client != null ? docStorage.Client.PersonId : 0,
              Comment = docStorage.Comment,
              CreatedByPersonId = docStorage.CreatedBy != null ? docStorage.CreatedBy.PersonId : 0,
              FileFormatType = docStorage.FileFormatType != null ? Mapper.Map<FileFormatEnums.FormatType>(docStorage.FileFormatType.Type) : FileFormatEnums.FormatType.NotSet,
              Generator = Mapper.Map<GeneratorEnums.Generators>(docStorage.Generator),
              Reference = docStorage.Reference,
              Revision = docStorage.Revision,
              Size = docStorage.Size,
              SourceData = docStorage.SourceData != null ? Compression.InMemoryCompress(docStorage.SourceData) : null,
              SourceTemplateId = docStorage.SourceTemplate != null ? docStorage.SourceTemplate.TemplateId : 0,
              SourceDocumentId = docStorage.SourceDocument != null ? docStorage.SourceDocument.StorageId : 0,
              StorageId = docStorage.StorageId,
              StorageSystemRef = docStorage.StorageSystemRef
            });
          }
        }

        log.Information("{MethodName} completed: {@StorageInfo}", methodName, result);

        return result.ToArray();
      }
      catch (Exception err)
      {
        log.Error("{0}", err);
        return null;
      }
    }
  }
}