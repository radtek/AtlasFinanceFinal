using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Atlas.Common.Utils;
using AutoMapper;
using DevExpress.Xpo;
using Newtonsoft.Json;
using StackExchange.Redis;
using Falcon.Common.Structures;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;
using Atlas.Domain.Model;
using Atlas.Domain.Model.Biometric;
using Atlas.Enumerators;
using Falcon.Common.Repository.Properties;
using Falcon.Common.Structures.Branch;


namespace Falcon.Common.Repository
{
  public class UserRepository : IUserRepository
  {
    private const string Key = "falcon.user.cache";
    private readonly IDatabase _redis;
    private readonly IMappingEngine _mappingEngine;


    public UserRepository(IDatabase redis, IMappingEngine mappingEngine, IConfiguration mapperConfiguration)
    {
      _redis = redis;
      _mappingEngine = mappingEngine;

      mapperConfiguration.CreateMap<PER_Person, IPerson>();
      mapperConfiguration.CreateMap<BRN_Branch, IBranch>();
    }


    public IList<IUser> GetUsers(General.PersonType userType)
    {
      var users = new List<IUser>();

      var result = _redis.StringGet(Key);

      var redisResult = new List<User>();

      if (!result.IsNull)
        redisResult = JsonConvert.DeserializeObject<List<User>>(result);

      using (var uow = new UnitOfWork())
      {
        var userCount = new XPQuery<PER_Person>(uow).Count(p => p.PersonType.TypeId == (int)userType);

        users.AddRange(redisResult);

        if (userCount > redisResult.Count)
        {
          var maxValue = redisResult.Count == 0 ? 0 : redisResult.Max(p => p.UserId);
          var userCollection =
            new XPQuery<PER_Person>(uow).Where(
              p => p.PersonId > maxValue && p.PersonType.Type == userType && p.Security != null && p.Security.IsActive)
              .ToList();

          users.AddRange(userCollection.Select(user => new User
          {
            UserId = user.PersonId,
            FirstName = user.Firstname,
            LastName = user.Lastname
          }));
        }

        if ((userCount > redisResult.Count) || result.IsNull)
          _redis.StringSet(Key, JsonConvert.SerializeObject(users));
      }

      return users;
    }


    public List<IBranch> GetLinkedBranches(string userId)
    {
      using (var uow = new UnitOfWork())
      {
        var person =
          new XPQuery<PER_Person>(uow).Where(
            p => p.WebReference.ToString() == userId.ToString())
            .Distinct()
            .FirstOrDefault();
        if (person != null)
        {
            var branches = person.GetBranches.Where(b => b.Branch.Region != null && b.Branch.Company != null).Select(b=>b.Branch).ToList();
          var regions = person.GetRegions.Select(r => r.Region).ToList();
          branches.AddRange(new XPQuery<BRN_Branch>(uow)
            .Where(b => b.Company != null && regions.Contains(b.Region))
            .OrderBy(s => s.Company.Name).Distinct()
            .ToList());
          var tmpBranches = new List<Branch>();
          branches.Distinct().ToList().ForEach(b =>
          {
            tmpBranches.Add(new Branch
            {
              BranchId = b.BranchId,
              Name = b.Company.Name,
              Region = b.Region.LegacyRegionCode,
              LegacyBranchNum = b.LegacyBranchNum,
              RegionId = b.Region.RegionId
            });
          });

          return _mappingEngine.Map<List<Branch>, List<IBranch>>(tmpBranches.OrderBy(b => b.Name).ToList());
        }
        return new List<IBranch>();
      }
    }


    public bool AssignUser(long userId, string webUserId)
    {
      throw new NotImplementedException();
    }


    public IList<IPerson> GetActiveUsers()
    {
      var users = new List<IPerson>();
      using (var uow = new UnitOfWork())
      {
        // Short circuit to the bio table, as it should have the latest data's (afrikaans acent).
        var activeUsers =
          new XPQuery<BIO_LogRequest>(uow).Where(p => p.Person != null && p.Person.Security != null)
            .Select(p => p.Person)
            .Distinct()
            .ToList();
        users.AddRange(activeUsers.Select(user => new Person
        {
          PersonId = user.PersonId,
          IdNum = user.IdNum,
          Branch = _mappingEngine.Map<BRN_Branch, IBranch>(user.Branch),
          FullName = string.Format("{0} {1}", user.Firstname, user.Lastname),
          Firstname = user.Firstname,
          Lastname = user.Lastname,
          LegacyClientCode = user.Security.LegacyOperatorId,
          WebReference = user.WebReference
        }));
        return users;
      }
    }


