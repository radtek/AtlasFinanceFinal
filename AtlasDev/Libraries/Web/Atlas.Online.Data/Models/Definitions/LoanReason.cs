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
  public sealed class LoanReason : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int ReasonId { get; set; }
    [NonPersistent]
    public WebEnumerators.LoanReason Type
    {
      get { return Description.FromStringToEnum<WebEnumerators.LoanReason>(); }
      set { this.Description = value.ToStringEnum(); }
    }
    public string Description { get; set; }

    public LoanReason() : base() { }
    public LoanReason(Session session) : base(session) { }
  }
}