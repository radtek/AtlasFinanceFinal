using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model.Evolution;
using Atlas.Domain.Model;
using Atlas.Evolution.Server.Code.Data;
using Atlas.Evolution.Server.Code.Utils;
using EvolutionEnums = Atlas.Enumerators.Evolution;
using Atlas.Common.Interface;


namespace Atlas.Evolution.Server.Code.Batch
{
  internal static class DbCreateDailyBatch
  {
    /// <summary>
    /// Create necessary 1 x EVO_UploadBatch/ Per loan x EVO_LoanTrack/ Per event x EVO_LoanTrackSnapshot
    /// </summary>
    /// <param name="log">Logging</param>
    /// <param name="snapshotDate">The effective date of the snapshot</param>
    /// <param name="createFinalFiles">Must we create final files (true), else test files (false)</param>
    /// <param name="srn">The supplier reference number</param>
    /// <returns></returns>
    public static long CreateBatch(ILogging log, IConfigSettings config, DateTime snapshotDate, bool createFinalFiles, string srn)
    {
      using (var assRepos = new AssRepository(config.GetAssConnectionString()))
      using (var uow = new UnitOfWork())
      {
        var branches = uow.Query<BRN_Branch>().ToList();

        #region Start new batch
        var batchToday = uow.Query<EVO_UploadBatch>()
          .FirstOrDefault(s => s.BatchType == EvolutionEnums.BatchTypes.Daily &&
          s.BatchIsLive == createFinalFiles &&
          s.BatchPeriodStartDate == snapshotDate);
        if (batchToday != null)
        {
          log.Warning("Batch {snapshotDate:yyyy-MM-dd} has already been created! Regenerating...", snapshotDate);

          foreach(var snapshot in batchToday.LoanTrackSnapshots.ToList())
          {
            snapshot.Delete();
          }
          batchToday.Delete();
          uow.CommitChanges();
        }

        var batch = new EVO_UploadBatch(uow)
        {
          BatchIsLive = createFinalFiles,
          BatchType = EvolutionEnums.BatchTypes.Daily,
          CollateDate = DateTime.Now,
          BatchPeriodStartDate = snapshotDate,
          BatchPeriodEndDate = snapshotDate,
          Host = Enumerators.General.Host.ASS,
          StagingState = EvolutionEnums.StagingState.New
        };
        #endregion
            
        // New loans issued today
        var newLoans = assRepos.GetLoans(
          "SELECT * FROM company.loans " +
          $"WHERE nctrantype IN ('USE', 'N/A') AND (COALESCE(outamnt, 0) >= 100) AND (loandate = '{snapshotDate:yyyy-MM-dd}')").ToList();
        log.Information("New loans- Loans: {Opened}", newLoans.Count);

        #region Possible loans re-opened- get loans where something happened on the account today- compare outamnt to previous day and if balance was previously <100, re-open
        var prevDayBatch = uow.Query<EVO_UploadBatch>()
            .FirstOrDefault(s => s.BatchType == EvolutionEnums.BatchTypes.Daily &&
              s.BatchPeriodStartDate == snapshotDate.Subtract(TimeSpan.FromDays(1)));
        if (prevDayBatch != null)
        {
          var possibleReopened = assRepos.GetLoans(
            $"SELECT * FROM company.loans WHERE COALESCE(outamnt, 0) >= 100 AND paidate = '{snapshotDate:yyyy-MM-dd}' AND nctrantype IN ('USE', 'N/A')").ToList();

          log.Information("Suspect possible loans re-opened: {Count}", possibleReopened.Count);
          if (possibleReopened.Any())
          {
            log.Information("Checking for re-opened loans: {Count}", possibleReopened.Count);
            foreach (var loan in possibleReopened)
            {
              if (prevDayBatch != null)
              {
                var found = prevDayBatch.LoanTrackSnapshots.FirstOrDefault(s =>
                  s.LoanTrack.Branch.BranchId == branches.First(b => b.LegacyBranchNum.PadLeft(3, '0') == loan.brnum.PadLeft(3, '0')).BranchId &&
                  s.LoanTrack.AssClient == loan.client && s.LoanTrack.AssLoan == loan.loan);

                if (found != null)
                {
                  // Yesterday's loan balance was < R100, this loan was re-opened
                  if (found.CurrentBalance < 100)
                  {
                    newLoans.Add(loan);
                  }
                }
              }
            }
            log.Information("Re-opened loans confirmed: {Count}", newLoans.Count);
          }
        }
        #endregion

        if (newLoans.Any())
        {
          #region Add new loan registrations
          var branchClientWhere = string.Join(" OR ",
              newLoans.Select(s => new { s.brnum, s.client, s.loan })
                .Select(s => $"(brnum = '{s.brnum}' AND client = '{s.client}')"));

          // Get all 'client' details for these loans
          var clients = assRepos.GetClients($"SELECT * FROM company.client WHERE ({branchClientWhere})").ToList();
          log.Information("New loans- client': {Clients}", clients.Count);

          var branchClientLoanWhere = string.Join(" OR ",
            newLoans.Select(s => new { s.brnum, s.client, s.loan })
              .Select(s => $"(brnum = '{s.brnum}' AND client = '{s.client}' AND loan = '{s.loan}')"));

          var newTrans = assRepos.GetTrans($"SELECT * FROM company.trans WHERE ({branchClientLoanWhere})").ToList();
          log.Information("New loans- 'trans': {Count}", newTrans.Count);

          // Add to snapshot
          foreach (var loan in newLoans)
          {
            var loanTrans = newTrans.Where(s => s.client == loan.client && s.brnum == loan.brnum && s.loan == loan.loan).ToList();
            if (!loanTrans.Any())
            {
              log.Error("New loans: Failed to locate any transactions for {Branch} {Client} {Loan}", loan.brnum, loan.client, loan.loan);
              continue;
            }

            var loanTrack = uow.Query<EVO_LoanTrack>().FirstOrDefault(s => s.LoanRecId == loan.recid);
            if (loanTrack == null)
            {
              var branch = branches.First(s => s.LegacyBranchNum.PadLeft(3, '0') == loan.brnum.PadLeft(3, '0'));
              var client = clients.First(s => s.brnum == loan.brnum && s.client == loan.client);
              loanTrack = new EVO_LoanTrack(uow)
              {
                Branch = branch,
                ClientRecId = client.recid,
                LoanRecId = loan.recid,

                AssClient = loan.client,
                AssLoan = loan.loan,
                AssLoanReason = loan.popup_lr.Trim(),
                LoanAmount = loan.cheque,
                AssLoanStartDate = loan.loandate
              };
            }

            var loanSnapshot = new EVO_LoanTrackSnapshot(uow)
            {
              LoanTrack = loanTrack,
              Reason = EvolutionEnums.SnapshotReasonTypes.Registration,
              Created = DateTime.Now,
              SnapshotDate = snapshotDate,

              CurrentBalance = EvoCalcs.CurrentBalance(loan.cheque, loanTrans, snapshotDate),//loan.outamnt ?? 0,
              LoanStatus = loan.status.Trim(),

              PayFrequency = loan.loanmeth.Trim(),
              InstalmentAmount = loan.tramount,
              LoanPeriod = (int)loan.payno_orig,
              GrossSalary = (loan.basic ?? 0)
            };

            batch.LoanTrackSnapshots.Add(loanSnapshot);
          }
          #endregion

          log.Information("Created {Batches} snapshots", batch.LoanTrackSnapshots.Count());
        }

        #region Daily loan closures (normal or handed over- outamnt = NULL/zero and  paidate/hovrdate = date
        var closedLoans = assRepos.GetLoans(
          $"SELECT * FROM company.loans " +
          $"WHERE ((paidate = '{snapshotDate:yyyy-MM-dd}') OR (hovrdate = '{snapshotDate:yyyy-MM-dd}')) " +
          $"AND (COALESCE(outamnt, 0) < 100) AND (nctrantype IN ('USE', 'N/A'))").ToList(); // 0 or 100? Evolution treats anything <100 as closed
        log.Information("Loan closures: {Count}", closedLoans.Count);

        if (closedLoans.Any())
        {          
          // Check for any custom re-payment plans for these loans         
          var branchClientLoanWhere = string.Join(" OR ",
            closedLoans.Select(s => new { s.brnum, s.client, s.loan })
              .Select(s => $"(brnum = '{s.brnum}' AND client = '{s.client}' AND loan = '{s.loan}')"));
          var payplanh = assRepos.GetPayPlanH($"SELECT * FROM company.payplanh WHERE {branchClientLoanWhere}").ToList();
          log.Information("Loan closures- 'payplanh': {Count}", payplanh.Count);

          // Get transactions for closed loans, to perform necessary calculations
          var trans = assRepos.GetTrans($"SELECT * " + // client, loan, brnum, trtype, trdate, trstat, tramount, instcheq, instinitfe, instinitva, interest, " +
                                                       //"servicefee, servicevat, inspremval, inspremvat, inspolival, inspolivat " +
            $"FROM company.trans WHERE {branchClientLoanWhere}").ToList();
          log.Information("Loan closures- 'trans': {Count}", trans.Count);

          // Get client details for submission
          var branchClientWhere = string.Join(" OR ",
            closedLoans.Select(s => new { s.brnum, s.client, s.loan })
              .Select(s => $"(brnum = '{s.brnum}' AND client = '{s.client}')"));
          var clients = assRepos.GetClients($"SELECT * FROM company.client WHERE ({branchClientWhere})").ToList();
          log.Information("Loan closures- 'client': {Count}", clients.Count);

          foreach (var loan in closedLoans)
          {
            var loanTrans = trans.Where(s => s.brnum == loan.brnum && s.client == loan.client && s.loan == loan.loan).ToList();
            if (!loanTrans.Any())
            {
              log.Error("Loan closures: Failed to locate any transactions for {Branch} {Client} {Loan}", loan.brnum, loan.client, loan.loan);
              continue;
            }

            var loanTrack = uow.Query<EVO_LoanTrack>().Where(s => s.LoanRecId == loan.recid).FirstOrDefault();
            if (loanTrack == null)
            {
              // We've recently implemented the Evolution system, or error
              log.Warning("CLOSED: Unable to locate loan tracking record for {recid}- adding new loan tracking...", loan.recid);

              loanTrack = new EVO_LoanTrack(uow)
              {
                Branch = branches.First(s => s.LegacyBranchNum.PadLeft(3, '0') == loan.brnum.PadLeft(3, '0')),
                ClientRecId = clients.First(s => s.brnum == loan.brnum && s.client == loan.client).recid,
                LoanRecId = loan.recid,

                AssClient = loan.client,
                AssLoan = loan.loan,
                AssLoanReason = loan.popup_lr.Trim(),
                LoanAmount = loan.cheque,
                AssLoanStartDate = loan.loandate
              };
            }

            // Get most recent payplan
            var payplan = payplanh.OrderByDescending(s => s.sr_recno).FirstOrDefault(s => s.brnum == loan.brnum && s.client == loan.client && s.loan == loan.loan);

            var lastReceipt = (loan.status.Trim() != "H") ?
              EvoCalcs.LastReceipt(loanTrans, snapshotDate) : EvoCalcs.FirstMissedPayment(loanTrans, snapshotDate);

            var loanSnapshot = new EVO_LoanTrackSnapshot(uow)
            {
              LoanTrack = loanTrack,
              Reason = EvolutionEnums.SnapshotReasonTypes.Closure,
              Created = DateTime.Now,
              SnapshotDate = snapshotDate,

              LoanStatus = loan.status.Trim(),
              
              PayFrequency = (payplan != null) ? payplan.loanmeth.Trim() : loan.loanmeth.Trim(),
              InstalmentAmount = EvoMapping.InstallmentPerMonth((payplan != null) ? payplan.loanmeth.Trim() : loan.loanmeth.Trim(),
                                                                (payplan != null) ? payplan.instalamt : loan.tramount),
              LoanPeriod = (payplan != null) ? (int)payplan.payno : (int)loan.payno.Value,

              CurrentBalance = EvoCalcs.CurrentBalance(loan.cheque, loanTrans, snapshotDate),
              OverdueAmount = EvoCalcs.OverdueAmount(loanTrans, snapshotDate),
              OverdueSince = EvoCalcs.OverdueSince(loanTrans, snapshotDate),
              LastReceipt = lastReceipt,
              GrossSalary = EvoCalcs.GetMonthSalary(loan.loanmeth.Trim(), (loan.basic ?? 0)) // loan.monthly2= nett salary, loan.basic = gross salary
            };

            batch.LoanTrackSnapshots.Add(loanSnapshot);
          }
        }
        #endregion

        log.Information("Posting {Snapshots} to daily batch...", batch.LoanTrackSnapshots.Count);
        batch.StagingState = EvolutionEnums.StagingState.Ready;
        uow.CommitChanges();

        return batch.UploadBatchId;
      }
    }

  }

}
