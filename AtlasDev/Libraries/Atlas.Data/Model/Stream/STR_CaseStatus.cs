using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  public class STR_CaseStatus : XPLiteObject
  {
    private int _caseStatusId;
    [Key]
    public int CaseStatusId
    {
      get
      {
        return _caseStatusId;
      }
      set
      {
        SetPropertyValue("CaseStatusId", ref _caseStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Stream.CaseStatus Status
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.CaseStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.CaseStatus>();
      }
    }

    private string _description;
    [Persistent, Size(40)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }


    #region Constructors

    public STR_CaseStatus() : base() { }
    public STR_CaseStatus(Session session) : base(session) { }

    #endregion

  }
}
