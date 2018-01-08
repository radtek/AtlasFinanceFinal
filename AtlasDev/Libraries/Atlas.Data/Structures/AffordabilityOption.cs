using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Structures
{
  public class AffordabilityOption
  {
    public Int64 AffordabilityOptionId { get; set; }
    public Decimal Amount { get; set; }
    public Decimal TotalFees { get; set; }
    public Decimal CapitalAmount { get; set; }
    public Decimal? TotalPayBack { get; set; }
    public Decimal? Instalment { get; set; }
    public int NumOfInstalment { get; set; }
    public int Period { get; set; }
    public int PeriodFrequencyId { get; set; }
    public string PeriodFrequency { get; set; }
    public int AffordabilityOptionTypeId { get; set; }
    public Enumerators.Account.AffordabilityOptionType AffordabilityOptionType { get; set; }
    public float? InterestRate { get; set; }
    public bool? CanClientAfford { get; set; }
  }
}
