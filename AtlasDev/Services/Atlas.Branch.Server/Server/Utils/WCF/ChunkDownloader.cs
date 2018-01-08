using System;
using System.IO;

using Atlas.DataSync.WCF.Client.ClientProxies;


namespace ASSSyncClient.Utils.WCF
{
  public static class ChunkDownloader
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="localFilePath"></param>
    /// <param name="remoteFileName"></param>
    /// <returns></returns>
    public static bool DownloadFile(string localFilePath, string remoteFileName, int chunkSize = 65000)
    {
      var complete = false;
      var attempCount = 0;
      while (!complete && attempCount++ < 3)
      {        
        try
        {
          long remoteFileSize = 0;
          #region Get file size on server
          using (var client = new DataSyncDataFileClient())
            {
              remoteFileSize = client.GetFileSize(ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest(), remoteFileName);              
            }
                    
          if (remoteFileSize <= 0)
          {
            return false;
          }
          #endregion

          #region Get size of file locally
          long localFileSize = 0;
          if (File.Exists(localFilePath))
          {
            var fileInfo = new FileInfo(localFilePath);
            localFileSize = fileInfo.Length;
          }
          if (localFileSize == remoteFileSize)
          {
            return true;
          }
          #endregion

          while (localFileSize < remoteFileSize)
          {
            byte[] buffer = null;
            using (var client =  new DataSyncDataFileClient())
              {
                buffer = client.DownloadFileChunk(ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest(), remoteFileName, localFileSize, chunkSize);
                if (buffer != null && buffer.Length > 0)
                {
                  using (var localFile = File.Open(localFilePath, FileMode.Append, FileAccess.Write, FileShare.None))
                  {
                    localFile.Write(buffer, 0, buffer.Length);
                  }
                  localFileSize += buffer.Length;
                }
                else
                {
                  throw new Exception("No response");
                }
              }
            
          }

          complete = localFileSize == remoteFileSize;
        }
        catch
        {
          System.Threading.Thread.Sleep(1000);
        }
      }

      return complete;
    }
  }
}
