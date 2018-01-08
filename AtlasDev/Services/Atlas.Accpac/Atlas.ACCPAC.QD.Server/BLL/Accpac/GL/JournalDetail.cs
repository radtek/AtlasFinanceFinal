using System;


namespace Atlas.ACCPAC.BLL.Accpac.GL
{
  public class GLJournalDetail
  {
    #region Constructor

    public GLJournalDetail()
    {
    } 
    #endregion


    #region Public Properties

    public int InternalBatchDetailID { get; set; }

    public int InternalBatchID { get; set; }

    public string TransactionNr { get; set; }

    public string AccountNr { get; set; }

    public decimal SourceCurrencyAmt { get; set; }

    public string SourceCurrency { get; set; }

    public string Reference { get; set; }

    public string Description { get; set; }

    public DateTime ProcessedDate { get; set; }

    public Int16 ProcessedInd { get; set; }

    public string Error { get; set; }

    public string SourceDescription { get; set; }

    public string SourceReference { get; set; }

    #endregion

  }
}


