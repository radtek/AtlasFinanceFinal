using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_TopUpStatus: XPLiteObject
  {
    private Int64 _topUpStatusId;
    [Key(AutoGenerate = true)]
    public Int64 TopUpStatusId
    {
      get
      {
        return _topUpStatusId;
      }
      set
      {
        SetPropertyValue("TopUpStatusId", ref _topUpStatusId, value);
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

    public ACC_TopUpStatus() : base() { }
    public ACC_TopUpStatus(Session session) : base(session) { }

    #endregion
  }
}
