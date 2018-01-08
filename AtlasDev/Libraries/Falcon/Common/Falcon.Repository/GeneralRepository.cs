using System.Collections.Generic;
using System.Linq;
using Atlas.Domain.Model;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Structures.Branch;

namespace Falcon.Common.Repository
{
  public class GeneralRepository : IGeneralRepository
  {
    public ICollection<string> GetLegacyBranchNumbers(bool isClosed = false)
    {
      using (var uow = new UnitOfWork())
      {
        return new XPQuery<BRN_Branch>(uow).Where(b => b.IsClosed == isClosed).Select(b => b.LegacyBranchNum).ToList();
      }
    }

    public ICollection<long> GetBranchIds(bool isClosed = false)
    {
      using (var uow = new UnitOfWork())
      {
        return new XPQuery<BRN_Branch>(uow).Where(b => b.IsClosed == isClosed).Select(b => b.BranchId).ToList();
      }
    }

    public IBranch GetBranchByLegacyBranchNumber(string legacyBranchNumber)
    {
      using (var uow = new UnitOfWork())
      {
        var branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.LegacyBranchNum == legacyBranchNumber);

        if (branch == null)
          return null;

        return new Branch
        {
          BranchId = branch.BranchId,
          LegacyBranchNum = branch.LegacyBranchNum,
          Region = branch.Region.Description,
          Name = branch.Company.Name,
          RegionId = branch.Region.RegionId
        };
      }
    }
  }
}
