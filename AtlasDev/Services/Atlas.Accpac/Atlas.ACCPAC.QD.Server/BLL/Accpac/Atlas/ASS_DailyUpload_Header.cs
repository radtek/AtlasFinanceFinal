using System;
using System.Collections.Generic;

using BusinessLogicLayer.DAL;


namespace BLL.CustomerSpecific.Atlas.GL
{
  public class ASS_DailyUpload_Header
  {
    public ASS_DailyUpload_Header()
    {
    }


    #region Member variables

    // Children
    public List<ASS_DailyUpload_Detail> myASS_DailyUpload_Detail;

    #endregion

    #region Public Properties

    public int ProcessedInd { get; set; }
    public DateTime ProcessedDate { get; set; }
    public string Error { get; set; }
    public string BranchCode { get; set; }
    public decimal TransactionDate { get; set; }
    public DateTime DateAdded { get; set; }

    #endregion
  }
}


