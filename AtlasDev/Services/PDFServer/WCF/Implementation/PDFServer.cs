

using Atlas.PDF.Server.WCF.Interface;
using DevExpress.Xpo;
using log4net;
using Pechkin;
using Pechkin.Synchronized;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Atlas.PDF.Server.WCF.Implemenation
{
  /// <summary>
  /// Implementation of Configuration server
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class PDFServer : IPDFServer
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(PDFServer));
    

    public PdfResult GetPdf(string content)
    {
      var result = new PdfResult();
      try
      {
        var oc = new ObjectConfig();
        result.Bytes = new SynchronizedPechkin(new GlobalConfig()).Convert(oc, content);
      }
      catch (Exception ex)
      {
        result.Error = ex.Message;
      }
      return result;
    }

    public PdfResult ConvertMhtToPdf(string content)
    {
      string decodedContent = Atlas.Common.Utils.Base64.Base64Decode(content);
      return GetPdf(decodedContent);
    }
  }
}
