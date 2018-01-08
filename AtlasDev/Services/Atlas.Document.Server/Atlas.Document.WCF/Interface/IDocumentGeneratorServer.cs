using System;
using System.ServiceModel;


namespace Atlas.DocServer.WCF.Interface
{
  /// <summary>
  /// Template-based document generation using DX Snap reports and DX Reporting
  /// </summary>
  [ServiceContract(Namespace = "urn:Atlas/ASS/DocServer/Generator/2014/05")]
  public interface IDocumentGeneratorServer
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateType"></param>
    /// <param name="revision"></param>
    /// <param name="language"></param>
    /// <param name="wantTemplateFileBytes"></param>
    /// <returns></returns>
    [OperationContract]
    DocTemplate GetTemplateByType(TemplateEnums.TemplateTypes templateType, int revision, LanguageEnums.Language language, bool wantTemplateFileBytes);


    DocTemplate GetTemplateById(Int64 templateStoreId);


    /// <summary>
    /// Renders a document using given template and data to a specific format (PDF, etc.), using Newtonsoft compatible Json data
    /// </summary>
    /// <param name="templateStoreId">The document template to use</param>
    /// <param name="jsonData">Newtonsoft json compatible DataSet</param>
    /// <param name="fileFormat">File format type to generate</param>    
    /// <param name="destCompress">Compress the output using GZip?</param>    
    /// <param name="storageId">Store to document system using the given StorageId</param>
    /// <returns>null if any error, else file in requested file format</returns>
    [OperationContract]
    byte[] RenderDocumentWithJSonDataSet(Int64 templateStoreId, Int64 storageId, byte[] data, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress);


    /* The json looks like:
    ---------------------------
     {
       "Table1": [
         {
           "id": 0,
           "item": "item 0"
         },
         {
           "id": 1,
           "item": "item 1"
         }
       ]
     }
     ---------------------------
     */

    /// <summary>
    /// Renders a document using given template and data to a specific format (PDF, etc.), using binary serialized and compressed .NET DataSet
    /// </summary>
    /// <param name="templateStoreId">The document template to use</param>
    /// <param name="data">Binary serialized, compressed DataSet (Atlas.Common.Utils.Serialization.DeserializeFromBytes/SerializeToBytes(, true)</param>
    /// <param name="fileFormat">File format type to generate</param>
    /// <param name="destCompress">Compress the output using GZip?</param>    
    /// <param name="storageId">Store to document system using the given StorageId</param>
    /// <returns>null if any error, else file in requested file format</returns>
    [OperationContract]
    byte[] RenderDocumentWithDataSet(Int64 templateStoreId, Int64 storageId, byte[] data, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress);


    /// <summary>
    /// Renders/generates a document which does not need a data source 
    /// </summary>
    /// <param name="templateStoreId">The document template to use</param>
    /// <param name="fileFormat">File format type to generate</param>
    /// <param name="destCompress">Compress the output using GZip?</param>    
    /// <param name="storageId">Store to documnet system using the given StorageId</param>
    /// <returns>null if error, else file in requested file format</returns>
    [OperationContract]
    byte[] RenderDocument(Int64 templateStoreId, Int64 storageId, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress);


    /// <summary>
    /// Renders a document using simple json and template to a specific format (PDF, etc.)
    /// </summary>
    /// <param name="templateStoreId">The document template to use</param>
    /// <param name="json">A JSON string- can be array of values or simple key/value</param>
    /// <param name="fileFormat">File format type to generate</param>
    /// <param name="docOptions">Document options (encryption, security, page size, etc.)</param>
    /// <param name="renderOptions">Render options- i.e. colour depth, margins, DPI</param>
    /// <param name="destCompress">Compress the output using GZip?</param>
    /// <param name="storageId">Store to document system using the given StorageId</param>
    /// <returns>null if error, else rendered file</returns>
    [OperationContract]
    byte[] RenderDocumentWithJson(Int64 templateStoreId, Int64 storageId, string json, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress);
  }

}
