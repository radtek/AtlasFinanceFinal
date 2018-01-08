using System;
using System.Diagnostics;
using System.Linq;

using DevExpress.Xpo;
using AutoMapper;

using Atlas.DocServer.WCF.Interface;
using Atlas.Domain.Model;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  internal class GetStorageInfo_Impl
  {
    internal static StorageInfo Execute(ILogging log, long storageId)
    {
      var methodName = "GetStorageInfo";
      log.Information("{MethodName} started: {StorageId}", methodName, storageId);

      if (storageId <= 0)
      {
        log.Warning("{MethodName}- 'storageId' parameter invalid", methodName);
        return null;
      }

      var timer = Stopwatch.StartNew();

      StorageInfo result = null;
      try
      {
        using (var unitOfWork = new UnitOfWork())
        {
          var docStorage = unitOfWork.Query<DOC_FileStore>().FirstOrDefault(s => s.StorageId == storageId);
          if (docStorage != null)
          {
            result = new StorageInfo()
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
              StorageId = docStorage.StorageId,
              StorageSystemRef = docStorage.StorageSystemRef
            };
          }
          else
          {
            log.Warning("{MethodName}- 'StorageId': {@StorageId} could not be located in database", methodName, storageId);
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      log.Information("{MethodName}- result: {@StorageInfo}, time: {ElapsedMS}ms", methodName, result, timer.ElapsedMilliseconds);
      return result;
    }
  }
}
