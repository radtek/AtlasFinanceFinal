/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    WCF implementation of document generator- uses DevExpress XtraReporting, Snap and RichText server
 *    components.
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
 *     Using lots of large byte[] arrays ?may? cause GC memory issues (LOH- large object heap)- I try to use streams as 
 *     much as possible to avoid LOH issues and only convert the stream to a byte[] at the end.
 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;
using System.Text;

using AutoMapper;

using Atlas.DocServer.WCF.Implementation.Admin;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Generator
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class DocumentGeneratorServer : IDocumentGeneratorServer
  {
    public DocumentGeneratorServer(ILogging log)
    {
      _log = log;
    }
    /// <summary>
    /// Returns template details (DOC_TemplateStore), based on template type, revision and language
    /// </summary>
    /// <param name="templateType">The specific template type, i.e. Ass quote - Enumerators.Document.DocumentTemplate</param>
    /// <param name="revision">Revision (optional- defaults to the latest)</param>
    /// <param name="language">Language wanted (optional- defaults to English)</param>
    /// <param name="wantTemplateFileBytes">Return the DOC_TemplateStore.FileBytes actual template as byte array</param>
    /// <returns>The template found, null if none found</returns>
    public DocTemplate GetTemplateByType(TemplateEnums.TemplateTypes templateType, int revision, LanguageEnums.Language language, bool wantTemplateFileBytes)
    {
      return GetTemplateByType_Impl.Execute(_log, templateType, revision, language, wantTemplateFileBytes);
    }

    
    /// <summary>
    /// Get a template using the template store id
    /// </summary>
    /// <param name="templateStoreId">THe template store ID</param>
    /// <returns>The template</returns>
    public DocTemplate GetTemplateById(long templateStoreId)
    {
      return GetTemplateById_Impl.Execute(_log, templateStoreId);
    }


    /// <summary>
    /// Renders a document with a Newtonsoft Json encoded dataset
    /// </summary>
    /// <param name="templateId">The document template to use</param>
    /// <param name="data">The Newtonsoft serialized DataSEt</param>
    /// <param name="fileFormat">File format type to generate</param>
    /// <param name="docOptions">The document destination document properties</param>
    /// <param name="renderOptions">The rendering options</param>
    /// <param name="destCompress">Compress the result</param>
    /// <returns>null if any error, else file in requested file format</returns>
    public byte[] RenderDocumentWithJSonDataSet(Int64 templateId, Int64 storageId, byte[] data, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      var result = RenderDocumentWithJSonDataSet_Impl.Execute(_log, templateId, storageId, data, fileFormat, docOptions, renderOptions, destCompress);      
      AddToStorage(result, templateId, storageId, data, fileFormat, docOptions, renderOptions, destCompress);
      return result;
    }


    /// <summary>
    /// Renders a document using given template and data to a specific format (PDF, etc.), using binary & compressed .NET DataSet
    /// </summary>
    /// <param name="templateId">The document template to use</param>
    /// <param name="data">Binary, compressed DataSet</param>
    /// <param name="fileFormat">File format type to generate</param>
    /// <param name="destCompress">Compress the result</param>
    /// <param name="docOptions">The document destination document properties</param>
    /// <param name="renderOptions">The rendering options</param>
    /// <returns>null if any error, else file in requested file format</returns>
    public byte[] RenderDocumentWithDataSet(Int64 templateId, Int64 storageId, byte[] data, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      var result = RenderDocumentWithDataSet_Impl.Execute(_log, templateId, storageId, data, fileFormat, docOptions, renderOptions, destCompress);
      AddToStorage(result, templateId, storageId, data, fileFormat, docOptions, renderOptions, destCompress); 
      return result;
    }


    /// <summary>
    /// Renders/generates a document template which does not need a data source
    /// </summary>
    /// <param name="templateId">The document template to use</param>
    /// <param name="fileFormat">File format type to generate</param>
    /// <returnsnull if any error, else file in requested file format></returns>
    public byte[] RenderDocument(Int64 templateId, Int64 storageId, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      var result = RenderDocument_Impl.Execute(_log, templateId, storageId, fileFormat, docOptions, renderOptions, destCompress);
      AddToStorage(result, templateId, templateId, null, fileFormat, docOptions, renderOptions, destCompress);
      return result;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="templateId"></param>
    /// <param name="json"></param>
    /// <param name="fileFormat"></param>
    /// <param name="docOptions"></param>
    /// <param name="renderOptions"></param>
    /// <param name="destCompress"></param>
    /// <param name="storageId"></param>
    /// <returns></returns>
    public byte[] RenderDocumentWithJson(Int64 templateId, Int64 storageId, string json, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      var result = RenderDocumentWithJSon_Impl.Execute(_log, templateId, storageId, json, fileFormat, docOptions, renderOptions, destCompress);
      AddToStorage(result, templateId, storageId, Encoding.UTF8.GetBytes(json), fileFormat, docOptions, renderOptions, destCompress);
      return result;
    }


    /// <summary>
    /// Add a rendered document to the document storage system
    /// </summary>
    /// <param name="result"></param>
    /// <param name="templateId"></param>
    /// <param name="json"></param>
    /// <param name="fileFormat"></param>
    /// <param name="docOptions"></param>
    /// <param name="renderOptions"></param>
    /// <param name="destCompress"></param>
    /// <param name="storageId"></param>
    private static void AddToStorage(byte[] result, long templateId, Int64 storageId, byte[] data, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      if (result != null && storageId > 0)
      {
        AddFileToPreparedStore_Impl.Execute(_log, new StorageInfo
          {
            Generator = GeneratorEnums.Generators.AtlasDocGen,
            StorageId = storageId,
            FileFormatType = Mapper.Map<FileFormatEnums.FormatType>(fileFormat),
            SourceTemplateId = templateId,
            CreateDate = DateTime.Now,
            SourceData = data,
            Keywords = docOptions.Keywords
          }, result, destCompress);
      }
      else
      {
        _log.Error("AddToStorage: Failed to add document to storage: Result or StorageId empty: {ResultLength}, {StorageId}", result != null ? result.Length : 0, storageId);
      }
    }


    #region Private vars

    /// <summary>
    /// log4net
    /// </summary>
    private static ILogging _log;

    #endregion
    
  }
}
