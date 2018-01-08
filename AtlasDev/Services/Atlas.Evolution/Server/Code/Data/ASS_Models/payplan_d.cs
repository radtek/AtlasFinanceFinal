using System;


namespace Atlas.Evolution.Server.Code.Data.ASS_Models
{
  public class payplan_d
  {
    public string brnum { get; set; }
    public string client { get; set; }
    public string loan { get; set; }

    public decimal? plan_num { get; set; }
    public decimal? order { get; set; }
    public DateTime trdate { get; set; }
    public decimal? tramount { get; set; }
    public DateTime backupdate { get; set; }
    public decimal sr_recno { get; set; }
    public string sr_deleted { get; set; }
    public long recid { get; set; }
    public string lrep_brnum { get; set; }
  }
}
