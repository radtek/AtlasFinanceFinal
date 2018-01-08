using System;
using System.Collections.Generic;


namespace BLL.CustomerSpecific.Atlas.GL
{
  public class ASS_DailyUpload_Detail
  {
    public ASS_DailyUpload_Detail()
    {
    }


    #region Public Properties

    public Int32 ADID { get; set; }

    public Int16 ProcessedInd { get; set; }
    public DateTime ProcessedDate { get; set; }
    //public Decimal TranDate { get; set; }
    public string Error { get; set; }
    public string BranchCode { get; set; }
    public decimal TransactionDate { get; set; }
    public string TransType { get; set; }
    public string TransDetailInd { get; set; }
    public string Description { get; set; }
    public string AdditionalInfo { get; set; }
    public decimal TransAmt { get; set; }
    public decimal TranVatAmt { get; set; }
    #endregion
  }
}


