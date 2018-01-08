using System.Collections.Generic;

using Atlas.ThirdParty.CompuScan.Batch.XML;


namespace Atlas.ThirdParty.CompuScan.Batch
{
  public sealed class Update
  {
    public List<CSUPD_CLIENT> Client { get; set; }

    public List<CSUPD_LOAN> Loan { get; set; }
  }
}
