using Atlas.Enumerators;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Affordability : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int AffordabilityId { get; set; }
    [Indexed]
    [Persistent]
    public long OptionId { get; set; }
    [Indexed]
    [Persistent("ApplicationId")]
    public Application Application { get; set; }
    [Persistent]
    public decimal Amount { get; set; }
    [Persistent]
    public decimal RepaymentAmount { get; set; }
    [Persistent]
    public Decimal TotalFees { get; set; }
    [Persistent]
    public Decimal CapitalAmount { get; set; }
    [Persistent]
    public Decimal? Instalment { get; set; }
    [Persistent]
    public Decimal? Arrears { get; set; }
    [Persistent]
    public Account.AffordabilityOptionType OptionType { get; set; }
    [Persistent]
    public bool Accepted { get; set; }
    [Persistent]    
    public DateTime CreateDate { get; set; }

    public Affordability() : base() { }
    public Affordability(Session session) : base(session) { }

    public static Affordability GetFirstBy(Session session, Expression<Func<Affordability, bool>> predicate)
    {
      return new XPQuery<Affordability>(session).FirstOrDefault(predicate);
    }
  }
}