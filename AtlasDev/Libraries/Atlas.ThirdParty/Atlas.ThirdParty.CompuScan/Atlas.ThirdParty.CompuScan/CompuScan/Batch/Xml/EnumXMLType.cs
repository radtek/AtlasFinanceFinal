using System;
using System.ComponentModel;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
  public static class EnumXMLType
  {
    public enum XMLType
    { 
      [Description("CSREG_LOAN")]
      CSREG_LOAD = 0,
      [Description("CSREG_CLIENT")]
      CSREG_CLIENT = 1,
      [Description("CSREG_PAYMENT")]
      CSREG_PAYMENT = 2,
      [Description("CSREG_ADDRESS")]
      CSREG_ADDRESS = 3,
      [Description("CSREG_TELEPHONE")]
      CSREG_TELEPHONE = 4,
      [Description("CSREG_EMPLOYER")]
      CSREG_EMPLOYER = 5,
      [Description("CSUPD_CLIENT")]
      CSUPD_CLIENT = 6,
      [Description("CSENQ_CONFLICT")]
      CSENQ_CONFLICT = 7,
      [Description("CSENQ_GLOBAL2")]
      CSENQ_GLOBAL2 = 8,
      [Description("NLR_LOANREG")]
      NLR_LOANREG = 9,
      [Description("NLR_LOANCLOSE")]
      NLR_LOANCLOSE = 10,
      [Description("NLR_BATA")]
      NLR_BATA = 11,
      [Description("NLR_BATB2")]
      NLR_BATB2 = 12,
      [Description("CSUPD_LOAN")]
      CSUPD_LOAN = 13,
      [Description("NLR_LOANREG2")]
      NLR_LOANREG2 = 14
    }
  }
}
