using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

using DevExpress.Xpo;

using Atlas.DocServer.WCF.Interface;
using Atlas.Domain.Model;
using Atlas.DocServer.Utils;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  internal static class StoreScannedFile_Impl
  {
    public static List<StorageInfo> Execute(ILogging log, byte[] data, FileFormatEnums.FormatType fileFormat, bool documentIsCompressed)
    {
      var result = new List<StorageInfo>();

      var methodName = "StoreScannedFile";
      log.Information("{MethodName} started: {DocumentLength} bytes, {FileFormat}, {DocumentIsCompressed}", 
        methodName, data != null ? data.Length : 0, fileFormat, documentIsCompressed);

      var timer = Stopwatch.StartNew();

      try
      {
        if (fileFormat == FileFormatEnums.FormatType.NotSet)
        {
          log.Error("Parameter 'fileFormat' not set");
          return null;
        }

        if (fileFormat != FileFormatEnums.FormatType.PDF && fileFormat != FileFormatEnums.FormatType.TIFF)
        {
          log.Error("Parameter fileFormat contained invalid value: {fileFormat}- only TIFF and PDF supported", fileFormat);
          return null;
        }

        if (data == null || data.Length == 0)
        {
          log.Error("Parameter 'data' not set");
          return null;
        }

        if (documentIsCompressed)
        {
          data = Compression.InMemoryDecompress(data);
        }

        #region Convert the TIFF to an quivalent PDF
        if (fileFormat == FileFormatEnums.FormatType.TIFF)
        {
          using (var source = new MemoryStream(data))
          {
            using (var pdf = Utils.GdPictureUtils.ImageToPdf(source, Enumerators.Document.FileFormat.TIFF, Utils.DocUtils.DocOptionsGetNotNull(null), true))
            {
              data = pdf.StreamToByte();
            }
          }
        }
        #endregion

        // Extract documents in correct order
        List<Tuple<int, int, string, Int64>> extractedFiles = null;
        using (var ms = new MemoryStream(data))
        {
          extractedFiles = Utils.DataMatrixDocUtils.ScannedPDFOrderDocuments(ms);
        }

        if (extractedFiles != null && extractedFiles.Count > 0)
        {
          using (var unitOfWork = new UnitOfWork())
          {
            var added = new List<DOC_FileStore>();
            var pdfFileFormat = unitOfWork.Query<DOC_FileFormatType>().First(s => s.FileFormatTypeId == (int)Atlas.Enumerators.Document.FileFormat.PDF);
            foreach (var extractedFile in extractedFiles)
            {
              var expectedPages = extractedFile.Item1;
              var actualPages = extractedFile.Item2;
              var fileName = extractedFile.Item3;
              var sourceStorageId = extractedFile.Item4;

              if (expectedPages == actualPages)
              {
                var fileInfo = new FileInfo(fileName);

                #region Store in DOC_FileStore/mongo
                var sourceDoc = unitOfWork.Query<DOC_FileStore>().FirstOrDefault(s => s.StorageId == sourceStorageId);
                if (sourceDoc != null)
                {
                  var docBytes = File.ReadAllBytes(fileName);
                  added.Add(new DOC_FileStore(unitOfWork)
                  {
                    Category = sourceDoc.Category,
                    Client = sourceDoc.Client,
                    Comment = sourceDoc.Comment,
                    CreateDate = DateTime.Now,
                    CreatedBy = sourceDoc.CreatedBy,
                    FileFormatType = pdfFileFormat,
                    Generator = Enumerators.Document.Generator.Scanner,
                    Hash = Utils.DocUtils.CalcSHA512Hash(sourceDoc.Reference ?? sourceStorageId.ToString(), docBytes),
                    Keywords = sourceDoc.Keywords,
                    Reference = sourceDoc.Reference,
                    Revision = sourceDoc.Revision,
                    Size = (int)fileInfo.Length,
                    SourceData = sourceDoc.SourceData,
                    SourceDocument = sourceDoc,
                    SourceTemplate = sourceDoc.SourceTemplate,
                    StorageSystemRef = Utils.MongoStoreUtils.AddDocumentToStore(docBytes, false)
                  });
                }
                else
                {
                  log.Error("Failed to locate source document {sourceStorageId}", sourceStorageId);
                }
                #endregion
              }
              else
              {
                log.Warning("Document {StorageId} contains incorrect number of pages. Counted: {CountedPages}, Expected: {ExpectedPages}- file was skipped",
                  extractedFile.Item4, actualPages, expectedPages);
              }

              File.Delete(extractedFile.Item3);
            }

            unitOfWork.CommitChanges();
            foreach (var doc in added)
            {
              result.Add(AutoMapper.Mapper.Map<StorageInfo>(doc));
            }
          }
        }

        log.Information("{MethodName} completed- result: {@StorageInfo}, time: {ElapsedMS}ms", methodName, result, timer.ElapsedMilliseconds);

        return result;
      }
      catch (Exception err)
      {
        log.Error(err, "StoreScannedFile_Impl");
        return null;
      }
    }

  }
}
