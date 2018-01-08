using System;
using System.IO;

using DevExpress.Snap;
using DevExpress.Snap.Core.API;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal class SnapUtils
  {
    internal static void LoadTemplate(SnapDocumentServer snapServer, byte[] sourceDocument,
      Enumerators.Document.FileFormat sourceFormat)
    {
      using (var ms = new MemoryStream(sourceDocument))
      {
        switch (sourceFormat)
        {
          case Atlas.Enumerators.Document.FileFormat.HTML:
            snapServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Html);
            break;

          case Atlas.Enumerators.Document.FileFormat.MHT:
            // TOD: DX to fix            
            snapServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Mht);
            var rtf = snapServer.RtfText;
            snapServer.CreateNewDocument();
            snapServer.RtfText = rtf;

            break;

          case Atlas.Enumerators.Document.FileFormat.TXT:
            snapServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
            break;

          case Atlas.Enumerators.Document.FileFormat.RTF:
            snapServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Rtf);
            break;

          case Atlas.Enumerators.Document.FileFormat.DOC:
            snapServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Doc);
            break;

          case Atlas.Enumerators.Document.FileFormat.DOCX:
            snapServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            break;

          case Atlas.Enumerators.Document.FileFormat.ODT:
            snapServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenDocument);
            break;

          case Atlas.Enumerators.Document.FileFormat.SNX:
            snapServer.LoadDocument(ms, SnapDocumentFormat.Snap);
            break;

          default:
            throw new NotSupportedException(sourceFormat.ToString());
        }
      }
    }



  }
}
