using System;
using System.Collections.Generic;

namespace Falcon.Models
{
  public class UserModel
  {
    public class WebUser
    {
      public Guid Id { get; set; }
      public string UserName { get; set; }
      public string Email { get; set; }
      public bool Linked { get; set; }
      public long? PersonId { get; set; }
      public DateTime? LastLogin { get; set; }
      public DateTime Created { get; set; }

      public List<WebRole> Roles { get; set; }
    }

    public class WebRole
    {
      public string RoleName { get; set; }
      public string ClaimSignature { get; set; }
    }

    public class SaveRoleModel
    {
      public Guid UserId { get; set; }
      public List<WebRole> Roles { get; set; }
    }
  }
}