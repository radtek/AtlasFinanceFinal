namespace Falcon.Common.Interfaces.Structures
{
  public interface IUser
  {
    long UserId { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
  }
}
