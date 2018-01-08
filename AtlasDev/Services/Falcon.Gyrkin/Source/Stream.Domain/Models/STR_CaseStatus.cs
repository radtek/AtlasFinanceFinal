using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
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
    public Framework.Enumerators.CaseStatus.Type Status
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.CaseStatus.Type>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.CaseStatus.Type>();
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

    public STR_CaseStatus()
    { }
    public STR_CaseStatus(Session session) : base(session) { }

    #endregion

  }
}
