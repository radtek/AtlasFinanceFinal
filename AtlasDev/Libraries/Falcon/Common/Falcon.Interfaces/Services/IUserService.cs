using System;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Services
{
  public interface IUserService
  {
    IPerson Get(string userId);
  }
}
