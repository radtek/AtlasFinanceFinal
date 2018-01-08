using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Integration.Interface;
using Atlas.Domain.Model;
using Atlas.Server.Implementation.Token;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class GetBranchList_Impl
  {

    internal static BranchListResult GetBranchList(ILogging log, string loginToken)
    {
      try
      {
        log.Information("[GetBranchList]- {LoginToken}", loginToken);

        #region Basic validation
        if (string.IsNullOrEmpty(loginToken))
        {
          return new BranchListResult() { Error = "Token cannot be empty- login first!" };
        }
        string userId;
        string branch;
        if (!UserToken.TryGetUserInfo(loginToken, out userId, out branch))
        {
          return new BranchListResult() { Error = "Login token invalid/has expired" };
        }
        #endregion

        BranchDetail[] branchList;
        using (var uow = new UnitOfWork())
        {
          branchList = uow.Query<BRN_Branch>()
            .Where(s => !s.IsClosed && s.Company != null && s.Region != null)
            .Select(s => new BranchDetail() { Code = s.LegacyBranchNum, Description = s.Company.Name, Region = s.Region.Description })
            .ToArray();
        }

        return new BranchListResult() { BranchList = branchList };
      }
      catch (Exception err)
      {
        log.Error(err, "GetBranchList");
        return new BranchListResult() { Error = "Unexpected server error" };
      }
    }
  }
}