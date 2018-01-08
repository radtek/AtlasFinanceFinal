using System;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.UserTracking
{
  public class UserTrackingUserMovement : IUserTrackingUserMovement
  {
    public long UsageId { get; set; }
    public long UserId { get; set; }
    public DateTime EventDate { get; set; }
    public string Event { get; set; }
    public string Branch { get; set; }
    public string Machine { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string OperatorCode { get; set; }
  }
}
