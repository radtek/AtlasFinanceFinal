using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class ApplicationSettlement : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int SettlementId { get; set; }
    [Indexed]
    [Persistent("ApplicationId")]
    public Application Application { get;set;}
    [Indexed]
    public DateTime RepaymentDate { get; set; }
    [Persistent]
    public decimal Amount { get; set; }
    [Persistent]
    [Indexed]
    public long ReferenceId { get; set; }
    [Indexed]
    [Persistent]
    public DateTime CreateDate { get; set; }

    public ApplicationSettlement() : base() { }
    public ApplicationSettlement(Session session) : base(session) { }
  }
}