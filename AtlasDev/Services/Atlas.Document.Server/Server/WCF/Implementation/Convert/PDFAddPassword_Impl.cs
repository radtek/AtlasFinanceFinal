using System;
using System.IO;
using System.Diagnostics;

using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Convert
{
  class PDFAddPassword_Impl
  {
    internal static byte[] Execute(ILogging log, byte[] source, bool isCompressed, string openPassword, DocOptions docOptions, bool destCompress)
    {
      var methodName = "PDFAddPassword";
      log.Information("{MethodName} starting- {@DocOptions}, source data is {DocumentLength} bytes", methodName, docOptions, source != null ? source.Length : 0);
      var sw = Stopwatch.StartNew();

      if (source == null || source.Length == 0)
      {
        return null;
      }

      Stream result = null;
      try
      {
        if (isCompressed)
        {
          source = Compression.InMemoryDecompress(source);
        }

        result = Utils.PdfUtils.SetPDFProperties(source, openPassword, docOptions);
      }
      catch (Exception err)
      {
        log.Error("{0}", err);
      }

      if (destCompress && result != null && result.Length > 0)
      {
        result = Compression.InMemoryCompress(result);
      }

      log.Information("{MethodName} completed: took {ElapsedMS}ms, generated {ResultLength} bytes", methodName,
          sw.Elapsed.TotalMilliseconds, result != null ? result.Length : 0);

      return result.StreamToByte();
    }
  }
}
