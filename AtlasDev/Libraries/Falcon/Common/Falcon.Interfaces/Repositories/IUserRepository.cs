using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IUserRepository
  {
    IList<IUser> GetUsers(General.PersonType userType);
    bool AssignUser(long userId, string webUserId);
    IList<IPerson> GetActiveUsers();
    IPerson GetPerson(string userId);
    IPerson GetPerson(long personId);
    IPerson CheckLink(string userId);
    bool LinkUser(string idNo, string userId);
    void LinkUser(long personId, string userId);
    bool UnLinkUser(long personId, string userId);
    IPerson GetUserByOperatorCode(string operatorCode, string legacyBranchNum);
    IPerson GetBranchManager(string legacyBranchNum);
    IPerson GetSystemUser();
    List<IPerson> GetConsultants(long branchId);
    List<IPerson> List(long? branchId, string firstName, string lastName, string idNo);
    List<IBranch> GetLinkedBranches(string userId);
  }
}
