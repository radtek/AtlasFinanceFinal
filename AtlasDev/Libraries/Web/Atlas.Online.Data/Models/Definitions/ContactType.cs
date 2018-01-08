using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class ContactType : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int ContactTypeId { get; set; }
    public string Description { get; set; }

    public ContactType() : base() { }
    public ContactType(Session session) : base(session) { }
  }
}