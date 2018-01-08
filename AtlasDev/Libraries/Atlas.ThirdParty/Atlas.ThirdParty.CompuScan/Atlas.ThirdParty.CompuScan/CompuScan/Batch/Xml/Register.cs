using Atlas.ThirdParty.CompuScan.Batch.XML;
using System.Collections.Generic;


namespace Atlas.ThirdParty.CompuScan.Batch
{
  public sealed class Register
  {
    public List<CSREG_CLIENT> Client { get; set; }
    public List<CSREG_LOAN> Loan { get; set; }
    public List<CSREG_PAYMENT> Payment { get; set; }
    public List<CSREG_ADDRESS> Address { get; set; }
    public List<CSREG_TELEPHONE> Telephone { get; set; }
    public List<CSREG_EMPLOYER> Employer { get; set; }
  }
}
