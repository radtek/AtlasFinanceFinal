using Atlas.Online.Web.Security;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Atlas.Online.Web.Controllers.Api
{
  [Authorize]
  [ValidateHttpAntiForgeryToken]
  public class QuoteAcceptanceController : AppApiController
  {
    private TimeSpan _timeMorning = TimeSpan.FromHours(8) + TimeSpan.FromMinutes(00);
    private TimeSpan _timeEvening = TimeSpan.FromHours(14) + TimeSpan.FromMinutes(50);

    private static List<DateTime> _holidays = null;

    [HttpPost]
    public HttpResponseMessage Accept(long? id)
    {

      _holidays = Services.WebServiceClient.UTL_GetHolidays();

      HttpResponseMessage response;
      if (!ValidateApplication(id, out response))
      {
        return response;
      }

      var application = CurrentClient.CurrentApplication;
      if (application.Status != Enumerators.Account.AccountStatus.PreApproved)
      {
        return RedirectToAction("Index", "MyAccount");
      }

      if (!Services.WebServiceClient.QTE_AcceptQuote(id.Value))
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Quote acceptance failed.");
      }

      if (application.Status == Enumerators.Account.AccountStatus.Declined)
        return RedirectTo("Declined", "Application");

      if (application.BankDetail.Bank.Type == Enumerators.General.BankName.ABS)
        _timeEvening = TimeSpan.FromHours(17) + TimeSpan.FromMinutes(45);

      if (DateTime.Now.TimeOfDay >= _timeMorning && DateTime.Now.TimeOfDay <= _timeEvening && DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday
        && !_holidays.Contains(DateTime.Today) && !application.BankDetail.IsVerified)
      {
        return RedirectToAction("Pending", "Application", new { id = id });
      }
      else if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.DayOfWeek == DayOfWeek.Sunday || _holidays.Contains(DateTime.Today))
      {
        return RedirectToAction("PendingHoliday", "Application", new { id = id });
      }
      else if (DateTime.Now.TimeOfDay >= _timeMorning && DateTime.Now.TimeOfDay >= _timeEvening) // Reached cut off time.
      {
        var nextPayDay = DateTime.Today;

        if ((DateTime.Now.TimeOfDay > _timeEvening && DateTime.Now.TimeOfDay < new TimeSpan(23, 59, 59)))
          nextPayDay = nextPayDay.AddDays(1);

        if (nextPayDay.DayOfWeek != DayOfWeek.Saturday && nextPayDay.DayOfWeek != DayOfWeek.Sunday && !_holidays.Contains(nextPayDay))
        {
          return RedirectToAction("PendingNextWorkingDay", "Application", new { id = id });
        }
        else
        {
          return RedirectToAction("PendingHoliday", "Application", new { id = id });
        }
      }
      else if (DateTime.Now.TimeOfDay <= _timeMorning && DateTime.Now.TimeOfDay <= _timeEvening && DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
      {
        var nextPayDay = DateTime.Today;

        if ((DateTime.Now.TimeOfDay > _timeEvening && DateTime.Now.TimeOfDay < new TimeSpan(23, 59, 59)))
          nextPayDay = nextPayDay.AddDays(1);

        if (nextPayDay.DayOfWeek != DayOfWeek.Saturday && nextPayDay.DayOfWeek != DayOfWeek.Sunday && !_holidays.Contains(nextPayDay))
        {
          return RedirectToAction("PendingNextWorkingDay", "Application", new { id = id });
        }
        else
        {
          return RedirectToAction("PendingHoliday", "Application", new { id = id });
        }

      }
      else if (DateTime.Now.TimeOfDay >= _timeMorning && DateTime.Now.TimeOfDay <= _timeEvening && DateTime.Now.DayOfWeek != DayOfWeek.Saturday && DateTime.Now.DayOfWeek != DayOfWeek.Sunday
        && !_holidays.Contains(DateTime.Today) && application.BankDetail.IsVerified)
      {
        return RedirectToAction("Complete", "Application", new { id = id });
      }
      return RedirectToAction("Pending", "Application", new { id = id });
    }

    [HttpPost]
    public HttpResponseMessage Reject(long? id)
    {
      HttpResponseMessage response;
      if (!ValidateApplication(id, out response))
      {
        return response;
      }

      var application = CurrentClient.CurrentApplication;

      if (!Services.WebServiceClient.QTE_RejectQuote(id.Value))
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Quote rejection failed.");

      return RedirectToAction("Index", "MyAccount", new { id = id });
    }

    private bool ValidateApplication(long? id, out HttpResponseMessage response)
    {
      response = null;
      if (!id.HasValue)
      {
        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id parameter required.");
        return false;
      }

      var application = CurrentClient.CurrentApplication;
      if (application.Status != Enumerators.Account.AccountStatus.PreApproved)
      {
        response = RedirectToAction("Index", "MyAccount");
        return false;
      }

      if (application == null || application.ApplicationId != id.Value)
      {
        response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid id.");
        return false;
      }

      return true;
    }
  }
}