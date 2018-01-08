using Atlas.Enumerators;
using Atlas.ThirdParty.CompuScan.Batch.XML.Response;


namespace Atlas.ThirdParty.CompuScan.Batch
{
  public static class RecordToCSV
  {
    public static string Convert(Risk.BatchTransactionType transactionType, Risk.BatchSubTransactionType subTransactionType, object obj, string seqNo)
    {
      var loanReg = ((TRANS_DATARECORD)obj);
      //Clean
      loanReg.CC_FILENAME = loanReg.CC_FILENAME.Replace(",", ";");
      loanReg.ERROR_CODE = loanReg.ERROR_CODE.Replace(",", ";");
      loanReg.FILENAME = loanReg.FILENAME.Replace(",", ";");
      loanReg.JOB_ID = loanReg.JOB_ID.Replace(",", ";");
      loanReg.MESSAGE = loanReg.MESSAGE.Replace(",", ";");
      loanReg.MISC_DATA = loanReg.MISC_DATA.Replace(",", ";");
      loanReg.NLR_ENQ_REF_NO = loanReg.NLR_ENQ_REF_NO.Replace(",", ";");
      loanReg.NLR_ENQ_START_DATE = loanReg.NLR_ENQ_START_DATE.Replace(",", ";");
      loanReg.NLR_FILENAME = loanReg.NLR_FILENAME.Replace(",", ";");
      loanReg.NUM = loanReg.NUM.Replace(",", ";");
      loanReg.REFERENCE_NO = loanReg.REFERENCE_NO.Replace(",", ";");
      loanReg.SUMM_FILENAME = loanReg.SUMM_FILENAME.Replace(",", ";");
      loanReg.TRANS_STATUS = loanReg.TRANS_STATUS.Replace(",", ";");
      loanReg.TRANS_SUB_TYPE = loanReg.TRANS_SUB_TYPE.Replace(",", ";");
      loanReg.TRANS_TYPE = loanReg.TRANS_TYPE.Replace(",", ";");

      return string.Format("{0},{1},{2},{3},{4},{5},{6}", seqNo, loanReg.TRANS_STATUS, loanReg.ERROR_CODE, loanReg.MESSAGE, loanReg.MISC_DATA, loanReg.NLR_ENQ_REF_NO, loanReg.NLR_ENQ_START_DATE);
    }
  }
}
