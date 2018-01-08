using System.Collections.Generic;

using Atlas.ThirdParty.CompuScan.Batch.XML;


namespace Atlas.ThirdParty.CompuScan.Batch
{
  public sealed class NLR
  {
    public List<NLR_LOANREG> Loan { get; set; }
    public List<NLR_LOANCLOSE> LoanClose { get; set; }
    public List<NLR_BATB2> BATB2 { get; set; }
  }
}
