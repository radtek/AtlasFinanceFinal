using System;



namespace Atlas.Evolution.Server.Code.Data.ASS_Models
{
  public class payplan_h
  {
    public string brnum { get; set; }
    public string client { get; set; }
    public string loan { get; set; }

    public decimal? plan_num { get; set; }
    public string status { get; set; }
    public string contract { get; set; }
    public string oper { get; set; }
    public string station { get; set; }
    public string loanmeth { get; set; }
    public decimal? payno { get; set; }
    public decimal? outamnt { get; set; }
    public decimal instalamt { get; set; }
    public DateTime? startdate { get; set; }
    public DateTime? userdate { get; set; }
    public string usertime { get; set; }
    public string swipresult { get; set; }
    public string swipetrnid { get; set; }
    public DateTime backupdate { get; set; }
    public string cardno { get; set; }
    public string bnktype { get; set; }
    public string brcode { get; set; }
    public string bankacc { get; set; }
    public string bank { get; set; }
    public decimal sr_recno { get; set; }
    public string sr_deleted { get; set; }

    public long recid { get; set; }
    public string lrep_brnum { get; set; }

  }
}
