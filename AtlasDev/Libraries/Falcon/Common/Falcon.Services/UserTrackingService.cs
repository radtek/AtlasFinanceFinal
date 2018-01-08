using System;
using System.Collections.Generic;
using System.Linq;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Structures.UserTracking;

namespace Falcon.Common.Services
{
  public class UserTrackingService : IUserTrackingService
  {
    private readonly IUserTrackingRepository _userTrackingRepository;
    public UserTrackingService(IUserTrackingRepository userTrackingRepository)
    {
      _userTrackingRepository = userTrackingRepository;
    }

    public IList<IUserTrackingPinnedUser> GetViolations(DateTime startDate, DateTime endDate)
    {
      var userTrackingPinnedUser = new List<IUserTrackingPinnedUser>();

      var pinnedUsers = _userTrackingRepository.GetPinned(true).ToList();

      foreach (var pin in pinnedUsers)
      {
        var trackingCollections = _userTrackingRepository.GetUserMovements(pin.UserId, null, startDate, endDate);
        if (trackingCollections.Count > 0)
        {
          var lastEventDate = trackingCollections.OrderByDescending(p => p.EventDate).FirstOrDefault();

          if (lastEventDate != null)
          {
            var span = DateTime.Now - lastEventDate.EventDate;

            switch (pin.RuleSet.Elapse)
            {
              case Atlas.Enumerators.Tracking.Elapse.Minutes:
                var totalMinutes = span.TotalMinutes;
                if (totalMinutes > pin.RuleSet.Value)
                {
                  var totalViolations = ((int)(Math.Floor(totalMinutes / pin.RuleSet.Value)));
                  userTrackingPinnedUser.Add(new UserTrackingPinnedUser()
                  {
                    Active = pin.Active,
                    CreateDate = pin.CreateDate,
                    FirstName = pin.FirstName,
                    LastName = pin.LastName,
                    PinnedUserId = pin.PinnedUserId,
                    ViolationCount = totalViolations,
                    LastBranchActivity = lastEventDate.Branch,
                    UserId = pin.UserId,
                    RuleSet = new UserTrackingPinnedUserRuleset()
                    {
                      AlertType = pin.RuleSet.AlertType,
                      Elapse = pin.RuleSet.Elapse,
                      SeverityClassification = pin.RuleSet.SeverityClassification,
                      Value = pin.RuleSet.Value,
                      RuleSetId = pin.RuleSet.RuleSetId
                    }
                  });

                  _userTrackingRepository.UpdateViolationCount(pin.PinnedUserId, pin.RuleSet.RuleSetId, totalViolations);
                }
                break;
              case Atlas.Enumerators.Tracking.Elapse.Hours:
                var totalHours = span.TotalHours;
                if (totalHours > pin.RuleSet.Value)
                {
                  var totalViolations = ((int)Math.Floor(totalHours / pin.RuleSet.Value));
                  userTrackingPinnedUser.Add(new UserTrackingPinnedUser()
                  {
                    Active = pin.Active,
                    CreateDate = pin.CreateDate,
                    FirstName = pin.FirstName,
                    LastName = pin.LastName,
                    PinnedUserId = pin.PinnedUserId,
                    ViolationCount = totalViolations,
                    UserId = pin.UserId,
                    RuleSet = new UserTrackingPinnedUserRuleset()
                    {
                      AlertType = pin.RuleSet.AlertType,
                      Elapse = pin.RuleSet.Elapse,
                      SeverityClassification = pin.RuleSet.SeverityClassification,
                      Value = pin.RuleSet.Value,
                      RuleSetId = pin.RuleSet.RuleSetId
                    }
                  });
                  _userTrackingRepository.UpdateViolationCount(pin.PinnedUserId, pin.RuleSet.RuleSetId, totalViolations);
                }
                break;
              case Atlas.Enumerators.Tracking.Elapse.Days:
                var totalDays = span.TotalDays;
                if (totalDays > pin.RuleSet.Value)
                {
                  var totalViolations = ((int)Math.Floor(totalDays / pin.RuleSet.Value));
                  userTrackingPinnedUser.Add(new UserTrackingPinnedUser()
                  {
                    Active = pin.Active,
                    CreateDate = pin.CreateDate,
                    FirstName = pin.FirstName,
                    LastName = pin.LastName,
                    PinnedUserId = pin.PinnedUserId,
                    ViolationCount = totalViolations,
                    UserId = pin.UserId,
                    RuleSet = new UserTrackingPinnedUserRuleset()
                    {
                      AlertType = pin.RuleSet.AlertType,
                      Elapse = pin.RuleSet.Elapse,
                      SeverityClassification = pin.RuleSet.SeverityClassification,
                      Value = pin.RuleSet.Value,
                      RuleSetId = pin.RuleSet.RuleSetId
                    }
                  });
                  _userTrackingRepository.UpdateViolationCount(pin.PinnedUserId, pin.RuleSet.RuleSetId, totalViolations);
                }
                break;
            }
          }
        }
      }
      return userTrackingPinnedUser;
    }
  }
}
