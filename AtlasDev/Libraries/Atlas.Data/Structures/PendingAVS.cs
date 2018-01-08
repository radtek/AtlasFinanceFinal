using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Structures
{
  public class PendingAVS
  {
    public string IdNumber { get; set; }
    public string Initials { get; set; }
    public string LastName { get; set; }
    public string BankAccountNumber { get; set; }
    public string BankName { get; set; }
    public long BankId { get; set; }
  }
}