using System;


namespace Atlas.WCF.Interface
{
  public class AVSResponse
  {
    public bool WaitingReply { get; set; }
    public long TransactionId { get; set; }
    public Atlas.Enumerators.AVS.Result? FinalResult { get; set; }
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
