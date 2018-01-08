using System;
using System.IO;

using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  internal static class ChunkGetFileSize_Impl
  {
    internal static Int64 Execute(ILogging log, string fileCorrelationId)
    {
      #region Check parameters
      Guid guid;
      if (string.IsNullOrEmpty(fileCorrelationId) || !Guid.TryParse(fileCorrelationId, out guid))
      {
        log.Error("Invalid value for parameter 'fileCorrelationId': {fileCorrelationId}", fileCorrelationId);
        return -1;
      }    
      #endregion

      var file = Utils.ChunkedFileUtils.GetChunkedFileName(fileCorrelationId);
      var info = new FileInfo(file);
      return info.Exists ? info.Length : 0;
    }
  }
}
