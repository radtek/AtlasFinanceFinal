using System;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IUserTrackingUserMovement
  {
    long UsageId { get; set; }
    long UserId { get; set; }
    DateTime EventDate { get; set; }
    string Event { get; set; }
    string Branch { get; set; }
    string Machine { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string OperatorCode { get; set; }      
  }
}
