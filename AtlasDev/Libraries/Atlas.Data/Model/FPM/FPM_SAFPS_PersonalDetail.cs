namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_PersonDetail : XPLiteObject
  {
    private Int64 _personDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("PersonDetailId")]
    public Int64 PersonDetailId
    {
      get
      {
        return _personDetailId;
      }
      set
      {
        SetPropertyValue("PersonDetailId", ref _personDetailId, value);
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

    private string _title;
    [Persistent, Size(4)]
    public string Title
    {
      get
      {
        return _title;
      }
      set
      {
        SetPropertyValue("Title", ref _title, value);
      }
    }

    private string _surname;
    [Persistent, Size(50)]
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

    private string _firstname;
    [Persistent, Size(50)]
    public string Firstname
    {
      get
      {
        return _firstname;
      }
      set
      {
        SetPropertyValue("Firstname", ref _firstname, value);
      }
    }

    private string _id;
    [Persistent, Size(30)]
    public string ID
    {
      get
      {
        return _id;
      }
      set
      {
        SetPropertyValue("ID", ref _id, value);
      }
    }

    private string _passport;
    [Persistent, Size(30)]
    public string Passport
    {
      get
      {
        return _passport;
      }
      set
      {
        SetPropertyValue("Passport", ref _passport, value);
      }
    }

    private string _dateOfBirth;
    [Persistent, Size(30)]
    public string DateOfBirth
    {
      get
      {
        return _dateOfBirth;
      }
      set
      {
        SetPropertyValue("DateOfBirth", ref _dateOfBirth, value);
      }
    }

    private string _gender;
    [Persistent, Size(2)]
    public string Gender
    {
      get
      {
        return _gender;
      }
      set
      {
        SetPropertyValue("Gender", ref _gender, value);
      }
    }

    private string _email;
    [Persistent, Size(2)]
    public string Email
    {
      get
      {
        return _email;
      }
      set
      {
        SetPropertyValue("Email", ref _email, value);
      }
    }

    #region Constructors

    public FPM_SAFPS_PersonDetail() : base() { }
    public FPM_SAFPS_PersonDetail(Session session) : base(session) { }

    #endregion
  }
}
