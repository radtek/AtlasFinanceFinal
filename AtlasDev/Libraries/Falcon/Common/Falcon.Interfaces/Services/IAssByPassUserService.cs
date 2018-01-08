using Falcon.Common.Interfaces.Structures;

using System;


namespace Falcon.Common.Interfaces.Services
{
  public interface IAssByPassUserService
  {
    long AuthorizeUserForByPassOnFalcon(DateTime startDate, DateTime endDate, string userOperatorCode, string branchNum,
      string regionalOperatorId, byte newLevel, string reason);
    
  }
}
