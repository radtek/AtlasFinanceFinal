using System;
using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Services
{
  public interface IUserTrackingService
  {
    IList<IUserTrackingPinnedUser> GetViolations(DateTime startDate, DateTime endDate);
    
  }
}
