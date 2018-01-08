using Atlas.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Orchestration.Server.Structures
{
  public sealed class BankDetail
  {

    public string AccountName { get;set;}
    public string AccountNo { get;set;}
    public General.BankPeriod Period { get;set;}
    public General.BankAccountType AccountType { get;set;}
    public General.BankName Bank { get;set;}


 
  }
}
