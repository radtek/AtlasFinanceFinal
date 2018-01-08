namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_Enquiry : XPLiteObject
  {
    private Int64 _safpsId;
    [Key(AutoGenerate = true)]
    [Persistent("SafpsId")]
    public Int64 SafpsId
    {
      get
      {
        return _safpsId;
      }
      set
      {
        SetPropertyValue("SafpsId", ref _safpsId, value);
      }
    }

    private BUR_Enquiry _enquiry;
    [Persistent("EnquiryId")]
    [Indexed]
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

    public FPM_SAFPS_Enquiry() : base() { }
    public FPM_SAFPS_Enquiry(Session session) : base(session) { }

    #endregion
  }
}
