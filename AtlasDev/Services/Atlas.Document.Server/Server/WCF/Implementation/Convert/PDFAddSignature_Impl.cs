using System;
using System.IO;
using System.Diagnostics;

using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Convert
{
  class PDFAddSignature_Impl
  {
    internal static byte[] Execute(ILogging log, byte[] source, bool isCompressed, string openPassword, DocOptions options, bool destCompress)
    {
      var methodName = "PDFAddSignature";
      log.Information("{MethodName} starting- source data is {DocumentLength} bytes", methodName, source != null ? source.Length : 0);
      var sw = Stopwatch.StartNew();

      Stream result = null;
      try
      {
        if (source == null || source.Length == 0)
        {
          return null;
        }

        if (isCompressed)
        {
          source = Compression.InMemoryDecompress(source);
        }

        // TODO: Sign the document with our signature
        //gdPicture.DigiSignWithStamp()

        result = Utils.PdfUtils.SetPDFProperties(source, openPassword, options);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      if (destCompress && result != null && result.Length > 0)
      {
        result = Compression.InMemoryCompress(result);
      }

      log.Information("{MethodName} completed: took {ElapsedMS}ms, generated {ResultLength} bytes",
          sw.Elapsed.TotalMilliseconds, result != null ? result.Length : 0);

      return result.StreamToByte();
    }
  }
}
