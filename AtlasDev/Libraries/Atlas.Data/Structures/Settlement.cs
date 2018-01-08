using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Structures
{
  public class Settlement
  {
    public long SettlementId { get; set; }
    public string ResidentialAddress { get; set; }
    public string IdNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; } 
    public string AccountNo { get; set; }
    public decimal Amount { get; set; }
    public decimal Fees { get; set; }
    public decimal Interest { get; set; }
    public decimal TotalAmount { get; set; }
    public int ValidDaysLeft { get; set; }
    public DateTime SettlementDate { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ExpirationDate { get; set; }
  }
}
