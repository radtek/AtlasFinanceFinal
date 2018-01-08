using System;

namespace Falcon.Gyrkin.Controllers.Api.Models
{
  public class UserTrackingModel
  {
    public class TrackUserModel
    {
      public long? UserId { get; set; }
      public long? BranchId { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }

    }

    public class TrackBranchModel
    {
      public long BranchId { get; set; }
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
    }

    public class TrackUserSavePinModel
    {
      public long UserId { get; set; }
      public int AlertType { get; set; }
      public int Severity { get; set; }
      public int Elapse { get; set; }
      public int Value { get; set; }
      public string Notify { get; set; }
    }

    public class PinnedUserQueryModel
    {
      public bool Active { get; set; }
    }

    public class RemovePinnedQueryModel
    {
      public long PinnedUserId { get; set; }
    }
  }
}