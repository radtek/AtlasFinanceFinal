using System;
using System.Diagnostics;
using System.Linq;

using DevExpress.Xpo;
using AutoMapper;

using Atlas.Domain.Model;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  class StoreFile_Impl
  {
    internal static long Execute(ILogging log, StorageInfo info, byte[] document, 
      bool documentIsCompressed, byte[] sourceData, bool sourceDataIsCompressed)
    {
      var methodName = "StoreFile";
      log.Information("{MethodName} started: {DocumentLength} bytes, {@StorageInfo}", methodName, document != null ? document.Length : 0, info);

      if (info == null)
      {
        log.Error("{MethodName}- parameter 'info' not specified", methodName);
        return 0;
      }

      if (info.FileFormatType == FileFormatEnums.FormatType.NotSet)
      {
        log.Error("{MethodName} parameter 'fileFormat' not specified", methodName);
        return 0;
      }

      if (document == null || document.Length == 0)
      {
        log.Error("{MethodName} parameter 'document' not specified or empty", methodName);
        return 0;
      }

      var timer = Stopwatch.StartNew();

      var category = Mapper.Map<Enumerators.Document.Category>(info.Category);
      var fileFormat = Mapper.Map<Enumerators.Document.FileFormat>(info.FileFormatType);
      var generator = Mapper.Map<Enumerators.Document.Generator>(info.Generator);
      Int64 result = 0;
      if (documentIsCompressed)
      {
        document = Compression.InMemoryDecompress(document);
      }
      if (string.IsNullOrEmpty(info.Hash))
      {
        info.Hash = Utils.DocUtils.CalcSHA512Hash(info.Reference, document);
      }      
      try
      {
        using (var unitOfWork = new UnitOfWork())
        {
          var docStorage = new DOC_FileStore(unitOfWork)
          {
            Category = unitOfWork.Query<DOC_Category>().FirstOrDefault(s => s.Type == category),
            Client = info.ClientPersonId > 0 ? unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == info.ClientPersonId) : null,
            CreatedBy = info.CreatedByPersonId > 0 ? unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == info.CreatedByPersonId) : null,
            Comment = info.Comment,
            CreateDate = info.CreateDate,
            FileFormatType = unitOfWork.Query<DOC_FileFormatType>().FirstOrDefault(s => s.Type == fileFormat),
            Generator = generator,
            Hash = info.Hash,
            Keywords = info.Keywords,
            Reference = info.Reference,
            Revision = info.Revision,
            Size = document.Length,
            SourceData = sourceDataIsCompressed ? sourceData : sourceData != null ? Compression.InMemoryCompress(sourceData) : null,
            SourceTemplate = info.SourceTemplateId > 0 ? unitOfWork.Query<DOC_TemplateStore>().FirstOrDefault(s => s.TemplateId == info.SourceTemplateId) : null,
            StorageSystemRef = Utils.MongoStoreUtils.AddDocumentToStore(document, false)
          };
                    
          unitOfWork.CommitChanges();
          result = docStorage.StorageId;
        }
      }
      catch (Exception err)
      {
        log.Error("{0}", err);
      }

      log.Information("{MethodName} completed- result: {StorageId}, time: {ElapsedMS}ms", methodName, result, timer.ElapsedMilliseconds);

      return result;
    }
  }
}
