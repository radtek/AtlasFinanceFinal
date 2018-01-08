namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_AliasDetail: XPLiteObject
  {

    private Int64 _aliasDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("AliasDetailId")]
    public Int64 AliasDetailId
    {
      get
      {
        return _aliasDetailId;
      }
      set
      {
        SetPropertyValue("AliasDetailId", ref _aliasDetailId, value);
      }
    }

    private FPM_SAFPS_Enquiry _sAFPS;
    [Persistent("SafpsId")]
    [Indexed]
    public FPM_SAFPS_Enquiry SAFPS
    {
      get
      {
        return _sAFPS;
      }
      set
      {
        SetPropertyValue("SAFPS", ref _sAFPS, value);
      }
    }

    private string _surname;
    [Persistent, Size(100)]
    public string Surname
    {
      get
      {
        return _surname;
      }
      set
      {
        SetPropertyValue("Surname", ref _surname, value);
      }
    }

    private string _firstName;
    [Persistent, Size(100)]
    public string FirstName
    {
      get
      {
        return _firstName;
      }
      set
      {
        SetPropertyValue("FirstName", ref _firstName, value);
      }
    }

    #region Constructors

    public FPM_SAFPS_AliasDetail() : base() { }
    public FPM_SAFPS_AliasDetail(Session session) : base(session) { }

    #endregion
  }
}
