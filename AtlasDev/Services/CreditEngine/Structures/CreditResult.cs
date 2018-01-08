using Atlas.Enumerators;
using System.Collections.Generic;

using System;


namespace Atlas.Credit.Engine.Structures
{
  public class CreditResult
  {
    public string Score { get; set; }
    public Account.AccountStatus Decision { get; set; }
    public List<string> Reasons { get; set; }
  }
}
