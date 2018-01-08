using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Atlas.Domain.Security;
using Atlas.Domain.Model;
using DevExpress.Xpo;
using Atlas.Domain.DTO;

namespace Atlas.Security.User
{
  public static class UserProfile
  {
    /// <summary>
    /// Logs a user in and authenticates the user
    /// </summary>
    public static PER_PersonDTO LoginUser(string userName, string password, out string errorMessage)
    {
      errorMessage = string.Empty;

      using (var UoW = new UnitOfWork())
      {
        var user = (from e in new XPQuery<PER_Person>(UoW) where e.Security.Username == userName select e).FirstOrDefault();

        if (user == null)
        {
          errorMessage = "User was not found in the security store";
          return null;
        }

        if (user.Security.IsLocked)
          return AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(user);

        // Set defaults as false
        user.Security.IsLocked = false;
        user.Security.InvalidUserNameOrPassword = false;

        if (Cryptography.CheckPassword(password, user.Security.Salt, user.Security.Hash))
        {
          //Update the last logged in flag.
          user.Security.LastLoggedIn = DateTime.Now;
          user.Security.LoggedIn = true;
          user.Security.IsLocked = false;
          user.Security.LogInAttemptCount = 0;

          return AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(user);
        }
        else
        {
          // Increment bad try count
          user.Security.LogInAttemptCount += 1;

          // Check to see if we passed 3, account is locked
          if (user.Security.LogInAttemptCount >= 3)
          {
            user.Security.IsLocked = true;
          }

          UoW.CommitChanges();

          user.Security.InvalidUserNameOrPassword = true;
                    
          return AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(user);
        }
      }
    }

    /// <summary>
    /// Change the user's password
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="currentPassword"></param>
    /// <param name="newPassword"></param>
    /// <returns></returns>
    public static PER_PersonDTO ChangePassword(string userName, string currentPassword, string newPassword)
    {
      PER_PersonDTO user = null;
      using (var UoW = new UnitOfWork())
      {
        var resultUser = (from e in new XPQuery<PER_Person>(UoW) where e.Security.Username == userName select e).FirstOrDefault();

        // If the user result is null we cant continue.
        if (resultUser == null)
        {
          throw new SystemException("User was not found in the security store.");
        }

        // Check to ensure that the user knows their existing password
        if (Cryptography.CheckPassword(currentPassword, resultUser.Security.Salt, resultUser.Security.Hash))
        {

          // Generate new salt and hash of password
          Tuple<string, string> result = Cryptography.HashPassword(newPassword);

          // Create new unit of work
          resultUser.Security.Salt = result.Item1;
          resultUser.Security.Hash = result.Item2;

          user = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(resultUser);

          // Save immediately.
          UoW.CommitChanges();
        }
        else
        {
          throw new SystemException("The current password supplied the user is invalid");
        }
      }
      return user;
    }

    /// <summary>
    /// Resets the user profiles password
    /// </summary>
    public static PER_Person ForgotPassword(string userName)
    {
      PER_Person user = null;
      using (var UoW = new UnitOfWork())
      {
        user = (from e in new XPQuery<PER_Person>(UoW) where e.Security.Username == userName select e).FirstOrDefault();

        // if we cannot find the user we cant continue.
        if (user == null)
          throw new SystemException("User was not found in the security store.");


        //  Generate random password
        string randomPassword = Guid.NewGuid().ToString().Replace("-", "");

        //  Change the password to the randomized one.
        Tuple<string, string> result = Cryptography.HashPassword(randomPassword);


        // Create new unit of work
        user.Security.Salt = result.Item1;
        user.Security.Hash = result.Item2;

        UoW.CommitChanges();
        // Regenerate hardware verification file



        // Insert notification code
      }
      return user;
    }
  }
}