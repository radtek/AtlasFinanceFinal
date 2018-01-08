using System;

using System.Collections.Generic;

namespace Atlas.ACCPAC.BLL.Accpac.GL
{
  public class JournalHeader
  {

    #region Constructor

    public JournalHeader()
    {
      _journalDetails = new List<GLJournalDetail>();
    }

    public JournalHeader(int internalBatchID, string entryNr, string description, string fiscalYear, string fiscalPeriod, decimal entryDate, string sourceLedger, string sourceType, Int16 autoRefersal, DateTime dateCreated, DateTime processedDate, Int16 processedInd, string error, string sourceDescription, string sourceReference)
    {
      InternalBatchID = internalBatchID;
      EntryNr = entryNr.Trim();
      Description = description.Trim();
      FiscalYear = fiscalYear.Trim();
      FiscalPeriod = fiscalPeriod.Trim();

      EntryDate = entryDate;
      SourceLedger = sourceLedger.Trim();
      SourceType = sourceType.Trim();
      AutoRefersal = autoRefersal;
      DateCreated = dateCreated;
      ProcessedDate = processedDate;
      ProcessedInd = processedInd;

      Error = error.Trim();
      SourceDescription = sourceDescription.Trim();
      SourceReference = sourceReference.Trim();

      _journalDetails = new List<GLJournalDetail>();
      PopulateClass();

    }

    private void PopulateClass()
    {
      var sql = new BusinessLogicLayer.DAL.SQL();
      _journalDetails = sql.Get_Accpac_GL_Get_JournalDetails(InternalBatchID);
    }

    #endregion


    #region Member variables
     
    // Children
    public List<GLJournalDetail> _journalDetails;

    #endregion


    #region Public Properties

    public int InternalBatchID { get; set; }

    public string EntryNr { get; set; }

    public string Description { get; set; }

    public string FiscalYear { get; set; }

    public string FiscalPeriod { get; set; }

    public decimal EntryDate { get; set; }

    public string SourceLedger{ get; set; }

    public string SourceType{ get; set; }

    public Int16 AutoRefersal{ get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime ProcessedDate { get; set; }

    public Int16 ProcessedInd { get; set; }

    public string Error { get; set; }

    public string SourceDescription { get; set; }

    public string SourceReference { get; set; }

    #endregion

  }
}


