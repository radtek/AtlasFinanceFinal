using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IGeneralRepository
  {
    ICollection<string> GetLegacyBranchNumbers(bool isClosed = false);
    ICollection<long> GetBranchIds(bool isClosed = false);
    IBranch GetBranchByLegacyBranchNumber(string legacyBranchNumber);
  }
}
