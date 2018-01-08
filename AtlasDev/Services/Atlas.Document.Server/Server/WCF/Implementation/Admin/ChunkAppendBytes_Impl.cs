using System;
using System.IO;

using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  internal static class ChunkAppendBytes_Impl
  {
    internal static Int64 Execute(ILogging log, string fileCorrelationId, byte[] data)
    {
      #region Check parameters
      Guid guid;
      if (string.IsNullOrEmpty(fileCorrelationId) || !Guid.TryParse(fileCorrelationId, out guid))
      {
        log.Error("Invalid value for parameter 'fileCorrelationId': {fileCorrelationId}", fileCorrelationId);
        return -1;
      }

      if (data == null || data.Length == 0)
      {
        log.Error("Parameter 'data' null or empty: {data}", data);
        return -1;
      }
      #endregion

      try
      {
        var fileName = Utils.ChunkedFileUtils.GetChunkedFileName(fileCorrelationId);
        using (var file = File.Open(fileName, FileMode.Append, FileAccess.Write, FileShare.None))
        {          
          file.Write(data, 0, data.Length);
          return file.Length;
        }
      }
      catch (Exception err)
      {
        log.Error(err, "ChunkAppendBytes_Impl");
        return -1;
      }
    }
  }
}
