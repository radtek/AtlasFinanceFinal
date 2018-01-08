using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class AVS_ResponseGroup:XPLiteObject
  {
    private Int32 _responseGroupId;
    [Key]
    public Int32 ResponseGroupId
    {
      get
      {
        return _responseGroupId;
      }
      set
      {
        SetPropertyValue("ResponseGroupId", ref _responseGroupId, value);
      }
    }

    [NonPersistent]
    public Enumerators.AVS.ResponseGroup Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.ResponseGroup>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.ResponseGroup>();
      }
    }
     
    private string _description;
    [Persistent, Size(20)]
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

    public AVS_ResponseGroup() : base() { }
    public AVS_ResponseGroup(Session session) : base(session) { }

    #endregion
  }
}
