using System;
using System.Data;
using System.Diagnostics;

using AutoMapper;
using Newtonsoft.Json;
using System.Text;

using Atlas.Common.Utils;
using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Generator
{
  class RenderDocumentWithJSonDataSet_Impl
  {
    internal static byte[] Execute(ILogging log, long templateId, Int64 storageId, byte[] data, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      var methodName = "RenderDocumentWithJsonDataSet";
      byte[] result = null;
      log.Information("{MethodName} starting {TemplateId}, {FileFormat}, {DataLength}, {@DocOptions}, {@RenderOptions}", methodName,
        templateId, fileFormat, data != null ? data.Length : 0, docOptions, renderOptions);
      var sw = Stopwatch.StartNew();

      try
      {
        if (templateId <= 0 || data == null || data.Length == 0 || fileFormat == FileFormatEnums.FormatType.NotSet)
        {
          log.Error("{MethodName}- Missing key parameters: {TemplateId}, {FileFormat}, {DataLength}, {@DocOptions}, {@RenderOptions}",
            methodName, templateId, fileFormat, data != null ? data.Length : 0, docOptions, renderOptions);
          return null;
        }

        var dataSet = JsonConvert.DeserializeObject<DataSet>(Encoding.UTF8.GetString(data));
        var fileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(fileFormat);
        var stream = Utils.TemplateUtils.RenderTemplate(templateId, storageId, dataSet, fileFormatEnum, docOptions, renderOptions);
        if (stream != null)
        {
          result = stream.StreamToByte();
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      log.Information("{MethodName} completed in {ElapsedMS}ms with {ResultLength} bytes", methodName,
        sw.ElapsedMilliseconds, result != null ? result.Length : 0);

      return (destCompress && result != null && result.Length > 0) ? Compression.InMemoryCompress(result) : result;
    }
  }
}
