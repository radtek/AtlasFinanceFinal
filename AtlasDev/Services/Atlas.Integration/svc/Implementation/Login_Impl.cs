using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Server.Implementation.Token;
using Atlas.Integration.Interface;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public static class Login_Impl
  {
    public static LoginResult Login(ILogging log, SystemLoginRequest system, UserLoginRequest user)
    {     
      log.Information("{@System}-{@User}", system, user);
      try
      {
        #region Basic validation
        if (system == null)
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'system' cannot be null" };
        }
        if (user == null)
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'user' cannot be null" };
        }

        if (string.IsNullOrEmpty(system.UserName))
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'system.UserName' cannot be null" };
        }
        if (system.UserName.Length < 5 || system.UserName.Length > 20)
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'system.UserName' does not contain a valid value" };
        }

        if (string.IsNullOrEmpty(system.Password))
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'system.Password' cannot be null" };
        }
        if (system.Password.Length < 5 || system.Password.Length > 20)
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'system.Password' does not contain a valid value" };
        }

        if (string.IsNullOrEmpty(system.SystemId))
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'system.SystemId' cannot be null" };
        }
        if (system.SystemId.Length < 5 || system.SystemId.Length > 20)
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'system.SystemId' does not contain a valid value" };
        }

        if (string.IsNullOrEmpty(user.UserIdNum))
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'user.UserIdNum' cannot be null" };
        }
        if (user.UserIdNum.Length < 5 || user.UserIdNum.Length > 20)
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'user.UserIdNum' contains an invalid ID/passport number" };
        }

        if (string.IsNullOrEmpty(user.UserBranch))
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'user.UserBranch' cannot be null" };
        }
        if (user.UserBranch.Length != 3 && user.UserBranch.Length != 2)
        {
          return new LoginResult() { Successful = false, Error = "Parameter 'user.UserBranch' must contain a 2-digit branch" };
        }

        // TODO: We need to auth users
        if (system.UserName.ToLower() != "david" || system.Password != "kitley")
        {
          return new LoginResult() { Successful = false, Error = "Invalid logon credentials" };
        }

        // TODO: We need to auth apps
        if (system.SystemId != "server")
        {
          return new LoginResult() { Successful = false, Error = "Invalid system ID (system.SystemId)" };
        }
        log.Information("Basic validation passed");
        #endregion

        using (var uow = new UnitOfWork())
        {
          var employee = uow.Query<PER_Person>()
            .FirstOrDefault(s => s.IdNum == user.UserIdNum && s.Security != null && s.Security.IsActive);
          if (employee == null)
          {
            return new LoginResult() { Successful = false, Error = "Invalid user- check ID number" };
          }
          log.Information("Found user");

          var fullBranch = user.UserBranch.PadLeft(3, '0');
          var branch = uow.Query<BRN_Branch>()
            .FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == fullBranch && !s.IsClosed);
          if (branch == null)
          {
            return new LoginResult() { Successful = false, Error = "Invalid user- check branch number" };
          }
          log.Information("Found branch");
                    
          var token = UserToken.IssueNewToken(user.UserIdNum, user.UserBranch, TimeSpan.FromMinutes(60));
          var result = new LoginResult() { Successful = true, LoginToken = token };
          log.Information("Result: {@Result}", result);
          return result;
        }
      }
      catch (Exception err)
      {
        log.Error(err, "Login");
        return new LoginResult() { Successful = false, Error = "Unexpected server error" };
      }
     
    }

  }
}