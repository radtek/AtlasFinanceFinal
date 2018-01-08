using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class BankAccountType : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int AccountTypeId { get; set; }
    [NonPersistent]
    public Enumerators.General.BankAccountType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.BankAccountType>(); }
      set { Description = value.ToStringEnum(); }
    }
    public string Description { get; set; }

    public BankAccountType() : base() { }
    public BankAccountType(Session session) : base(session) { }
  }
}