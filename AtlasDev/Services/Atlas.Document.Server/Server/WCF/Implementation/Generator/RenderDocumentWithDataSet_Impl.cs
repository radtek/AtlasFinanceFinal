using System;
using System.Data;
using System.Diagnostics;
using System.IO;

using AutoMapper;

using Atlas.Common.Utils;
using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Generator
{
  class RenderDocumentWithDataSet_Impl
  {
    internal static byte[] Execute(ILogging log, long templateId, Int64 storageId, byte[] data,
      FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      var methodName = "RenderDocumentWithDataSet";
      log.Information("{MethodName} started, {TemplateId}, {DataLength}, {FileFormat}, {@DocOptions}, {@RenderOptions} ", methodName,
        templateId, data != null ? data.Length : 0, fileFormat, docOptions, renderOptions);
      var sw = Stopwatch.StartNew();

      if (templateId <= 0 || data == null || fileFormat == FileFormatEnums.FormatType.NotSet || docOptions == null)
      {
        log.Error("{MethodName} missing key parameters: {TemplateId}, {DataLength}, {FileFormat}, {@DocOptions}, {@RenderOptions} ", methodName,
          templateId, data != null ? data.Length : 0, fileFormat, docOptions, renderOptions);
      
        return null;
      }
      
      Stream result = null;

      try
      {
        var dataSet = Serialization.DeserializeFromBytes(data, true) as DataSet;
        if (dataSet == null || dataSet.Tables == null || dataSet.Tables.Count == 0)
        {
          log.Error(new Exception("Empty dataset"), methodName);
          return null;
        }
                
        var fileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(fileFormat);
        result = Utils.TemplateUtils.RenderTemplate(templateId, storageId, dataSet, fileFormatEnum, docOptions, renderOptions);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }
            
      log.Information("{MethodName} completed in {ElapsedMS}ms with {ResultLength} bytes", methodName,
        sw.ElapsedMilliseconds, result != null ? result.Length : 0);

      return (destCompress && result != null && result.Length > 0) ? Compression.InMemoryCompress(result).ToArray() : result.StreamToByte();
    }
  }
}
