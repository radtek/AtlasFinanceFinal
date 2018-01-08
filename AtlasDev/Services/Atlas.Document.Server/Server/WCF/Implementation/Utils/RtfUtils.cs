using DevExpress.XtraRichEdit;
using System;
using System.IO;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal static class RtfUtils
  {
    /// <summary>
    /// Loads a RichEditDocumentServer using given document data and file format
    /// </summary>
    /// <param name="rtfServer">The RichEditDocumentServer</param>
    /// <param name="sourceDocument">Document data to bve loaded</param>
    /// <param name="sourceFormat">The source document file format</param>
    internal static void LoadDocument(RichEditDocumentServer rtfServer, byte[] sourceDocument,
      Enumerators.Document.FileFormat sourceFormat)
    {
      using (var ms = new MemoryStream(sourceDocument))
      {
        switch (sourceFormat)
        {
          case Atlas.Enumerators.Document.FileFormat.HTML:
            rtfServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Html);
            break;

          case Atlas.Enumerators.Document.FileFormat.MHT:
            rtfServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Mht);
            break;

          case Atlas.Enumerators.Document.FileFormat.TXT:
            rtfServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.PlainText);
            break;

          case Atlas.Enumerators.Document.FileFormat.RTF:
            rtfServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Rtf);
            break;

          case Atlas.Enumerators.Document.FileFormat.DOC:
            rtfServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.Doc);
            break;

          case Atlas.Enumerators.Document.FileFormat.DOCX:
            rtfServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
            break;

          case Atlas.Enumerators.Document.FileFormat.ODT:
            rtfServer.LoadDocument(ms, DevExpress.XtraRichEdit.DocumentFormat.OpenDocument);
            break;

          default:
            throw new NotSupportedException(sourceFormat.ToString());
        }
      }
    }

  }
}