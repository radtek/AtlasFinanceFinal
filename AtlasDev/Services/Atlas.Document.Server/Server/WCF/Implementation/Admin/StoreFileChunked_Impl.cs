using System;
using System.IO;

using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  internal static class StoreFileChunked_Impl
  {
    internal static Int64 Execute(ILogging log, StorageInfo info, string fileCorrelationId, bool documentIsCompressed, byte[] sourceData, bool sourceDataIsCompressed)
    {
      var methodName = "StoreFileChunked";
      log.Information("{MethodName} started: {@StorageInfo}", methodName, info);

      #region Check parameters
      if (info == null)
      {
        return -1;
      }
            
      Guid guid;
      if (string.IsNullOrEmpty(fileCorrelationId) || !Guid.TryParse(fileCorrelationId, out guid))
      {
        log.Error("Invalid value for parameter 'fileCorrelationId': {fileCorrelationId}", fileCorrelationId);
        return -1;
      }

      var fileName = Utils.ChunkedFileUtils.GetChunkedFileName(fileCorrelationId);
      var fileInfo = new FileInfo(fileName);
      if (!fileInfo.Exists)
      {
        log.Error("Unable to locate chunked file: '{fileName}' --> {fileCorrelationId}", fileName, fileCorrelationId);
        return -1;
      }

      if (fileInfo.Length == 0)
      {
        File.Delete(fileName);
        log.Error("Chunked file is empty: '{fileName}'", fileName);
        return -1;
      }

      if (info.FileFormatType == FileFormatEnums.FormatType.NotSet)
      {
        log.Error("Invalid value for parameter 'fileFormat': {fileFormat}", info.FileFormatType);
        return -1;
      }
      #endregion
      
      return StoreFile_Impl.Execute(log, info, File.ReadAllBytes(fileName), documentIsCompressed, sourceData, sourceDataIsCompressed);
    }
  }

}
