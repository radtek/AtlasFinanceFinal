// -----------------------------------------------------------------------
// <copyright file="RiskEnquiry.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_PhoneTypeMap : XPLiteObject
  {
    private Int64 _PhoneTypeMapId;
    [Key(AutoGenerate = true)]
    public Int64 PhoneTypeMapId
    {
      get
      {
        return _PhoneTypeMapId;
      }
      set
      {
        SetPropertyValue("PhoneTypeMapId", ref _PhoneTypeMapId, value);
      }
    }

    private ContactType _PhoneType;
    [Indexed]
    [Persistent("PhoneTypeId")]
    public ContactType PhoneType
    {
      get
      {
        return _PhoneType;
      }
      set
      {
        SetPropertyValue("PhoneType", ref _PhoneType, value);
      }
    }

    private string _description;
    [Persistent, Size(35)]
    public string Description
    {
      get { return _description; }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    #region Constructors

    public FPM_PhoneTypeMap() : base() { }
    public FPM_PhoneTypeMap(Session session) : base(session) { }

    #endregion
  }
}