    public IPerson GetPerson(string userId)
    {
      var rawSql = new RawSql();
      var connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
      if (string.IsNullOrEmpty(connectionString))
      {
        using (var uow = new UnitOfWork())
        {
          uow.LockingOption = LockingOption.None;
          uow.TrackPropertiesModifications = false;
          // Check to see if perhaps its already linked not via FP
          var personQuery =
            new XPQuery<PER_Person>(uow).Where(
              p => p.WebReference != null && p.WebReference.ToString() == userId.ToString()).AsQueryable();
          var person = personQuery.FirstOrDefault();
          if (person != null)
          {
            return new Person
            {
              PersonId = person.PersonId,
              IdNum = person.IdNum,
              FullName = string.Format("{0} {1}", person.Firstname, person.Lastname),
              Firstname = person.Firstname,
              Lastname = person.Lastname,
              LegacyClientCode = person.Security == null ? string.Empty : person.Security.LegacyOperatorId,
              WebReference = person.WebReference,
              Email = person.Email,
              Branch = new Branch
              {
                BranchId = person.Branch == null ? 0 : person.Branch.BranchId,
                Name = person.Branch == null ? string.Empty : person.Branch.Company.Name,
                Region = person.Branch == null ? string.Empty : person.Branch.Region.LegacyRegionCode,
                RegionId = person.Branch == null ? 0 : person.Branch.Region.RegionId,
                LegacyBranchNum = person.Branch == null ? string.Empty : person.Branch.LegacyBranchNum
              }
            };
          }
        }
      }
      else
      {
        var person =
          rawSql.ExecuteObject<GetPersonRecord>(string.Format(Resources.GetPersonByWebReferenceQuery, userId),
            connectionString.Replace("XpoProvider=Postgres;", "")).FirstOrDefault();

        if (person != null)
        {
          return new Person
           {
             PersonId = person.PersonId,
             IdNum = person.IdNum,
             FullName = string.Format("{0} {1}", person.Firstname, person.Lastname),
             Firstname = person.Firstname,
             Lastname = person.Lastname,
             LegacyClientCode = person.LegacyClientCode,
             WebReference = person.WebReference,
              Email = person.Email,
             Branch = new Branch
             {
               BranchId = person.BranchId,
               Name = person.Branch,
               Region = person.LegacyRegionCode,
               RegionId = person.RegionId,
               LegacyBranchNum = person.LegacyBranchNum
             }
           };
        }
      }
      return null;
    }

    public IPerson GetPerson(long personId)
    {
      using (var uow = new UnitOfWork())
      {
        uow.LockingOption = LockingOption.None;
        uow.TrackPropertiesModifications = false;
        // Check to see if perhaps its already linked not via FP
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (person != null)
        {
          return new Person
          {
            PersonId = person.PersonId,
            IdNum = person.IdNum,
            FullName = $"{person.Firstname} {person.Lastname}",
            Firstname = person.Firstname,
            Lastname = person.Lastname,
            LegacyClientCode = person.Security == null ? string.Empty : person.Security.LegacyOperatorId,
            WebReference = person.WebReference,
            Email = person.Email,
            Branch = new Branch
            {
              BranchId = person.Branch?.BranchId ?? 0,
              Name = person.Branch == null ? string.Empty : person.Branch.Company.Name,
              Region = person.Branch == null ? string.Empty : person.Branch.Region.LegacyRegionCode,
              RegionId = person.Branch?.Region.RegionId ?? 0,
              LegacyBranchNum = person.Branch == null ? string.Empty : person.Branch.LegacyBranchNum
            }
          };
        }
      }
      return null;
    }

    private class GetPersonRecord
    {
      public long PersonId { get; set; }
      public string IdNum { get; set; }
      public string Firstname { get; set; }
      public string Lastname { get; set; }
      public string LegacyClientCode { get; set; }
      public string WebReference { get; set; }
      public long BranchId { get; set; }
      public string Branch { get; set; }
      public long RegionId { get; set; }
      public string LegacyRegionCode { get; set; }
      public string LegacyBranchNum { get; set; }
      public string Email { get; set; }
    }


