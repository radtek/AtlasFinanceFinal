using DevExpress.XtraRichEdit;
using Falcon.Common.Interfaces.Services;
using System.IO;
using System.Text;

namespace Falcon.Common.Services
{
  public class PdfService : IPdfService
  {
    public byte[] GetPdfForMhtml(string content)
    {
      using (var documentServer = new RichEditDocumentServer())
      using (var msReader = new MemoryStream(Encoding.ASCII.GetBytes(content)))
      {
        documentServer.LoadDocument(msReader, DocumentFormat.Mht);
        documentServer.Options.Export.Html.EmbedImages = true;

        foreach (var section in documentServer.Document.Sections)
        {
          section.Margins.Top = 80;
          section.Margins.Bottom = 80;
          section.Margins.Left = 80;
          section.Margins.Right = 80;
        }
        using (var stream = new MemoryStream())
        {
          documentServer.ExportToPdf(stream);
          stream.Seek(0, SeekOrigin.Begin);
          return stream.ToArray();
        }
      }
    }
  }
}