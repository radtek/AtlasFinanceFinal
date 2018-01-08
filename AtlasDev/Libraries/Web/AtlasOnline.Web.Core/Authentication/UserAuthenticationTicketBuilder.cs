using System;
using System.Web.Security;

namespace AutoFac.Web.Core.Authentication
{
  public class UserAuthenticationTicketBuilder
  {
    public class User
    {
      public int UserId { get; set; }

      public string FirstName { get; set; }

      public string DisplayName { get; set; }

      public string Email { get; set; }
    }

    public class UserInfo
    {
      public int UserId { get; set; }

      public string DisplayName { get; set; }

      public string UserIdentifier { get; set; }
    }

    /// <summary>
    /// Create authentication ticket based on user model
    /// </summary>
    /// <param name="user">User model</param>
    /// <returns>Forms Authentication Ticket</returns>
    public static FormsAuthenticationTicket CreateAuthenticationTicket(User user)
    {
      UserInfo userInfo = CreateUserContextFromUser(user);

      var ticket = new FormsAuthenticationTicket(
          1,
          user.FirstName,
          DateTime.Now,
          DateTime.Now.Add(FormsAuthentication.Timeout),
          false,
          userInfo.ToString());

      return ticket;
    }

    /// <summary>
    /// Create a user info context from a user model
    /// </summary>
    /// <param name="user">User Model</param>
    /// <returns>User Info Context</returns>
    private static UserInfo CreateUserContextFromUser(User user)
    {
      var userContext = new UserInfo
      {
        UserId = user.UserId,
        DisplayName = user.DisplayName,
        UserIdentifier = user.Email

        //RoleName = Enum.GetName(typeof(UserRoles), user.RoleId)
      };
      return userContext;
    }
  }
}