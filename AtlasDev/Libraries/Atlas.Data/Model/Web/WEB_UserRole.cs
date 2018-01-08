using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public sealed class WEB_UserRole : XPLiteObject
  {
     [Key(AutoGenerate = true)]
    public Int64 UserRoleId { get;set;}
    
    [Indexed]
    [Persistent("PersonId")]
    public PER_Person Person { get;set;}

    [Indexed]
    [Persistent("WebRoleId")]
    public WEB_WebRole WebRole { get;set;}

    public DateTime CreateDate { get;set;}

    #region Constructors

    public WEB_UserRole() : base() { }
    public WEB_UserRole(Session session) : base(session) { }

    #endregion
  }
}
