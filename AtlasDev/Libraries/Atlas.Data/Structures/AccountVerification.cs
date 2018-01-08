using Atlas.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Structures
{
  [Serializable]
  public sealed class AccountVerification
  {
    public long? TransactionId { get; set; }
    public Orchestration.AVSTransaction Transaction { get; set; }
  }
}
