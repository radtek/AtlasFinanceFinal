using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Integration.Interface;
using Atlas.Server.Implementation.Token;
using Atlas.Domain.Model;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class GetUsers_Impl
  {
    internal static UsersResult GetUsers(ILogging log, string loginToken)
    {
      try
      {
        log.Information("[GetUsers]- {LoginToken}", loginToken);

        #region Basic validation
        if (string.IsNullOrEmpty(loginToken))
        {
          return new UsersResult() { Error = "Token cannot be empty- login first!" };
        }
        string userId;
        string branch;
        if (!UserToken.TryGetUserInfo(loginToken, out userId, out branch))
        {
          return new UsersResult() { Error = "Login token invalid/has expired" };
        }

        #endregion

        UserDetail[] users;
        using (var uow = new UnitOfWork())
        {
          users = uow.Query<PER_Person>()
            .Where(s => s.Security != null && s.Security.IsActive)
            .Select(s => new UserDetail()
            {
              FirstName = s.Firstname,
              Surname = s.Lastname,
              IdNumber = s.IdNum,              
              OperatorCode = s.Security.LegacyOperatorId
            }).ToArray();
        }

        return new UsersResult() { UserList = users };
      }
      catch (Exception err)
      {
        log.Error(err, "GetUsers()");
        return new UsersResult() { Error = "Unexpected server error" };
      }
    }
  }
}