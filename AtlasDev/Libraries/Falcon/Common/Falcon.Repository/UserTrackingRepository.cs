using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StackExchange.Redis;
using DevExpress.Xpo;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Domain.Security;
using Atlas.Enumerators;
using Falcon.Common.Structures.UserTracking;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;


namespace Falcon.Common.Repository
{
  public class UserTrackingRepository : IUserTrackingRepository
  {
    private readonly IDatabase _redis;
    public UserTrackingRepository(IDatabase redis)
    {
      _redis = redis;
    }

    public IList<IUserTrackingUserMovement> GetUserMovements(long? userId, long? branchId, DateTime startRange,
      DateTime endRange)
    {
      var movements = new List<IUserTrackingUserMovement>();
      using (var uow = new UnitOfWork())
      {
        IQueryable<COR_LogAppUsage> logCollection = new XPQuery<COR_LogAppUsage>(uow)
          .Where(p => p.EventDT >= startRange && p.EventDT <= endRange.AddDays(+1)).OrderByDescending(p => p.EventDT);

        if (branchId != null)
          logCollection = logCollection.Where(p => p.AppUsageId.BranchCode.BranchId == branchId);

        if (userId != null)
          logCollection = logCollection.Where(p => p.AppUsageId.User.Person.PersonId == userId);

        var cleanedCollection = new List<COR_LogAppUsage>();

        var regEx = new Regex(@"((^0[A-Z]{0,1}\d{1,2})|(^[A-Z]{1,1}\d{1})|(^\d{2,3}))\-00\-\d{2,2}$");

        foreach (var log in logCollection)
        {
          if (regEx.Match(log.AppUsageId.Machine.MachineName).Success)
            cleanedCollection.Add(log);
        }

        movements.AddRange((from use in cleanedCollection
          where use.AppUsageId.User.Person != null
          select new UserTrackingUserMovement()
          {
            UsageId = use.LogAppUsageId,
            EventDate = use.EventDT,
            Event = use.AtlasEvent.ToStringEnum(),
            Branch =
              use.AppUsageId.BranchCode != null
                ? Regex.Replace(use.AppUsageId.BranchCode.Company.Name, @"\r\n?|\n", string.Empty)
                : string.Empty,
            FirstName = use.AppUsageId.User.Person.Firstname,
            LastName = use.AppUsageId.User.Person.Lastname,
            UserId = use.AppUsageId.User.Person.PersonId,
            OperatorCode = use.AppUsageId.User.LegacyOperatorId,
            Machine = use.AppUsageId.Machine.MachineName
          }));
      }

      return movements.OrderBy(p => p.EventDate).ToList();
    }

    public IList<IUserTrackingBranchMovement> GetBranchMovements(long branchId, DateTime startRange, DateTime endRange)
    {
      throw new NotImplementedException();
    }

    public bool SavePin(long userId, Tracking.AlertType alertType,
      Tracking.SeverityClassification severityClassification, Tracking.Elapse elapse, int value, string notify)
    {
      using (var uow = new UnitOfWork())
      {
        var rule = SaveOrUpdateRule(uow, userId, alertType, severityClassification, elapse, value);

        var pinnedUser = new XPQuery<UserTracking_PinnedUsers>(uow).FirstOrDefault(p => p.Active &&
                                                                                        p.Rule.RuleSetId ==
                                                                                        rule.RuleSetId &&
                                                                                        p.User.PersonId == userId);

        if (pinnedUser == null)
        {
          pinnedUser = new UserTracking_PinnedUsers(uow)
          {
            Active = true,
            CreateDate = DateTime.Now,
            Rule = rule,
            User = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userId),
            ViolationCount = 0
          };

          pinnedUser.Save();
          uow.CommitChanges();
        }
        _redis.StringSet(string.Format("{0}-{1}", pinnedUser.PinnedUserId, pinnedUser.Rule.RuleSetId), notify);
      }
      return true;
    }


    public IList<IUserTrackingPinnedUser> GetPinned(bool active)
    {
      var pinnedUserList = new List<IUserTrackingPinnedUser>();
      using (var uow = new UnitOfWork())
      {
        var pinnedUsers = new XPQuery<UserTracking_PinnedUsers>(uow).Where(p => p.Active == active).ToList();

        pinnedUserList.AddRange(pinnedUsers.Select(pin => new UserTrackingPinnedUser
        {
          FirstName = pin.User.Firstname, LastName = pin.User.Lastname, PinnedUserId = pin.PinnedUserId, ViolationCount = pin.ViolationCount, UserId = pin.User.PersonId, Active = pin.Active, CreateDate = pin.CreateDate, RuleSet = new UserTrackingPinnedUserRuleset()
          {
            AlertType = pin.Rule.AlertType, Elapse = pin.Rule.Elapse, RuleSetId = pin.Rule.RuleSetId, SeverityClassification = pin.Rule.SeverityClassification, Value = pin.Rule.Value
          }
        }));
      }
      return pinnedUserList;
    }


    public bool ResetViolations(bool active)
    {
      using (var uow = new UnitOfWork())
      {
        var pinnedUsers = new XPQuery<UserTracking_PinnedUsers>(uow).Where(p => p.Active == active).ToList();
        foreach (var pin in pinnedUsers)
          pin.ViolationCount = 0;

        uow.CommitChanges();
      }
      return true;
    }


    public bool UpdateViolationCount(long pinnedUserId, long ruleSetId, int violationCount)
    {
      using (var uow = new UnitOfWork())
      {
        var pinnedUser = new XPQuery<UserTracking_PinnedUsers>(uow).FirstOrDefault(p => p.PinnedUserId == pinnedUserId && p.Rule.RuleSetId == ruleSetId && p.Active);

        if (pinnedUser != null)
        {
          pinnedUser.ViolationCount = violationCount;
        }
        uow.CommitChanges();
      }
      return true;
    }


    public bool RemovePin(long pinnedUserId)
    {
      using(var uow = new UnitOfWork())
      {
        var pin = new XPQuery<UserTracking_PinnedUsers>(uow).FirstOrDefault(p => p.PinnedUserId == pinnedUserId);
        if (pin != null)
        {
          pin.Active = false;
          pin.Save();

          uow.CommitChanges();
        }
      }
      return true;
    }
    

    internal UserTracking_RuleSet SaveOrUpdateRule(UnitOfWork uow, long userId, Tracking.AlertType alertType, Tracking.SeverityClassification severityClassification, Tracking.Elapse elapse, int value)
    {
      var ruleSet = new XPQuery<UserTracking_RuleSet>(uow)
        .FirstOrDefault(p => p.AlertType == alertType && p.SeverityClassification == severityClassification &&
          p.Elapse == elapse && p.Value == value);

      if (ruleSet != null)
      {
        // Check to see if rules are applied to other users other than provided
        var appliedRules = new XPQuery<UserTracking_PinnedUsers>(uow).FirstOrDefault(p => p.User.PersonId != userId && p.Rule.RuleSetId == ruleSet.RuleSetId);

        if (appliedRules != null)
        {
          ruleSet = new UserTracking_RuleSet(uow)
          {
            AlertType = alertType,
            SeverityClassification = severityClassification,
            Elapse = elapse,
            Value = value
          };
        }
      }
      else
      {
        ruleSet = new UserTracking_RuleSet(uow)
        {
          AlertType = alertType,
          SeverityClassification = severityClassification,
          Elapse = elapse,
          Value = value
        };
      }
      uow.CommitChanges();
      return ruleSet;
    }
    
  }
}
