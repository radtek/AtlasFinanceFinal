using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public sealed class BUR_EnquiryDocument : XPLiteObject
  {
    private Int64 _enquiryDocumentId;
    [Key(AutoGenerate = true)]
    public Int64 EnquiryDocumentId
    {
      get
      {
        return _enquiryDocumentId;
      }
      set
      {
        SetPropertyValue("EnquiryDocumentId", ref _enquiryDocumentId, value);
      }
    }

    private Enumerators.Credit.Report _report;
    [Persistent]
    public Enumerators.Credit.Report Report
    {
      get
      {
        return _report;
      }
      set
      {
        SetPropertyValue("Report", ref _report, value);
      }
    }

    private BUR_Enquiry _enquiry;
    [Persistent]
    public BUR_Enquiry Enquiry
    {
      get
      {
        return _enquiry;
      }
      set
      {
        SetPropertyValue("Enquiry", ref _enquiry, value);
      }
    }

    private DOC_FileStore _document;
    [Persistent]
    public DOC_FileStore Document
    {
      get
      {
        return _document;
      }
      set
      {
        SetPropertyValue("Document", ref _document, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
    public DateTime CreateDate
    {
      get
      {
        return _createDate;
      }
      set
      {
        SetPropertyValue("CreateDate", ref _createDate, value);
      }
    }

    #region Constructors

    public BUR_EnquiryDocument() : base() { }
    public BUR_EnquiryDocument(Session session) : base(session) { }

    #endregion
  }
}
