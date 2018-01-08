using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public class User : IUser
  {
    public long UserId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
  }
}
