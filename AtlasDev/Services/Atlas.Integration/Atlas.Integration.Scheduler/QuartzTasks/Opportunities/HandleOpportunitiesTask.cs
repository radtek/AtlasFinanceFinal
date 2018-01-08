/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Task to check on Active OPP_CaseDetail against ASS loans
  * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2015-08-31- Initial creation
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using Quartz;
using Npgsql;
using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Common.Interface;
using Atlas.Domain.Model.Opportunity;


namespace Atlas.Integration.Scheduler.QuartzTasks.Opportunities
{
  /// <summary>
  /// Scans for opportunities completed/expired in the ASS data and updates the opportunities accordingly
  /// </summary>
  [DisallowConcurrentExecution]
  internal class HandleOpportunitiesTask : IJob
  {
    public HandleOpportunitiesTask(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      _log.Information("Execute() starting");

      try
      {
        Dictionary<string, long> branchList;
        var casesWithPendingOpp = new List<Tuple<Int64, string, DateTime>>();
        using (var uow = new UnitOfWork())
        {
          branchList = uow.Query<BRN_Branch>().ToDictionary(k => k.LegacyBranchNum.PadLeft(3, '0'), v => v.BranchId);
          var currentDate = DateTime.Now;
          casesWithPendingOpp = uow.Query<OPP_CaseDetail>()
            .Where(s => s.FollowUp < currentDate && 
              (s.OpportunityState == Enumerators.Opportunity.OpportunityStatus.NotSet || s.OpportunityState == Enumerators.Opportunity.OpportunityStatus.New))
            .Select(s => new Tuple<Int64, string, DateTime>(s.CaseDetailId, s.ClientIdNum, s.Started))
            .ToList();
        }

        using (var connAss = new NpgsqlConnection(_config.GetAssConnectionString()))
        {
          if (casesWithPendingOpp.Any())
          {
            #region Find recent loans for IDs
            var loansFoundForIds = new List<Tuple<Int64, string, DateTime, decimal, string, decimal>>();
            connAss.Open();

            using (var cmdFoundClients = connAss.CreateCommand())
            {
              cmdFoundClients.CommandText = "SELECT lrep_brnum, client FROM company.client WHERE identno = @identno ORDER BY userdate DESC NULLS LAST LIMIT 10";
              var idParam = cmdFoundClients.Parameters.Add("identno", NpgsqlTypes.NpgsqlDbType.Char, 20);

              foreach (var idPending in casesWithPendingOpp)
              {
                idParam.Value = idPending.Item2;
                var foundClientAtBranches = new List<Tuple<string, string>>();
                using (var rdr = cmdFoundClients.ExecuteReader())
                {
                  while (rdr.Read())
                  {
                    foundClientAtBranches.Add(new Tuple<string, string>(rdr.GetString(0), rdr.GetString(1)));
                  }
                }

                if (foundClientAtBranches.Any())
                {
                  using (var cmdLoans = connAss.CreateCommand())
                  {
                    var branchClientPairs = string.Join(" OR ", foundClientAtBranches.Select(s => string.Format("(lrep_brnum='{0}' AND client='{1}')", s.Item1, s.Item2)));

                    cmdLoans.CommandType = CommandType.Text;
                    cmdLoans.CommandText = string.Format(
                      "SELECT lrep_brnum, loandate, cheque, loanmeth, payno " +
                      "FROM company.loans " +
                      "WHERE ({0}) AND (cheque >= 100) AND (status = 'N') " +
                      "AND (loandate BETWEEN '{1:yyyy-MM-dd}'::timestamp AND ('{1:yyyy-MM-dd}'::timestamp + INTERVAL '32 days') ) " +
                      "AND nctrantype IN ('USE', 'N/A')  " +
                      "ORDER BY loan DESC NULLS LAST LIMIT 1", branchClientPairs, idPending.Item3);

                    using (var rdr = cmdLoans.ExecuteReader())
                    {
                      if (rdr.Read())
                      {
                        loansFoundForIds.Add(new Tuple<long, string, DateTime, decimal, string, decimal>(
                          idPending.Item1, rdr.GetString(0), rdr.GetDateTime(1), rdr.GetDecimal(2), rdr.GetString(3), rdr.GetDecimal(4)));
                      }
                    }
                  }
                }
              }
            }
            #endregion

            #region Update opportunities            
            if (loansFoundForIds.Any())
            {
              _log.Information("Found successful opportunities: {@Found}", loansFoundForIds);

              using (var uow = new UnitOfWork())
              {
                foreach (var item in loansFoundForIds)
                {
                  var caseDetail = uow.Query<OPP_CaseDetail>().First(s => s.CaseDetailId == item.Item1);
                  caseDetail.GrantedBranch = uow.Query<BRN_Branch>().FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == item.Item2);
                  caseDetail.GrantedDate = item.Item3;
                  caseDetail.GrantedLoanAmount = item.Item4;
                  caseDetail.GrantedPeriodType = ToPeriod(item.Item5);
                  caseDetail.GrantedPeriodVal = (int)item.Item6;
                  caseDetail.OpportunityState = Enumerators.Opportunity.OpportunityStatus.Successful;
                }

                uow.CommitChanges();
              }
            }
            #endregion
          }

          #region Fail old pending cases
          using (var uow = new UnitOfWork())
          {
            var olderThan = DateTime.Now.Subtract(TimeSpan.FromDays(31));
            var failed = uow.Query<OPP_CaseDetail>()
              .Where(s => s.OpportunityState == Enumerators.Opportunity.OpportunityStatus.New && s.FollowUp < olderThan)
              .Select(s => s.CaseDetailId).ToList();

            if (failed.Any())
            {
              foreach (var caseId in failed)
              {
                var caseDetail = uow.Query<OPP_CaseDetail>().First(s => s.CaseDetailId == caseId);
                caseDetail.OpportunityState = Enumerators.Opportunity.OpportunityStatus.Unsuccessful;
              }
              uow.CommitChanges();
            }
            _log.Information("Expired {cases} old opportunities", failed.Count);
          }
          #endregion
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute()");
      }

      _log.Information("Execute() completed");
    }


    private static Enumerators.Account.PeriodFrequency ToPeriod(string p)
    {
      switch (p)
      {
        case "W":
          return Enumerators.Account.PeriodFrequency.Weekly;

        case "B":
          return Enumerators.Account.PeriodFrequency.BiWeekly;

        default:
          return Enumerators.Account.PeriodFrequency.Monthly;
      }
    }


    private readonly ILogging _log;

    private readonly IConfigSettings _config;

  }
}
