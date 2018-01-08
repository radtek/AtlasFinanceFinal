namespace Falcon.Gyrkin.Controllers.Api.Models
{
  public class BureauModels
  {
    public class ScoreModel
    {
      public long DebtorId { get; set; }
      public long BranchId { get; set; }
      public bool NewScore { get; set; }
    }
  }
}