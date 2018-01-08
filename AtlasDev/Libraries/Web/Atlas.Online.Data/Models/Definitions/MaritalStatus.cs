using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class MaritalStatus : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int MaritalStatusId { get; set; }
    [NonPersistent]
    public WebEnumerators.MaritalStatus Type
    {
      get { return Description.FromStringToEnum<WebEnumerators.MaritalStatus>(); }
      set { this.Description = value.ToStringEnum(); }
    }
    public string Description { get; set; }

    public MaritalStatus() : base() { }
    public MaritalStatus(Session session) : base(session) { }
  }
}