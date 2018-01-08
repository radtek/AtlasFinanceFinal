#region Using

using System;
using System.Linq;

using Atlas.Domain.Model;
using DevExpress.Xpo;
using Atlas.Domain.DTO;

#endregion


namespace Atlas.Data.Repository
{
  public static class AtlasData
  {
    /// <summary>
    /// Finds selected branch, else null
    /// </summary>
    /// <param name="legacyBranchId"></param>
    /// <returns></returns>
    public static BRN_BranchDTO FindBranch(string legacyBranchId)
    {
      BRN_BranchDTO result = null;

      using (var unitOfWork = new UnitOfWork())
      {
        var branchDB = unitOfWork.Query<BRN_Branch>().FirstOrDefault(s =>
          s.LegacyBranchNum.PadLeft(3, '0') == legacyBranchId.PadLeft(3, '0'));

        if (branchDB != null)
        {
          result = AutoMapper.Mapper.Map<BRN_BranchDTO>(branchDB);
        }
      }

      return result;
    }

  }
}
