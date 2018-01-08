using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class AddressType : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public Int64 AddressTypeId { get; set; }
    [NonPersistent]
    public Enumerators.General.AddressType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.AddressType>(); }
      set { Description = value.ToStringEnum(); }
    }

    public string Description { get; set; }

    public AddressType() : base() { }
    public AddressType(Session session) : base(session) { }
  }
}