using System;

namespace Falcon.Gyrkin.Controllers.Api.Models
{
  public class UserModel
  {
    public class AssignModel
    {
      public long UserId { get; set; }
      public string WebUserId { get; set; }
    }

    public class CheckLinkModel
    {
      public string UserId { get; set; }
    }

    public class LinkUserModel
    {
      public string IDNo { get; set; }
      public string UserId { get; set; }
    }

    public class LinkUserV2Model
    {
      public long PersonId { get; set; }
      public string UserId { get; set; }
    }

    public class ConsulantQueryModel
    {
      public long BranchId { get; set; }
    }

    public class UserLinkedBranchesQueryModel
    {
      public string UserId { get; set; }
    }

    public class UserDetailQueryModel
    {
      public string UserId { get; set; }
    }

    public class UserListQueryModel
    {
      public long? BranchId { get; set; }
      public string IdNo { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }
      
    }
  }
}