using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class AffordabilityDto
  {
    public int AffordabilityId { get; set; }
    public long OptionId { get; set; }
    public ApplicationDto Application { get; set; }
    public decimal Amount { get; set; }
    public decimal RepaymentAmount { get; set; }
    public Decimal TotalFees { get; set; }
    public Decimal CapitalAmount { get; set; }
    public Decimal? Instalment { get; set; }
    public Decimal? Arrears { get; set; }
    public Account.AffordabilityOptionType OptionType { get; set; }
    public bool Accepted { get; set; }
    public DateTime CreateDate { get; set; }
  }
}