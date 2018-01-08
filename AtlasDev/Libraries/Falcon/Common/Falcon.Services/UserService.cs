using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Interfaces.Structures;
using System;

namespace Falcon.Common.Services
{
  public class UserService : IUserService
  {
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    /// <summary>
    /// Get specific user from core based on web user
    /// </summary>
    /// <returns></returns>

    public IPerson Get(string userId)
    {
     return _userRepository.GetPerson(userId);
    }
  }
}
