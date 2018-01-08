using System;
using System.Collections.Generic;
using System.IO;

using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  internal static class StoreScannedFileChunked_Impl
  {
    public static List<StorageInfo> Execute(ILogging log, string fileCorrelationId, FileFormatEnums.FormatType fileFormat, bool documentIsCompressed)
    {
      var methodName = "StoreScannedFileChunked";
      log.Information("{MethodName} started: {FileCorrelationId}, {FileFormat}, {DocumentIsCompressed}", 
        methodName, fileCorrelationId, fileFormat, documentIsCompressed);

      #region Check parameters
      Guid guid;
      if (string.IsNullOrEmpty(fileCorrelationId) || !Guid.TryParse(fileCorrelationId, out guid))
      {
        log.Error("Invalid value for parameter 'fileCorrelationId': {fileCorrelationId}", fileCorrelationId);
        return null;
      }

      if (fileFormat == FileFormatEnums.FormatType.NotSet)
      {
        log.Error("Invalid value for parameter 'fileFormat': {fileFormat}", fileFormat);
        return null;
      }

      var fileName = Utils.ChunkedFileUtils.GetChunkedFileName(fileCorrelationId);
      var fileInfo = new FileInfo(fileName);
      if (!fileInfo.Exists)
      {
        log.Error("Unable to locate chunked file: '{fileName}' --> {fileCorrelationId}", fileName, fileCorrelationId);
        return null;
      }

      if (fileInfo.Length == 0)
      {
        File.Delete(fileName);
        log.Error("Chunked file is empty: '{fileName}'", fileName);
        return null;
      }      
      #endregion
      
      try
      {
        var result = StoreScannedFile_Impl.Execute(log, File.ReadAllBytes(fileName), fileFormat, documentIsCompressed);
        if (result != null)
        {
          File.Delete(fileName);
        }

        return result;
      }
      catch (Exception err)
      {
        log.Error(err, "StoreScannedFileChunked_Impl");
        return null;
      }
    }
  }
}
