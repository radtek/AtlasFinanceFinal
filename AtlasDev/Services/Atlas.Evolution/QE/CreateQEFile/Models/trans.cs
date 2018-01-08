using System;


namespace Atlas.Evolution.Server.Code.Data.ASS_Models
{
  public class trans
  {
    public string client { get; set; }
    public string loan { get; set; }
    public string brnum { get; set; }
    public decimal order { get; set; }
    public decimal seqno { get; set; }
    public DateTime trdate { get; set; }
    public string trtype { get; set; }
    public string trstat { get; set; }
    public decimal? tramount { get; set; }
    public decimal? instinitfe { get; set; }
    public decimal? instinitva { get; set; }
    public decimal? interest { get; set; }
    public decimal? servicefee { get; set; }
    public decimal? servicevat { get; set; }
    public decimal? inspremval { get; set; }
    public decimal? inspremvat { get; set; }
    public decimal? inspolival { get; set; }
    public decimal? inspolivat { get; set; }
  }
}
