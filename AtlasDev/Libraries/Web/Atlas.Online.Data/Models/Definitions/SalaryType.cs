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
  public sealed class SalaryType : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int AddressTypeId { get; set; }
    [NonPersistent]
    public Enumerators.General.SalaryFrequency Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.SalaryFrequency>(); }
      set { Description = value.ToStringEnum(); }
    }

    public string Description { get; set; }

    public SalaryType() : base() { }
    public SalaryType(Session session) : base(session) { }
  }
}