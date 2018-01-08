/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    DevExpress helper class to get high quality rendering, via use of GDI+
 *       
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     May 2014- Created
 * 
 * 
 *  Comments:
 *  ------------------
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Printing;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal class CustomRichEditPrinter : RichEditPrinter
  {
    public CustomRichEditPrinter(RichEditDocumentServer server)
      : base(server.Model)
    {
    }

    protected override bool UseGdiPlus
    {
      get
      {
        return true;
      }
    }
  }
}
