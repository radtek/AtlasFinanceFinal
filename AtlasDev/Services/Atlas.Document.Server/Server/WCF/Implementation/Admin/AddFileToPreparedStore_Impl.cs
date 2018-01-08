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
  internal class AddFileToPreparedStore_Impl
  {
    internal static void Execute(ILogging log, StorageInfo info, byte[] document, bool documentIsCompressed)
    {
      var methodName = "AddFileToPreparedStore";
      log.Information("{MethodName} started: {DocumentLength} bytes", methodName, document != null ? document.Length : 0, info);

      #region Check parameters
      if (info == null)
      {
        log.Error("{MethodName}- parameter 'info' not specified", methodName);
        return;
      }

      if (info.StorageId <= 0)
      {
        log.Error("{MethodName}- parameter 'storageId' not specified", methodName);
        return;
      }

      if (document == null || document.Length == 0)
      {
        log.Error("{MethodName}- parameter 'document' not specified", methodName);
        return;
      }
      #endregion

      var timer = Stopwatch.StartNew();

      try
      {
        using (var unitOfWork = new UnitOfWork())
        {
          var docStorage = unitOfWork.Query<DOC_FileStore>().FirstOrDefault(s => s.StorageId == info.StorageId);
          if (docStorage == null)
          {
            log.Error("{MethodName}- failed to locate 'StorageId': {StorageId}", methodName, info.StorageId);
            return;
          }
          
          if (docStorage.StorageSystemRef != null && docStorage.StorageSystemRef.Length > 0)
          {
            log.Error("{MethodName}- row is already linked to a document storage item: {StorageId}", methodName, info.StorageId);
            return;
          }
                    
          if (info.ClientPersonId > 0)
          {
            docStorage.Client = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == info.ClientPersonId);
          }

          if (info.CreatedByPersonId > 0)
          {
            docStorage.CreatedBy = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == info.CreatedByPersonId);
          }

          if (!string.IsNullOrEmpty(info.Comment))
          {
            docStorage.Comment = info.Comment;
          }
          if (info.CreateDate > DateTime.MinValue)
          {
            docStorage.CreateDate = info.CreateDate;
          }

          if (info.FileFormatType != FileFormatEnums.FormatType.NotSet)
          {
            var fileFormat = Mapper.Map<Enumerators.Document.FileFormat>(info.FileFormatType);
            docStorage.FileFormatType = unitOfWork.Query<DOC_FileFormatType>().FirstOrDefault(s => s.Type == fileFormat);
          }

          if (info.Generator != GeneratorEnums.Generators.NotSet)
          {
            var generator = Mapper.Map<Enumerators.Document.Generator>(info.Generator);
            docStorage.Generator = generator;
          }

          if (documentIsCompressed)
          {
            document = Compression.InMemoryDecompress(document);
          }
          if (string.IsNullOrEmpty(info.Hash))
          {
            info.Hash = Utils.DocUtils.CalcSHA512Hash(info.Reference ?? docStorage.Reference ?? info.StorageId.ToString(), document);
          }          
          docStorage.Hash = info.Hash;

          if (!string.IsNullOrEmpty(info.Keywords))
          {
            docStorage.Keywords = info.Keywords;
          }

          if (!string.IsNullOrEmpty(info.Reference))
          {
            docStorage.Reference = info.Reference;
          }

          if (info.Revision > 0)
          {
            docStorage.Revision = info.Revision;
          }

          docStorage.Size = document.Length;
          docStorage.StorageSystemRef = Utils.MongoStoreUtils.AddDocumentToStore(document, false);

          unitOfWork.CommitChanges();
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      log.Information("{MethodName}- time: {ElapsedMS}ms", methodName, timer.ElapsedMilliseconds);

    }
  }
}
