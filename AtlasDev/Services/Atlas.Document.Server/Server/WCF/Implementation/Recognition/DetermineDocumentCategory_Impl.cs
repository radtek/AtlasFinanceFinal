using System;
using System.Diagnostics;

using Atlas.Common.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Recognition
{
  internal class DetermineDocumentCategory_Impl
  {
    internal static DocumentFound Execute(ILogging log, FileFormatEnums.FormatType sourceFileFormat, byte[] source, string openPassword, bool isCompressed)
    {
      var methodName = "DetermineDocumentCategory";
      log.Information("{MethodName} starting: {DocumentFileFormat}, {DocumentLength}", methodName, sourceFileFormat, source != null ? source.Length : 0);
      if (source == null || source.Length == 0)
      {
        log.Error("DetermineDocumentCategory: invalid parameter 'source'");
        return null;
      }
      if (sourceFileFormat == FileFormatEnums.FormatType.NotSet)
      {
        log.Error("{MethodName}: invalid parameter 'sourceFileFormat'", methodName);
        return null;
      }

      try
      {
        DocumentFound result = null;
        var timer = Stopwatch.StartNew();
        if (isCompressed)
        {
          source = Compression.InMemoryDecompress(source);
        }

        // TODO: // Load/cached GdPicture ADR?!?
        //  

        log.Information("{MethodName} completed with result: {@DocumentFound}, time: {ElapsedMS}ms", methodName, result, timer.ElapsedMilliseconds);

        return result;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        return null;
      }
    }
  }
}
