using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Interfaces.Structures.Reports.General;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface ICompanyRepository
  {
    ICollection<IBranch> GetAllBranches();
    List<IRegionBranch> GetPersonRegionBranches();
    ICollection<IBranch> GetActiveBranches(bool active = true);
    ICollection<IBranch> GetBranchesByIds(IList<long> branchIds);
    bool AssociateUser(long branchId, long personId);
    ICollection<IBranchServer> GetBranchSyncStatus(ICollection<long> branchIds);
    ICollection<IHost> GetAllHosts(General.Host? hostType = null);
  }
}