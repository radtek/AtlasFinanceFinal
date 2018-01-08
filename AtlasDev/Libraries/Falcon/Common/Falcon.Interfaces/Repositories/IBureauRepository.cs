using System;
using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Interfaces.Structures.Bureau;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IBureauRepository
  {
    List<ICompuscanProducts> GetCompuscanProductsSummary(ICollection<long> branchIds, DateTime date);
    ICreditResponse GetScore(long debtorId, long branchId, bool newScore);
  }
}
