/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Template and DevExpress rendering utilities
 *       
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2014- Created
 * 
 * 
 *  Comments:
 *  ------------------
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Data;
using System.IO;
using System.Linq;

using AutoMapper;
using DevExpress.Xpo;
using DevExpress.Snap;
using DevExpress.XtraPrinting;
using DevExpress.Snap.Core.API;
using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using DevExpress.Office;

using Atlas.Domain.Model;
using Atlas.DocServer.WCF.Interface;
using Atlas.Domain.DTO;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  using Atlas.DocServer.Utils;
  internal static class TemplateUtils
  {
    /// <summary>
    /// Gets specific template
    /// </summary>
    /// <param name="templateId"></param>
    /// <returns></returns>
    internal static DOC_TemplateStoreDTO GetTemplate(Int64 templateId)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        return Mapper.Map<DOC_TemplateStoreDTO>(unitOfWork.Query<DOC_TemplateStore>().First(s => s.TemplateId == templateId));
      }
    }


    internal static Stream RenderTemplate(long templateId, Int64 storageId, DataSet dataSet,
      Enumerators.Document.FileFormat renderFileFormat, DocOptions docOptions, RenderOptions renderOptions)
    {
      Stream result = new MemoryStream();

      var template = GetTemplate(templateId);
      if (template == null)
      {
        return null;
      }

      if (dataSet != null)
      {
        if (dataSet.Tables[0].Columns.IndexOf("storageid") < 0)
        {
          dataSet.Tables[0].Columns.Add("storageid", typeof(Int64));
          dataSet.Tables[0].Rows[0]["storageid"] = storageId;
        }
        var table = dataSet.Tables[0];
        for (var pageCount = 1; pageCount < 10; pageCount++)
        {
          for (var currPage = 1; currPage <= pageCount; currPage++)
          {
            table.Columns.Add(string.Format("Page_{0}_Of_{1}", currPage, pageCount), typeof(string),
              string.Format("[storageid]+'*{0}*{1}'", currPage, pageCount));
          }
        }
      }

      switch (template.TemplateFileFormat.Type)
      {
        #region DevExpress Snap reports
        case Enumerators.Document.FileFormat.SNX:
          using (var report = new SnapDocumentServer())
          {
            report.SnxBytes = template.FileBytes;

            var options = report.CreateSnapMailMergeExportOptions();
            options.DataSource = dataSet.Tables[0];
            options.ProgressIndicationFormVisible = false;
            
            if (renderFileFormat == Enumerators.Document.FileFormat.PRNX)
            {
              using (var ms = new MemoryStream())
              {
                // Create the mail-merged document                
                report.SnapMailMerge(options, ms, SnapDocumentFormat.Snap);

                ms.Position = 0;
                report.LoadDocument(ms, SnapDocumentFormat.Snap);

                // Convert to PRNX
                using (var printingSystem = new PrintingSystem())
                using (var printableComponentLink = new PrintableComponentLink(printingSystem))
                {
                  // Add the link to the printing system's collection of links.
                  //printingSystem.Links.AddRange(new object[] { printableComponentLink });
                  // Keep non-visible
                  printingSystem.ShowPrintStatusDialog = false;
                  printingSystem.ShowMarginsWarning = false;
                  printingSystem.ExportOptions.NativeFormat.Compressed = true;
                  printingSystem.ExportOptions.NativeFormat.ShowOptionsBeforeSave = false;

                  // Assign a control to be printed by this link.
                  printableComponentLink.Component = report;

                  // Create PRNX from source
                  printableComponentLink.CreateDocument(printingSystem);
                  printingSystem.SaveDocument(result);
                }
              }
            }

            else if (renderFileFormat == Enumerators.Document.FileFormat.PDF)
            {
              // TODO: We have to mail-merge to another Snap and export that to PDF?!!?
              using (var dest = new MemoryStream())
              {
                report.ExportToPdf(dest);
                result = Atlas.DocServer.WCF.Implementation.Utils.PdfUtils.SetPDFProperties(dest, "", docOptions);                
              }
            }
            else
            {
              report.SnapMailMerge(options, result, Atlas.DocServer.WCF.Implementation.Utils.DocUtils.ToDocumentFormat(renderFileFormat));
            }
          }

          break;
        #endregion

        #region DevExpress report
        case Enumerators.Document.FileFormat.REPX:
          using (var ms = new MemoryStream(template.FileBytes))
          {
            using (var report = XtraReport.FromStream(ms, false))
            {
              report.DataSource = dataSet;
              result = Atlas.DocServer.WCF.Implementation.Utils.XtraReportUtils.RenderReport(report, renderFileFormat, docOptions, renderOptions);
            }
          }
          break;

        #endregion

        #region Document mail-merge
        case Atlas.Enumerators.Document.FileFormat.TXT:
        case Atlas.Enumerators.Document.FileFormat.HTML:
        case Atlas.Enumerators.Document.FileFormat.MHT:
        case Atlas.Enumerators.Document.FileFormat.DOCX:
        case Atlas.Enumerators.Document.FileFormat.DOC:
        case Atlas.Enumerators.Document.FileFormat.ODT:
        case Atlas.Enumerators.Document.FileFormat.RTF:
          using (var rtfServer = new RichEditDocumentServer())
          {
            rtfServer.Unit = DocumentUnit.Millimeter;
            Atlas.DocServer.WCF.Implementation.Utils.RtfUtils.LoadDocument(rtfServer, template.FileBytes, template.TemplateFileFormat.Type);

            rtfServer.Options.MailMerge.DataSource = dataSet;
            if (renderFileFormat == Enumerators.Document.FileFormat.PDF)
            {
              // TODO: We have to mail-merge to another Snap and export that to PDF?!!?
              using (var dest = new MemoryStream())
              {
                rtfServer.MailMerge(dest, Atlas.DocServer.WCF.Implementation.Utils.DocUtils.ToDocumentFormat(renderFileFormat));
                {
                  result = Atlas.DocServer.WCF.Implementation.Utils.PdfUtils.SetPDFProperties(dest, "", docOptions);
                }
              }
            }
            else
            {
              rtfServer.MailMerge(result, Atlas.DocServer.WCF.Implementation.Utils.DocUtils.ToDocumentFormat(renderFileFormat));
            }
          }
          break;

        #endregion
      }

      return result;
    }    

  }
}
