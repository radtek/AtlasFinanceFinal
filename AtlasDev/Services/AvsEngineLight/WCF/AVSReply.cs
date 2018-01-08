using System;

using Atlas.Enumerators;


namespace Atlas.WCF.Interface
{
  public class AVSReply
  {
    public string Initials { get; set; }
    public string Lastname { get; set; }
    public string IdNumber { get; set; }
    public General.BankName Bank { get; set; }
    public string BankAccountNo { get; set; }
    public string BranchCode { get; set; }


    public bool WaitingReply { get; set; }
    public long TransactionId { get; set; }
    public Atlas.Enumerators.AVS.Result? FinalResult { get; set; }
    // Account specific
    public bool AccountExists { get; set; }
    public bool IdNumberMatch { get; set; }
    public bool InitialsMatch { get; set; }
    public bool LastNameMatch { get; set; }
    public bool AccountOpen { get; set; }
    public bool AccountAcceptsCredits { get; set; }
    public bool AccountAcceptsDebits { get; set; }
    public bool AccountOpen90days { get; set; }
  }

}
