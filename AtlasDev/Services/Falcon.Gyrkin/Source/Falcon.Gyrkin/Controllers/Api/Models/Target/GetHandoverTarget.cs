namespace Falcon.Gyrkin.Controllers.Api.Models.Target
{
  public class GetHandoverTarget
  {
    public long BranchId { get; set; }
    public long HostId { get; set; }
    public int TargetMonth { get; set; }
    public int TargetYear { get; set; }
  }
}
