using System;
using System.Collections.Generic;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IAssByPassUserRepository
  {
    long ByPassAssUser(DateTime StartDate, DateTime EndDate, string UserOperatorCode, string BranchNum,string RegionalOperatorCode, byte NewLevel, string Reason);
  }
}