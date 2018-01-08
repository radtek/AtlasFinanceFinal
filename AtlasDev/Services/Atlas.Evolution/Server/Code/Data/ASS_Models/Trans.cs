using System;


namespace Atlas.Evolution.Server.Code.Data.ASS_Models
{
  public class trans
  {
    public string client { get; set; }
    public string loan { get; set; }
    public string brnum { get; set; }
    public string oper_brnum { get; set; }
    public decimal order { get; set; }
    public decimal seqno { get; set; }
    public DateTime trdate { get; set; }
    public string trtype { get; set; }
    public string trstat { get; set; }
    public string split_type { get; set; }
    public decimal? tramount { get; set; }
    public DateTime statdate { get; set; }
    public decimal aedo_feev { get; set; }
    public decimal npamount { get; set; }
    public decimal outamnt { get; set; }
    public string reason { get; set; }
    public string old_reason { get; set; }
    public decimal receiptno { get; set; }
    public string user { get; set; }
    public DateTime userdateo { get; set; }
    public DateTime userdate { get; set; }
    public string usertime { get; set; }
    public bool absa { get; set; }    
    
    public string oper { get; set; }
    public string operold { get; set; }
    public string glbank { get; set; }
    public decimal star_qin { get; set; }
    public decimal star_qbal { get; set; }
    public decimal star_issue { get; set; }
    public decimal star_nin { get; set; }
    public decimal star_nbal { get; set; }
    public decimal amgrnum { get; set; }
    public decimal authnum { get; set; }
    public DateTime origdate { get; set; }
    public string smart_date { get; set; }
    public decimal inst_days { get; set; }
    public DateTime duedate { get; set; }
    public decimal deferedamt { get; set; }
    public decimal instalamt { get; set; }
    public decimal? instcap { get; set; }
    public decimal? instcheq { get; set; }
    public decimal? instinitfe { get; set; }
    public decimal? instinitva { get; set; }
    public decimal? interest { get; set; }
    public decimal? servicefee { get; set; }
    public decimal? servicevat { get; set; }
    public decimal? inspremval { get; set; }
    public decimal? inspremvat { get; set; }
    public decimal? inspolival { get; set; }
    public decimal? inspolivat { get; set; }
    public decimal? collectfee { get; set; }
    public decimal? collectvat { get; set; }
    public decimal? eftfee { get; set; }
    public decimal? eftvat { get; set; }
    public decimal? nucardvat { get; set; }
    public string npaytranid { get; set; }
    public decimal repo_rate { get; set; }
    public DateTime backupdate { get; set; }
    public string loanrebate { get; set; }
    public string glbank1 { get; set; }
    public decimal loanamt1 { get; set; }
    public string glbank2 { get; set; }
    public decimal loanamt2 { get; set; }
    public string glbank3 { get; set; }
    public decimal loanamt3 { get; set; }
    public decimal sr_recno { get; set; }
    public string sr_deleted { get; set; }

    public long recid { get; set; }
    public string lrep_brnum { get; set; }
    public decimal itm1_cp_ex { get; set; }
    public decimal itm1_sp_ex { get; set; }
    public decimal itm1_vatpc { get; set; }
    public decimal itm2_cp_ex { get; set; }
    public decimal itm2_sp_ex { get; set; }
    public decimal lncheque { get; set; }
    public decimal itm2_vatpc { get; set; }
    public decimal totinsvap { get; set; }
    public string loan_link { get; set; }
  }
}