    /// <summary>
    /// Checks to see if a user has been linked.
    /// </summary>
    /// <returns></returns>
    public IPerson CheckLink(string userId)
    {
      using (var uow = new UnitOfWork())
      {
        var user = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.WebReference.ToString() == userId.ToString());
        if (user == null)
          return null;

        return _mappingEngine.Map<PER_Person, IPerson>(user);
      }
    }


    /// <summary>
    /// Link Falcon user to backend db user
    /// </summary>
    /// <returns></returns>
    public bool LinkUser(string idNo, string userId)
    {
      using (var uow = new UnitOfWork())
      {
        var users = GetActiveUsers();
        var countUser = users.Count(p => p.IdNum == idNo);

        if (countUser > 1)
          throw new Exception(string.Format("Link User: IdNo {0}, UserId: {1} appears to have duplicate entries.", idNo,
            userId));

        var user = users.FirstOrDefault(p => p.IdNum == idNo);

        if (user == null)
          throw new Exception(string.Format("Link User: IdNo {0}, UserId: {1} does not exist in the database.", idNo,
            userId));

        var dbUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == user.PersonId);

        if (dbUser != null)
        {
          dbUser.WebReference = userId;
          dbUser.Save();
        }
        uow.CommitChanges();

        return true;
      }
    }


    /// <summary>
    /// Link Falcon user to backend db user
    /// </summary>
    /// <returns></returns>
    public void LinkUser(long personId, string userId)
    {
      using (var uow = new UnitOfWork())
      {
        var dbUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (dbUser != null)
        {
          dbUser.WebReference = userId;
          uow.CommitChanges();
        }
      }
    }


    /// <summary>
    /// UnLink falcon user to backend db user
    /// </summary>
    /// <returns></returns>
    public bool UnLinkUser(long personId, string userId)
    {
      using (var uow = new UnitOfWork())
      {
        var user = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId && p.WebReference == userId);

        if (user == null)
          throw new Exception(string.Format("Person {0} does not exist in the database.", personId));

        user.WebReference = null;
        uow.CommitChanges();

        return true;
      }
    }

    public IPerson GetUserByOperatorCode(string operatorCode, string legacyBranchNum)
    {
      using (var uow = new UnitOfWork())
      {
        var person =
          new XPQuery<PER_Person>(uow).FirstOrDefault(
            p =>
              p.Security != null && p.Security.IsActive && p.Security.LegacyOperatorId == operatorCode &&
              p.Branch.LegacyBranchNum == legacyBranchNum);

        if (person == null)
        {
          return null;
        }

        return new Person
        {
          PersonId = person.PersonId,
          IdNum = person.IdNum,
          Username = person.Security.Username,
          FullName = string.Format("{0} {1}", person.Firstname, person.Lastname),
          Firstname = person.Firstname,
          Lastname = person.Lastname,
          LegacyClientCode = person.Security.LegacyOperatorId,
          WebReference = person.WebReference,
          Branch = new Branch
          {
            BranchId = person.Branch.BranchId,
            Name = person.Branch.Company.Name,
            Region = person.Branch.Region.LegacyRegionCode,
            RegionId = person.Branch.Region.RegionId
          }
        };
      }
    }

    public IPerson GetBranchManager(string legacyBranchNum)
    {
      using (var uow = new UnitOfWork())
      {
        var person =
          new XPQuery<PER_Person>(uow).Where(
            p =>
              p.Security != null && p.Security.IsActive && p.Branch.LegacyBranchNum == legacyBranchNum)
            .ToList()
            .FirstOrDefault(p =>
              p.GetRoles.Any(r => r.RoleType.Type == General.RoleType.Branch_Manager));

        if (person == null)
        {
          return null;
        }

        return new Person
        {
          PersonId = person.PersonId,
          IdNum = person.IdNum,
          Username = person.Security.Username,
          FullName = string.Format("{0} {1}", person.Firstname, person.Lastname),
          Firstname = person.Firstname,
          Lastname = person.Lastname,
          LegacyClientCode = person.Security.LegacyOperatorId,
          WebReference = person.WebReference,
          Branch = new Branch
          {
            BranchId = person.Branch.BranchId,
            Name = person.Branch.Company.Name,
            Region = person.Branch.Region.LegacyRegionCode,
            RegionId = person.Branch.Region.RegionId
          }
        };
      }
    }

    public IPerson GetSystemUser()
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)General.Person.System);

        if (person == null)
        {
          return null;
        }

        return new Person
        {
          PersonId = person.PersonId,
          IdNum = person.IdNum,
          Username = person.Security.Username,
          FullName = string.Format("{0} {1}", person.Firstname, person.Lastname),
          Firstname = person.Firstname,
          Lastname = person.Lastname,
          LegacyClientCode = person.Security.LegacyOperatorId,
          WebReference = person.WebReference,
          Branch = new Branch
          {
            BranchId = person.Branch.BranchId,
            Name = person.Branch.Company.Name,
            Region = person.Branch.Region.LegacyRegionCode,
            RegionId = person.Branch.Region.RegionId
          }
        };
      }
    }

    /// <summary>
    /// Get the consultants, that have finger prints and that have been allocated to the certain branch.
    /// </summary>
    /// <param name="branchId"></param>
    public List<IPerson> GetConsultants(long branchId)
    {
      var users = new List<IPerson>();
      using (var uow = new UnitOfWork())
      {
        var activeUsers =
          new XPQuery<BIO_LogRequest>(uow).Where(
            p => p.Person != null && p.Person.Security != null && p.Person.Branch.BranchId == branchId)
            .Select(p => p.Person)
            .OrderBy(s => s.Lastname).ThenBy(s => s.Firstname)
            .Distinct()
            .ToList();
        users.AddRange(activeUsers.Select(user => new Person
        {
          PersonId = user.PersonId,
          IdNum = user.IdNum,
          FullName = string.Format("{0} {1}", user.Firstname, user.Lastname),
          Firstname = user.Firstname,
          Lastname = user.Lastname,
          LegacyClientCode = user.Security.LegacyOperatorId,
          WebReference = user.WebReference,
          Branch = new Branch
          {
            BranchId = user.Branch.BranchId,
            Name = user.Branch.Company.Name,
            Region = user.Branch.Region.LegacyRegionCode,
            RegionId = user.Branch.Region.RegionId
          }
        }));
        return users;
      }
    }


    public List<IPerson> List(long? branchId, string firstName, string lastName, string idNo)
    {
      var userCollection = new List<IPerson>();
      using (var uow = new UnitOfWork())
      {
        var activeUsers = new XPQuery<BIO_LogRequest>(uow).Where(p => p.Person != null && p.Person.Security != null);

        if (branchId != null)
          activeUsers = activeUsers.Where(p => p.Person.Branch.Company.CompanyId == branchId);

        if (!string.IsNullOrEmpty(firstName))
        {
          firstName = firstName.ToUpper();
          activeUsers = activeUsers.Where(p => p.Person.Firstname.ToUpper() == firstName);
        }

        if (!string.IsNullOrEmpty(lastName))
        {
          lastName = lastName.ToUpper();
          activeUsers = activeUsers.Where(p => p.Person.Lastname.ToUpper() == lastName);
        }

        if (!string.IsNullOrEmpty(idNo))
          activeUsers = activeUsers.Where(p => p.Person.IdNum == idNo);

        var users = activeUsers.Select(p => p.Person).OrderBy(s => s.Lastname).ThenBy(s => s.Firstname).Distinct().ToList();

        foreach (var user in users)
        {
          var person = new Person
          {
            PersonId = user.PersonId,
            IdNum = user.IdNum,
            FullName = string.Format("{0} {1}", user.Firstname, user.Lastname),
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            LegacyClientCode = user.Security.LegacyOperatorId,
            WebLinked = user.WebReference != null && user.WebReference.ToString() != string.Empty.ToString(),
            WebReference = user.WebReference
          };

          if (user.Branch != null)
          {
            person.Branch = new Branch
            {
              BranchId = user.Branch.BranchId,
              Name = user.Branch.Company.Name,
              Region = user.Branch.Region.LegacyRegionCode,
              LegacyBranchNum = user.Branch.LegacyBranchNum,
              RegionId = user.Branch.Region.RegionId
            };
          }

          userCollection.Add(person);
        }

        return userCollection;
      }
    }

  }
}