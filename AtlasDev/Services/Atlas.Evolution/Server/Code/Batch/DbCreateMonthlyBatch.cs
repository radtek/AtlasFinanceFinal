using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Xpo;

using EvolutionEnums = Atlas.Enumerators.Evolution;
using Atlas.Domain.Model.Evolution;
using Atlas.Domain.Model;
using Atlas.Common.Interface;
using Atlas.Evolution.Server.Code.Data;
using Atlas.Evolution.Server.Code.Utils;


namespace Atlas.Evolution.Server.Code.Batch
{
  internal static class DbCreateMonthlyBatch
  {
    public static long CreateBatch(ILogging log, IConfigSettings config, bool createFinalFile, DateTime collateDate, DateTime monthStart, DateTime monthEnd)
    {
      using (var uow = new UnitOfWork())
      {
        #region Start new batch
        var batch = uow.Query<EVO_UploadBatch>()
          .FirstOrDefault(s => s.BatchType == EvolutionEnums.BatchTypes.Monthly &&
          s.BatchIsLive == createFinalFile &&
          s.BatchPeriodStartDate == monthStart && s.BatchPeriodEndDate == monthEnd);

        if (batch != null)
        {
          log.Warning("Batch {start:yyyy-MM-dd}-{end:yyyy-MM-dd} has already been created! Regenerating...", monthStart, monthEnd);
          foreach (var snapshot in batch.LoanTrackSnapshots.ToList())
          {
            snapshot.Delete();
          }

          batch.Delete();
          uow.CommitChanges();
        }

        batch = new EVO_UploadBatch(uow)
        {
          BatchIsLive = createFinalFile,
          BatchType = EvolutionEnums.BatchTypes.Monthly,
          CollateDate = collateDate,
          BatchPeriodStartDate = monthStart,
          BatchPeriodEndDate = monthEnd,
          Host = Enumerators.General.Host.ASS,
          StagingState = EvolutionEnums.StagingState.New
        };
        #endregion

        var added = 0;
        var branches = uow.Query<BRN_Branch>().OrderBy(s => s.LegacyBranchNum).ToList();
        foreach (var branch in branches)
        {
          var branchSnapshots = 0;
          var snapShotsForBranch = uow.Query<EVO_LoanTrackSnapshot>()
            .Where(s => s.LoanTrack.Branch == branch && (s.SnapshotDate >= monthStart && s.SnapshotDate <= monthEnd))
            .OrderBy(s => s.SnapshotDate)
            .ToList();
          log.Information("Found {Snapshots} daily snapshots for {branch}", snapShotsForBranch.Count, branch.LegacyBranchNum);

          #region Get distinct loans from the daily open/close snapshots: last loan closed |||| first loan registered (not closed)
          var loansRecIdsAdded = new List<long>();
          if (snapShotsForBranch.Any())
          {
            var closed = 0;
            var registered = 0;
            var distinctLoanRecIds = snapShotsForBranch.Select(s => s.LoanTrack.LoanRecId).Distinct().ToList();
            log.Information("Found {Count} distinct daily snapshots for branch {Branch}", distinctLoanRecIds.Count(), branch.LegacyBranchNum);

            foreach (var loanRecId in distinctLoanRecIds)
            {
              var lastClosed = snapShotsForBranch
               .Where(s => s.LoanTrack.LoanRecId == loanRecId && s.Reason == EvolutionEnums.SnapshotReasonTypes.Closure)
               .OrderByDescending(s => s.SnapshotDate).FirstOrDefault();
              if (lastClosed != null)
              {
                loansRecIdsAdded.Add(lastClosed.LoanTrack.LoanRecId);
                batch.LoanTrackSnapshots.Add(lastClosed);
                branchSnapshots++;
                added++;
                closed++;
              }

              var firstRegistered = snapShotsForBranch
                .Where(s => s.LoanTrack.LoanRecId == loanRecId && s.Reason == EvolutionEnums.SnapshotReasonTypes.Registration)
                .OrderBy(s => s.SnapshotDate).FirstOrDefault();

              // If loan closed in the same month as the registration, only send closure...
              if (firstRegistered != null && lastClosed == null)
              {
                loansRecIdsAdded.Add(firstRegistered.LoanTrack.LoanRecId);
                batch.LoanTrackSnapshots.Add(firstRegistered);
                branchSnapshots++;
                added++;
                registered++;
              }
            }
            log.Information("Closed: {Closed}, Registered: {Registered} for branch {Branch}", closed, registered, branch.LegacyBranchNum);
          }
          #endregion

          #region Get currently active loans, excluding the above
          using (var assRepos = new AssRepository(config.GetAssConnectionString()))
          {
            // Don't do closed/opened            
            var excludeRecIds = loansRecIdsAdded.Any() ? $"AND (recid NOT IN ({string.Join(",", loansRecIdsAdded)}))" : string.Empty;

            // status = 'P' THEN 'Part Paid', status = 'N' THEN 'Newly created loan', 'J' THEN 'Journalised'
            // Open and outstanding >= 100, as per SACCRA requirements (<100 outstanding must not be submitted)
            var allOtherActiveLoans = assRepos.GetLoans(
              $"SELECT * FROM company.loans " +
              $"WHERE (COALESCE(outamnt, 0) >= 100) AND " +
              $"(loandate BETWEEN '{monthEnd.Subtract(TimeSpan.FromDays(545)):yyyy-MM-dd}' AND '{monthEnd:yyyy-MM-dd}') AND " + // Max. 1.5 years loan taken out
              $"(brnum = '{branch.LegacyBranchNum}') AND " +
              $"(status IN ('P', 'J', 'N')) AND (nctrantype IN ('N/A', 'USE')) {excludeRecIds}").ToList();

            if (allOtherActiveLoans.Any())
            {
              var status = 0;
              log.Information("Current loans- Loans: {Loans}", allOtherActiveLoans.Count);

              var branchClientWhere = string.Join(" OR ",
                allOtherActiveLoans.Select(s => new { s.brnum, s.client, s.loan })
                  .Select(s => $"(brnum = '{s.brnum}' AND client = '{s.client}')"));

              // Get all 'client' details for these loans
              var clients = assRepos.GetClients($"SELECT * FROM company.client WHERE ({branchClientWhere})").ToList();
              log.Information("Current loans- Clients: {Clients}", clients.Count);

              var branchClientLoanWhere = string.Join(" OR ",
                allOtherActiveLoans.Select(s => new { s.brnum, s.client, s.loan })
                  .Select(s => $"(brnum = '{s.brnum}' AND client = '{s.client}' AND loan = '{s.loan}')"));

              var newTrans = assRepos.GetTrans($"SELECT * FROM company.trans WHERE ({branchClientLoanWhere})").ToList();
              log.Information("Current loans- Trans: {Trans}", newTrans.Count);

              // Add to snapshot
              foreach (var loan in allOtherActiveLoans)
              {
                var loanTrans = newTrans.Where(s => s.brnum == loan.brnum && s.client == loan.client && s.loan == loan.loan).ToList();
                if (!loanTrans.Any())
                {
                  log.Error("Failed to locate any transactions for {Branch} {Client} {Loan}", loan.brnum, loan.client, loan.loan);
                  continue;
                }

                var loanTrack = uow.Query<EVO_LoanTrack>().FirstOrDefault(s => s.LoanRecId == loan.recid);
                if (loanTrack == null)
                {
                  var client = clients.FirstOrDefault(s => s.brnum == loan.brnum && s.client == loan.client);
                  if (client == null)
                  {
                    log.Error("Failed to locate client: {Branch}/{Client}", loan.brnum, loan.client);
                    continue;
                  }

                  status++;
                  loanTrack = new EVO_LoanTrack(uow)
                  {
                    Branch = branch,
                    ClientRecId = client.recid,
                    LoanRecId = loan.recid,

                    AssClient = loan.client,
                    AssLoan = loan.loan,
                    AssLoanReason = loan.popup_lr.Trim(),
                    LoanAmount = loan.cheque,
                    AssLoanStartDate = loan.loandate,
                  };
                }

                var lastReceipt = (loan.status.Trim() != "H") ? EvoCalcs.LastReceipt(loanTrans, monthEnd) :
                                                                EvoCalcs.FirstMissedPayment(loanTrans, monthEnd);

                var loanSnapshot = new EVO_LoanTrackSnapshot(uow)
                {
                  LoanTrack = loanTrack,
                  Reason = EvolutionEnums.SnapshotReasonTypes.Active,
                  Created = DateTime.Now,
                  SnapshotDate = monthEnd,

                  CurrentBalance = EvoCalcs.CurrentBalance(loan.cheque, loanTrans, monthEnd),
                  LastReceipt = lastReceipt,
                  OverdueAmount = EvoCalcs.OverdueAmount(loanTrans, monthEnd),
                  OverdueSince = EvoCalcs.OverdueSince(loanTrans, monthEnd),

                  LoanStatus = loan.status.Trim(),
                  PayFrequency = loan.loanmeth.Trim(),
                  InstalmentAmount = loan.tramount,
                  LoanPeriod = (int)loan.payno_orig,
                  GrossSalary = (loan.basic ?? 0),
                };

                batch.LoanTrackSnapshots.Add(loanSnapshot);

                status++;
                added++;
                branchSnapshots++;
                if (added > 500)
                {
                  log.Information("Posting {Snapshots} to monthly batch...", added);
                  uow.CommitChanges();
                  added = 0;
                }
              }
              log.Information("Adding 'loan status': {status} for branch {Branch}", status, branch.LegacyBranchNum);
            }
          }
          #endregion

          log.Information("Created {Batches} snapshots for {branch}", branchSnapshots, branch.LegacyBranchNum);
        }

        batch.StagingState = EvolutionEnums.StagingState.New; // still to be merged with sdc

        log.Information("Posting {Snapshots} to monthly batch...", added);
        uow.CommitChanges();

        log.Information("Total batch size: {BatchSize}", batch.LoanTrackSnapshots.Count);

        return batch.UploadBatchId;
      }
    }

  }
}
