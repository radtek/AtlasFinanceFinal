using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public sealed class WEB_WebRole : XPLiteObject
  {
    private Int64 _webRoleId;
    [Key(AutoGenerate = true)]
    public Int64 WebRoleId
    {
      get
      {
        return _webRoleId;
      }
      set
      {
        SetPropertyValue("WebRoleId", ref _webRoleId, value);
      }
    }

    private string _role;
    public string Role
    {
      get
      {
        return _role;
      }
      set
      {
        SetPropertyValue("Role", ref _role, value);
      }
    }

    private DateTime _createDate;
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

    public WEB_WebRole() : base() { }
    public WEB_WebRole(Session session) : base(session) { }

    #endregion
  }
}
