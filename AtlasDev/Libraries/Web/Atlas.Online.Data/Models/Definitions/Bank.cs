using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Bank : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int BankId { get; set; }
    [NonPersistent]
    public Enumerators.General.BankName Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.BankName>(); }
      set { Description = value.ToStringEnum(); }
    }
    [Size(6)]
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }

    public Bank() : base() { }
    public Bank(Session session) : base(session) { }
  }
}