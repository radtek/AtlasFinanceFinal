using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using AutoMapper;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Interfaces.Structures.Reports.General;
using Falcon.Common.Structures.ASS;
using Falcon.Common.Structures.Branch;
using Falcon.Common.Structures.Report.General;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Falcon.Common.Repository
{
  public class CompanyRepository : ICompanyRepository
  {
    private readonly IDatabase _redis;
    private readonly IMappingEngine _mappingEngine;

    private const string REDIS_KEY_PERSON_REGION_BRANCH =
      "falcon.stream.reporting.performancereport.person.{0}.region.branch"; // userid, startrange, endrange

    public CompanyRepository(IDatabase redis, IMappingEngine mappingEngine, IConfiguration mapperConfiguration)
    {
      _redis = redis;
      _mappingEngine = mappingEngine;

      #region mappings

      mapperConfiguration.CreateMap<RegionBranch, IRegionBranch>().ReverseMap();
      mapperConfiguration.CreateMap<BranchServer, IRegionBranch>().ReverseMap();

      #endregion
    }

    public ICollection<IBranch> GetActiveBranches(bool active = true)
    {
      var branchCollection = new List<IBranch>();

      using (var uow = new UnitOfWork())
      {
        var branches = new XPQuery<BRN_Branch>(uow)
          .Where(p => active == !p.IsClosed && p.Company != null && p.Region != null)
          .OrderBy(s => s.Company.Name).ToList();
        branchCollection.AddRange(branches.Select(branch => new Branch()
        {
          BranchId = branch.BranchId,
          Name = branch.Company.Name,
          Region = branch.Region.Description,
          RegionId = branch.Region.RegionId,
          LegacyBranchNum = branch.LegacyBranchNum
        }));
      }
      return branchCollection;
    }

    public ICollection<IBranch> GetAllBranches()
    {
      var branchCollection = new List<IBranch>();

      using (var uow = new UnitOfWork())
      {
        var branches = new XPQuery<BRN_Branch>(uow)
          .Where(p => p.Company != null && p.Region != null)
          .OrderBy(s => s.Company.Name).ToList();
        branchCollection.AddRange(branches.Select(branch => new Branch()
        {
          BranchId = branch.BranchId,
          Name = branch.Company.Name,
          Region = branch.Region.Description,
          RegionId = branch.Region.RegionId,
          LegacyBranchNum = branch.LegacyBranchNum
        }));
      }
      return branchCollection;
    }

    public ICollection<IBranch> GetBranchesByIds(IList<long> branchIds)
    {
      var branchCollection = new List<IBranch>();

      using (var uow = new UnitOfWork())
      {
        var branches = new XPQuery<BRN_Branch>(uow)
          .Where(b => branchIds.Contains(b.BranchId) && b.Company != null && b.Region != null)
          .OrderBy(s => s.Company.Name).Select(b => new {b.BranchId, b.Company, b.Region, b.LegacyBranchNum}).ToList();
        branchCollection.AddRange(branches.Select(branch => new Branch
        {
          BranchId = branch.BranchId,
          Name = branch.Company.Name,
          Region = branch.Region.Description,
          RegionId = branch.Region.RegionId,
          LegacyBranchNum = branch.LegacyBranchNum
        }));
      }
      return branchCollection;
    }

    public List<IRegionBranch> GetPersonRegionBranches()
    {
      var regionBranches = new List<RegionBranch>();
      if (_redis.KeyExists(REDIS_KEY_PERSON_REGION_BRANCH))
      {
        var redisResult = _redis.StringGet(REDIS_KEY_PERSON_REGION_BRANCH);
        if (!redisResult.IsNullOrEmpty)
          regionBranches = JsonConvert.DeserializeObject<List<RegionBranch>>(redisResult);
      }
      if (regionBranches.Count == 0)
      {
        regionBranches = new List<RegionBranch>
        {
          new RegionBranch()
          {
            Name = "All Branches",
            MultiSelectGroup = true
          }
        };

        using (var uow = new UnitOfWork())
        {
          new XPQuery<Region>(uow).Where(r => r != null)
            .ToList()
            .Select(r => new {r.RegionId, r.Description})
            .OrderBy(r => r.RegionId)
            .ToList()
            .ForEach(r =>
            {
              regionBranches.Add(new RegionBranch()
              {
                Name = r.Description,
                MultiSelectGroup = true
              });

              new XPQuery<BRN_Branch>(uow).Where(b => b.Region.RegionId == r.RegionId && !b.IsClosed)
                .ToList()
                .ForEach(b =>
                {
                  regionBranches.Add(new RegionBranch()
                  {
                    BranchId = b.BranchId,
                    Name = b.Company.Name,
                    Ticked = false,
                    MultiSelectGroup = false
                  });
                });

              regionBranches.Add(new RegionBranch()
              {
                MultiSelectGroup = false
              });
            });
        }

        regionBranches.Add(new RegionBranch()
        {
          MultiSelectGroup = false
        });

        _redis.StringSet(REDIS_KEY_PERSON_REGION_BRANCH, JsonConvert.SerializeObject(regionBranches));
        _redis.KeyExpire(REDIS_KEY_PERSON_REGION_BRANCH, new TimeSpan(0, 30, 0));
      }
      return _mappingEngine.Map<List<RegionBranch>, List<IRegionBranch>>(regionBranches);
    }

    public bool AssociateUser(long branchId, long personId)
    {
      using (var uow = new UnitOfWork())
      {
        var branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(p => p.BranchId == branchId);

        if (branch == null)
          throw new Exception(string.Format("Branch {0} does not exist", branchId));

        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

        if (person == null)
          throw new Exception(string.Format("Person {0} does not exist", personId));

        person.Branch = branch;

        person.Save();

        uow.CommitChanges();

      }
      return true;
    }


    public ICollection<IBranchServer> GetBranchSyncStatus(ICollection<long> branchIds)
    {
      using (var uow = new UnitOfWork())
      {
        var branchServers =
          new XPQuery<ASS_BranchServer>(uow).Where(b => branchIds.Contains(b.Branch.BranchId))
            .Select(b => new {b.Branch, b.BranchServerId, b.LastSyncDT})
            .ToList();

        var branchServerDtos = branchServers.Select(b => new BranchServer
        {
          BranchId = b.Branch.BranchId,
          BranchName = b.Branch.Company.Name,
          BranchServerId = b.BranchServerId,
          LastSyncDate = b.LastSyncDT
        }).OrderBy(b => b.BranchName).ToList();

        return _mappingEngine.Map<ICollection<IBranchServer>>(branchServerDtos);
      }
    }

    public ICollection<IHost> GetAllHosts(General.Host? hostType = null)
    {
      var hosts = new List<IHost>();
      using (var uow = new UnitOfWork())
      {
        IQueryable<Host> hostQuery;

        if (hostType == null)
        {
          hostQuery = new XPQuery<Host>(uow).OrderBy(h => h.Description).AsQueryable();
        }
        else
        {
          hostQuery = new XPQuery<Host>(uow).Where(h => h.HostId == hostType.ToInt()).OrderBy(h => h.Description).AsQueryable();
        }

        hosts.AddRange(hostQuery.ToList().Select(h => new Structures.Host
        {
          HostId = h.HostId,
          HostName = h.Description,
          Type = h.Type
        }));
      }

      return hosts;
    }
  }
}