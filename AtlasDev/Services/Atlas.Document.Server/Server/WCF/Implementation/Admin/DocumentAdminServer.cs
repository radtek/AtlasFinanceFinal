/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    WCF implementation of document administration- add documents, get documents, 
 *       
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     May 2014- Created
 * 
 * 
 *  Comments:
 *  ------------------
 *     
 *  
 *  Issues:
 *  ------------------
 *    Document security/viewing rights?
 *    
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;
using System.Collections.Generic;


using Atlas.DocServer.WCF.Interface;
using Atlas.DocServer.WCF.Implementation.Admin;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Admin
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class DocumentAdminServer : IDocumentAdminServer
  {
    public DocumentAdminServer(ILogging log)
    {
      _log = log;
    }

    public string ChunkStartUpload()
    {
      return ChunkStartUpload_Impl.Execute();
    }


    public long ChunkGetFileSize(string fileCorrelationId)
    {
      return ChunkGetFileSize_Impl.Execute(_log, fileCorrelationId);
    }


    public long ChunkAppendBytes(string fileCorrelationId, byte[] data)
    {
      return ChunkAppendBytes_Impl.Execute(_log, fileCorrelationId, data);
    }


    public List<StorageInfo> StoreScannedFileChunked(string fileCorrelationId, FileFormatEnums.FormatType fileFormat, bool documentIsCompressed)
    {
      return StoreScannedFileChunked_Impl.Execute(_log, fileCorrelationId, fileFormat, documentIsCompressed);
    }


    public List<StorageInfo> StoreScannedFile(byte[] data, FileFormatEnums.FormatType fileFormat, bool documentIsCompressed)
    {
      return StoreScannedFile_Impl.Execute(_log, data, fileFormat, documentIsCompressed);
    }


    public Int64 StoreFileChunked(StorageInfo info, string fileCorrelationId, bool documentIsCompressed, byte[] sourceData, bool sourceDataIsCompressed)
    {
      return StoreFileChunked_Impl.Execute(_log, info, fileCorrelationId, documentIsCompressed, sourceData, sourceDataIsCompressed);
    }


    public Int64 StoreFile(StorageInfo info, byte[] document, bool documentIsCompressed, byte[] sourceData, bool sourceDataIsCompressed)
    {
      return StoreFile_Impl.Execute(_log, info, document, documentIsCompressed, sourceData, sourceDataIsCompressed);
    }


    public Int64 PrepareStore(StorageInfo info, byte[] sourceData, bool sourceDataIsCompressed)
    {
      return PrepareStore_Impl.Execute(_log, info, sourceData, sourceDataIsCompressed);
    }


    public byte[] GetStorageFile(Int64 storageId, bool destCompress)
    {
      return GetStorageFile_Impl.Execute(_log, storageId, destCompress);
    }


    public StorageInfo GetStorageInfo(Int64 storageId)
    {
      return GetStorageInfo_Impl.Execute(_log, storageId);
    }


    public void AddFileToPreparedStore(StorageInfo info, byte[] document, bool documentIsCompressed)
    {
      AddFileToPreparedStore_Impl.Execute(_log, info, document, documentIsCompressed);
    }


    public StorageInfo[] ListStorageInfoFor(string reference, GeneratorEnums.Generators generator,
      TemplateEnums.TemplateTypes template, Int64 sourceStorageId)
    {
      return ListStorageInfoFor_Impl.Execute(_log, reference, generator, template, sourceStorageId);
    }


    #region Private fields

    private static ILogging _log;

    #endregion
                
  }
}
