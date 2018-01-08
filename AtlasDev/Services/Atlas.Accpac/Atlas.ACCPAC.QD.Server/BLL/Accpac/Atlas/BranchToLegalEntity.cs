using System;


namespace Atlas.ACCPAC.BLL.Accpac.Atlas
{
  public class BranchToLegalEntity
  {
    public BranchToLegalEntity()
    {
    }


    public string BranchCode { get; set; }
    public string LegalEntity { get; set; }

    public string GLPostFix
    {
      get { return String.Format("-{0}-{1}", BranchCode.Trim(), LegalEntity.Trim()); }
    }

  }
}
