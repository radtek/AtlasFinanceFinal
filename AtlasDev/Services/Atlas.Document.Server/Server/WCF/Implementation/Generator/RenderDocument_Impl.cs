using System;
using System.IO;
using System.Diagnostics;

using AutoMapper;

using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Generator
{
  class RenderDocument_Impl
  {
    internal static byte[] Execute(ILogging log, long templateId, Int64 storageId, FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      var methodName = "RenderDocument";
      log.Information("{MethodName} started, {TemplateId}, {FileFormat}, {@DocOptions}, {@RenderOptions} ", methodName,
        templateId, fileFormat, docOptions, renderOptions);
      var sw = Stopwatch.StartNew();

      if (templateId <= 0 && fileFormat == FileFormatEnums.FormatType.NotSet || docOptions == null || renderOptions == null)
      {
        log.Error("{MethodName} missing key parameters, {TemplateId}, {FileFormat}, {@DocOptions}, {@RenderOptions} ", methodName,
          templateId, fileFormat, docOptions, renderOptions);
        return null;
      }

      Stream result = null;

      var fileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(fileFormat);
      try
      {
        result = Utils.TemplateUtils.RenderTemplate(templateId, storageId, null, fileFormatEnum, docOptions, renderOptions);
      }
      catch (Exception err)
      {
        log.Error("{0}", err);
      }

      log.Information("{MethodName} completed in {ElapsedMS}ms with {ResultLength} bytes", methodName,
        sw.ElapsedMilliseconds, result != null ? result.Length : 0);

      if (destCompress && result != null && result.Length > 0)
      {
        return Compression.InMemoryCompress(result).ToArray();
      }

      return result.StreamToByte();
    }
  }
}
