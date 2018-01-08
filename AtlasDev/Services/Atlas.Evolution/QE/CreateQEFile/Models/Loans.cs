using System;


namespace Atlas.Evolution.Server.Code.Data.ASS_Models
{
  public class loans
  {
    public decimal? payno { get; set; }
    public string brnum { get; set; }
    public decimal cheque { get; set; }    
    public string client { get; set; }  
    public DateTime loandate { get; set; }   
    public string loanmeth { get; set; }
    public decimal tramount { get; set; }  
    public string loan { get; set; }
    public decimal? basic { get; set; }
    public decimal? flyer_com { get; set; }   
    public string popup_lr { get; set; }  
    public string status { get; set; }
    public decimal outamnt { get; set; }
  }
}
