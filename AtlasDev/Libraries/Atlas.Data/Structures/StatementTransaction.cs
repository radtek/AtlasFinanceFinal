using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Structures
{
  public class StatementTransaction
  {
    public long TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public decimal Balance { get; set; }
  }
}
