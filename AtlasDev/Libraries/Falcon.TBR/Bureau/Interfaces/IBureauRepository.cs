using System;
using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.TBR.Bureau.Interfaces
{
  public interface IBureauRepository
  {
    List<ICompuscanProducts> GetCompuscanProductsSummary(ICollection<long> branchIds, DateTime date);
    ICreditResponse GetScore(string debtorFirstName, string debtorLastName, string debtorIdNumber, IAddress debtorResidentialAddress,
     IContact debtorContactCellNo, IContact debtorContactTelNoHome, IContact debtorContactTelNoWork, long branchId, bool newScore);
  }
}
