using System;
using System.Collections.Generic;
using System.ServiceModel;


namespace Atlas.DocServer.WCF.Interface
{
  /// <summary>
  /// Document Administration- Upload file/doc, prepare docs, get doc info
  /// </summary>
  [ServiceContract(Namespace = "urn:Atlas/ASS/DocServer/Admin/2014/05")]
  public interface IDocumentAdminServer
  {
    /// <summary>
    /// Starts a chunked file upload
    /// </summary>        
    /// <returns>CorrelationId/file upload reference (a GUID)</returns>
    [OperationContract]
    string ChunkStartUpload();


    /// <summary>
    /// Returns file size of fileReference
    /// </summary>
    /// <param name="fileCorrelationId">The correlation Id returned from "StartChunkUpload"</param>
    /// <returns>The file size in bytes</returns>
    [OperationContract]
    Int64 ChunkGetFileSize(string fileCorrelationId);


    /// <summary>
    /// Appends a chunk to a file started with "StartChunkUpload"
    /// </summary>
    /// <param name="fileCorrelationId">The correlation Id returned from "StartChunkUpload"</param>
    /// <returns>New size of the file</returns>
    [OperationContract]
    Int64 ChunkAppendBytes(string fileCorrelationId, byte[] data);


    /// <summary>
    /// Store a new document and returns the storage id
    /// </summary>
    /// <param name="info"></param>
    /// <param name="fileFormat"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    [OperationContract]
    Int64 StoreFile(StorageInfo info, byte[] document, bool documentIsCompressed, byte[] sourceData, bool sourceDataIsCompressed);


    /// <summary>
    /// Store a document uploaded via the 'Chunk...' methods
    /// </summary>
    /// <param name="info">Information regarding the document</param>
    /// <param name="fileCorrelationId">>The correlation Id returned from "StartChunkUpload"</param>
    /// <param name="documentIsCompressed"></param>
    /// <param name="sourceData"></param>
    /// <param name="sourceDataIsCompressed"></param>
    /// <returns></returns>
    [OperationContract]
    Int64 StoreFileChunked(StorageInfo info, string fileCorrelationId, bool documentIsCompressed, byte[] sourceData, bool sourceDataIsCompressed);


    /// <summary>
    /// Store a scanned document uploaded via the 'Chunk...' methods- use the DataMatrix barcode to determine the source document(s)
    /// </summary>
    /// <param name="fileCorrelationId">>The correlation Id returned from "StartChunkUpload"</param>
    /// <param name="documentIsCompressed">Is the source document compressed</param>
    /// <returns>The new documents detected and added</returns>
    [OperationContract]
    List<StorageInfo> StoreScannedFileChunked(string fileCorrelationId, FileFormatEnums.FormatType fileFormat, bool documentIsCompressed);


    /// <summary>
    /// Store a scanned document contained in 'data'- use the DataMatrix barcode to determine the source document(s)
    /// </summary>    
    /// <param name="data">The document</param>
    /// <param name="fileFormat"></param>
    /// <param name="documentIsCompressed">Is the document compressed</param>
    /// <returns>The new documents detected and added</returns>
    [OperationContract]
    List<StorageInfo> StoreScannedFile(byte[] data, FileFormatEnums.FormatType fileFormat, bool documentIsCompressed);
    

    /// <summary>
    /// Prepares storage for a document- if we need to use DocumentId in the document (i.e. barcode), we first need
    /// to get the DocumentId, before we can generate the document.
    /// </summary>
    /// <param name="info">Document info</param>
    /// <param name="sourceData">The data used to generate the document</param>
    /// <param name="sourceDataIsCompressed">Is the source data compressed</param>
    /// <returns>The new row's DocumentId , else -1</returns>
    [OperationContract]
    Int64 PrepareStore(StorageInfo info, byte[] sourceData, bool sourceDataIsCompressed);
        

    /// <summary>
    /// Retrieve the document file from the document store
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <returns></returns>
    [OperationContract]
    byte[] GetStorageFile(Int64 storageId, bool destCompress);


    /// <summary>
    /// Retrieves the documents information
    /// </summary>
    /// <param name="documentId"></param>
    /// <returns></returns>
    [OperationContract]
    StorageInfo GetStorageInfo(Int64 storageId);
    

   /// <summary>
    /// Adds a document file to an already generated document store
    /// </summary>
    /// <param name="info">Document info (optional data to add, require StorageId field and document format)</param>
    /// <param name="document">The document data</param>
    /// <param name="documentIsCompressed">Is the source document byte array compressed</param>
    [OperationContract]
    void AddFileToPreparedStore(StorageInfo info, byte[] document, bool documentIsCompressed);


    /// <summary>
    /// Returns all documents found for a given reference, and optionally generator, template and source storage id
    /// </summary>
    /// <param name="reference"></param>
    /// <returns></returns>
    [OperationContract]
    StorageInfo[] ListStorageInfoFor(string reference, GeneratorEnums.Generators generator,
      TemplateEnums.TemplateTypes template, Int64 sourceStorageId);

  }

}
