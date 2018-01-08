using System.Collections.Generic;
using System.Linq;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using AutoMapper;
using DevExpress.Xpo;

namespace Falcon.Service.Business
{
  public class UserUtility
  {
    public static List<PER_PersonDTO> GetUsers(long? branchId, long? userId)
    {
      using (var uow = new UnitOfWork())
      {
        var userQuery = new XPQuery<PER_Person>(uow).Where(p => p.Security.IsActive && p.PersonType.Type == General.PersonType.Employee);
        if (branchId != null && branchId > 0)
          userQuery = userQuery.Where(p => p.Branch.Company.CompanyId == branchId);
        if (userId != null && userId > 0)
          // TODO: this will change according to how we sync both DB's for users
          userQuery = userQuery.Where(p => p.PersonId == userId);

        return Mapper.Map<List<PER_Person>, List<PER_PersonDTO>>(userQuery.ToList());
      }
    }
  }
}