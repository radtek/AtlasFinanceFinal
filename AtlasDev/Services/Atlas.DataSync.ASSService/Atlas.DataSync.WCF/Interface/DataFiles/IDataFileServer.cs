/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Chunking file uploading/downloading- Interface
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 *     16 June 2013- Created
 *     
 * 
 *  Comments:
 *  ------------------
 *     
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;


namespace Atlas.DataSync.WCF.Interface
{
  [ServiceContract(Namespace = "urn:Atlas/ASS/DataSync/File/2013/08")]
  public interface IDataFileServer
  {
    [OperationContract]
    long GetFileSize(SourceRequest sourceRequest, string fileName);

    [OperationContract]
    bool AppendFileChunk(SourceRequest sourceRequest, string fileName, byte[] buffer);

    [OperationContract]
    byte[] DownloadFileChunk(SourceRequest sourceRequest, string fileName, long offset, int chunkSize);

    [OperationContract]
    string GetFileChecksum(SourceRequest sourceRequest, string fileName, string method);

  }       
}