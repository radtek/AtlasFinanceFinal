using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;


namespace Atlas.LoanEngine.Account
{
  public class Arrearage
  {
    private long _accountId { get; set; }

    // Do a calculation on all Accounts
    public Arrearage()
    {
      _accountId = 0;
    }

    // Do a calculation on a specific account
    public Arrearage(long accountId)
    {
      _accountId = accountId;
    }

    public void CalculateArrears_old()
    {
      using (var uow = new UnitOfWork())
      {
        var ledgerTransactions = new XPQuery<LGR_Transaction>(uow).Where(t => t.CalculatedArrearsDate == null);

        if (_accountId > 0)
          ledgerTransactions = ledgerTransactions.Where(l => l.Account.AccountId == _accountId);

        var accountsAffected = ledgerTransactions.Select(t => t.Account).Distinct().ToList();

        var accountsAffordabilityOption = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => accountsAffected.Contains(a.Account) && a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Accepted).ToList();

        var accountsArrearage = new XPQuery<ACC_Arrearage>(uow).Where(a => accountsAffected.Contains(a.Account)).OrderBy(a => a.Account).ToList();

        var noOfAccountPerTask = 20;
        var totalTasks = accountsAffected.Count / noOfAccountPerTask + (accountsAffected.Count % noOfAccountPerTask > 0 ? 1 : 0);

        for (int i = 0; i < totalTasks; i++)
        {
          var tempAccountsAffected = accountsAffected.Skip(i * noOfAccountPerTask).Take(noOfAccountPerTask).ToList();
          var tasks = new Task[tempAccountsAffected.Count()];

          for (int j = 0; j < tempAccountsAffected.Count();j++ )
          {
            var tempAccount = tempAccountsAffected[j];
            var tempAccountDTO = AutoMapper.Mapper.Map<ACC_Account, ACC_AccountDTO>(tempAccount);
            tasks[j] = Task.Factory.StartNew(() =>
              {
                var accountEng = new Default(tempAccount.AccountId);
                var accountCycles = accountEng.GetCycles(tempAccountDTO);
                var affordabilityOption = accountsAffordabilityOption.FirstOrDefault(a => a.Account == tempAccount);

                if (affordabilityOption == null)
                  throw new Exception(string.Format("Account {0} does not have any affordability attached", tempAccount.AccountId));

                var accountArrearage = accountsArrearage.Where(a => a.Account == tempAccount).ToList();

                var affectedTransactions = ledgerTransactions.Where(t => t.Account == tempAccount).ToList();

                affectedTransactions.ForEach((t) => { t.CalculatedArrearsDate = DateTime.Now; });

                for (var cycleNo = 1; cycleNo <= accountCycles.Count; cycleNo++)
                {
                  var cycle = accountCycles[cycleNo - 1];

                  var nextDebitOrderTransaction = new XPQuery<DBT_Transaction>(uow).Where(a => (a.OverrideActionDate ?? a.ActionDate) > cycle.Item1 && a.DebitType.Type == Enumerators.Debit.DebitType.Regular).OrderBy(a => (a.OverrideActionDate ?? a.ActionDate)).FirstOrDefault();
                  if (nextDebitOrderTransaction != null || tempAccount.FirstInstalmentDate >= DateTime.Today)
                  {
                    var cycleTransactions = affectedTransactions.Where(t => cycle.Item1 <= t.TransactionDate && t.TransactionDate <= cycle.Item2).ToList();

                    var currentArrearageCycle = accountArrearage.FirstOrDefault(a => a.PeriodStart == cycle.Item1);

                    // Arrearage for this cycle does not exist. Add 1!
                    if (currentArrearageCycle == null)
                    {
                      currentArrearageCycle = new ACC_Arrearage(uow)
                      {
                        Account = tempAccount,
                        AmountPaid = 0,
                        ArrearageCycle = cycleNo,
                        ArrearsAmount = 0,
                        TotalArrearsAmount = 0,
                        DelinquencyRank = 0,
                        CreateDate = DateTime.Now,
                        InstalmentDue = tempAccount.InstalmentAmount,
                        PeriodStart = cycle.Item1,
                        PeriodEnd = cycle.Item2,
                        TotalPaid = accountArrearage.Where(a => a.PeriodStart < cycle.Item1).Sum(a => a.AmountPaid)
                      };

                      if (cycleNo == 1)
                      {
                        // first Cycle - Requires affordability option set
                        currentArrearageCycle.InstalmentDue = affordabilityOption.Instalment ?? 0;
                      }
                    }

                    if (cycleTransactions.Count > 0)
                    {
                      // Update Cycle amounts
                      UpdateCycle(ref currentArrearageCycle,
                        cycleTransactions.Where(t => t.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Payment || t.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Payment).Sum(t => t.Amount),
                        cycleTransactions.Where(t => t.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Reversal_Unsuccessful).Sum(t => t.Amount));

                      // is current cycle in arrears?
                      var paidRatio = Math.Round((currentArrearageCycle.AmountPaid / currentArrearageCycle.InstalmentDue) * 100);

                      // Calculate periods in arrears
                      var prevArrearage = accountArrearage.Where(a => a.PeriodStart < cycle.Item1).OrderBy(a => a.PeriodStart).LastOrDefault();
                      if (prevArrearage != null)
                      {
                        // calculate paidRatio
                        // is current cycle in arrears?
                        var prevPaidRatio = Math.Round((prevArrearage.AmountPaid / prevArrearage.InstalmentDue) * 100);

                        prevArrearage.ArrearsAmount = prevArrearage.InstalmentDue - prevArrearage.AmountPaid;
                        prevArrearage.TotalArrearsAmount = prevArrearage.InstalmentDue - prevArrearage.AmountPaid;

                        if (prevPaidRatio < 95)
                        {
                          // Cycle in Arrears
                          prevArrearage.DelinquencyRank = 1;
                        }
                        // check if prev arrear age cycle has a delinquency
                        if (prevArrearage.DelinquencyRank > 0)
                        {
                          if (currentArrearageCycle.DelinquencyRank > 0)
                          {
                            // Prev cycle was in arrears --  inc delinquency in current cycle
                            currentArrearageCycle.DelinquencyRank += prevArrearage.DelinquencyRank;
                          }
                          else
                          {
                            currentArrearageCycle.DelinquencyRank = prevArrearage.DelinquencyRank;

                            // check if the client paid above instalment amount, if so, delinquency might decrease
                            if (paidRatio > 100)
                            {
                              // Delinquency could decrease by more than 1
                              var arrearagesOfThisDelinquencyCycle = new List<ACC_Arrearage>();
                              foreach (var a in accountArrearage.Where(a => a.PeriodStart < cycle.Item1).OrderByDescending(a => a.PeriodStart).ToList())
                              {
                                if (a.DelinquencyRank == 0)
                                {
                                  break;
                                }
                                arrearagesOfThisDelinquencyCycle.Add(a);
                              }
                              arrearagesOfThisDelinquencyCycle = arrearagesOfThisDelinquencyCycle.OrderBy(a => a.PeriodStart).ToList();
                              var extraAmountPaid = currentArrearageCycle.AmountPaid - currentArrearageCycle.InstalmentDue;
                              for (int k = 0; k < arrearagesOfThisDelinquencyCycle.Count; k++)
                              {
                                var arrearage = arrearagesOfThisDelinquencyCycle[k];

                                if (arrearage.ArrearsAmount < extraAmountPaid)
                                {
                                  extraAmountPaid -= arrearage.ArrearsAmount;
                                  currentArrearageCycle.DelinquencyRank--;
                                }
                              }
                            }
                          }
                        }

                        // Update Total Arrears
                        if (tempAccount.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
                        {
                          currentArrearageCycle.TotalArrearsAmount = prevArrearage.TotalArrearsAmount + currentArrearageCycle.ArrearsAmount;
                        }
                        else
                        {
                          currentArrearageCycle.TotalArrearsAmount = prevArrearage.TotalArrearsAmount +
                            (currentArrearageCycle.ArrearsAmount >= 0
                              ? currentArrearageCycle.ArrearsAmount
                              : currentArrearageCycle.InstalmentDue - currentArrearageCycle.AmountPaid);
                        }
                      }
                      else
                      {
                        // Update Total Arrears
                        if (tempAccount.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
                          currentArrearageCycle.TotalArrearsAmount = currentArrearageCycle.ArrearsAmount;
                        else
                          currentArrearageCycle.TotalArrearsAmount = currentArrearageCycle.ArrearsAmount >= 0
                              ? currentArrearageCycle.ArrearsAmount
                              : currentArrearageCycle.InstalmentDue - currentArrearageCycle.AmountPaid;
                      }

                      // Update remainder cycles
                      var remainderArrearages = accountArrearage.Where(a => a.PeriodStart > currentArrearageCycle.PeriodStart).OrderBy(a => a.PeriodStart).ToList();

                      for (int k = 0; k < remainderArrearages.Count; k++)
                      {
                        var arrearageCycle = remainderArrearages[k];

                        UpdateCycle(ref arrearageCycle,
                           cycleTransactions.Where(t => t.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Payment || t.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Payment).Sum(t => t.Amount),
                           cycleTransactions.Where(t => t.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Reversal_Unsuccessful).Sum(t => t.Amount));
                      }
                    }
                  }
                }

                accountEng = null;
              });
          }

          Task.WaitAll(tasks);

          uow.CommitChanges();
        }
      }
    }

    public void CalculateArrears()
    {
      var accounts = new List<ACC_AccountDTO>();

      using (var uow = new UnitOfWork())
      {
        var newLedgerTransactions = new XPQuery<LGR_Transaction>(uow).Where(t => t.CalculatedArrearsDate == null);
        if (_accountId > 0)
          newLedgerTransactions = newLedgerTransactions.Where(l => l.Account.AccountId == _accountId);

        var affectedAccounts = newLedgerTransactions.Select(l => l.Account).Distinct().ToList();
        accounts = AutoMapper.Mapper.Map<List<ACC_Account>, List<ACC_AccountDTO>>(affectedAccounts);
      }

      var noOfAccountPerTask = 20;
      var totalTasks = accounts.Count / noOfAccountPerTask + (accounts.Count % noOfAccountPerTask > 0 ? 1 : 0);

      for (int i = 0; i < totalTasks; i++)
      {
        var tempAccounts = accounts.Skip(i * noOfAccountPerTask).Take(noOfAccountPerTask).ToList();
        var tasks = new Task[tempAccounts.Count()];

        for (int j = 0; j < tempAccounts.Count(); j++)
        {
          var tempAccount = tempAccounts[j];
          tasks[j] = Task.Factory.StartNew(() =>
            {
              using (var uow = new UnitOfWork())
              {
                var account = new XPQuery<ACC_Account>(uow).First(a => a.AccountId == tempAccount.AccountId);
                var arrearage = new XPQuery<ACC_Arrearage>(uow).Where(a => a.Account == account).ToList();
                var transactions = new XPQuery<LGR_Transaction>(uow).Where(l => l.Account == account).ToList();

                var acceptedAffordability = new XPQuery<ACC_AffordabilityOption>(uow).First(a => a.Account == account && a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Accepted);

                var accountEng = new Default(account.AccountId);
                var accountCycles = accountEng.GetCycles(AutoMapper.Mapper.Map<ACC_Account, ACC_AccountDTO>(account));

                for (var cycleNo = 1; cycleNo <= accountCycles.Count; cycleNo++)
                {
                  var cycle = accountCycles[cycleNo - 1];

                  var cycleArrearage = arrearage.FirstOrDefault(a => a.PeriodStart == cycle.Item1);

                  // If arrearage does not exist, add 1
                  if (cycleArrearage == null)
                  {
                    if (cycle.Item1.AddDays(account.AccountType.ArrearageBufferPeriod).Date <= DateTime.Today.Date
                      || cycleNo == 1)
                    {
                      cycleArrearage = new ACC_Arrearage(uow)
                      {
                        Account = account,
                        AmountPaid = 0,
                        ArrearageCycle = cycleNo,
                        ArrearsAmount = 0,
                        DelinquencyRank = 0,
                        CreateDate = DateTime.Now,
                        InstalmentDue = tempAccount.InstalmentAmount,
                        PeriodStart = cycle.Item1,
                        PeriodEnd = cycle.Item2,
                        TotalArrearsAmount = 0
                      };

                      if (cycleNo == 1)
                      {
                        // first Cycle - Requires affordability option set
                        cycleArrearage.InstalmentDue = acceptedAffordability.Instalment ?? 0;
                      }
                    }
                    else
                    {
                      continue;
                    }
                  }
                  else
                  {
                    cycleArrearage.DelinquencyRank = 0;
                    cycleArrearage.TotalArrearsAmount = 0;
                  }
                  var cycleTransactions = transactions.Where(t => t.TransactionDate >= cycle.Item1 && t.TransactionDate <= cycle.Item2).ToList();
                  cycleTransactions.ForEach(t =>
                    {
                      t.CalculatedArrearsDate = DateTime.Now;
                    });

                  // Caculate Amount paid this cycle
                  cycleArrearage.AmountPaid = cycleTransactions.Where(a => a.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Payment).Sum(a => a.Amount);
                  cycleArrearage.AmountPaid -= cycleTransactions.Where(a => a.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Reversal_Unsuccessful).Sum(a => a.Amount);

                  // Calculate Total Paid
                  cycleArrearage.TotalPaid = arrearage.Where(a => a.PeriodStart < cycle.Item1).Sum(a => a.AmountPaid);
                  cycleArrearage.TotalPaid += cycleArrearage.AmountPaid;

                  // Calculate Delinquency
                  var prevCycleArrearage = arrearage.Where(a => a.PeriodStart < cycleArrearage.PeriodStart).OrderByDescending(a => a.PeriodStart).FirstOrDefault();
                  if (prevCycleArrearage != null)
                  {
                    cycleArrearage.TotalArrearsAmount = prevCycleArrearage.TotalArrearsAmount;
                    // Calculate Arrears
                    if (cycleArrearage.PeriodEnd.AddDays(account.AccountType.ArrearageBufferPeriod).Date <= DateTime.Today.Date)
                    {
                      cycleArrearage.ArrearsAmount = cycleArrearage.InstalmentDue - cycleArrearage.AmountPaid;
                      cycleArrearage.TotalArrearsAmount = cycleArrearage.ArrearsAmount;
                      cycleArrearage.DelinquencyRank += prevCycleArrearage.DelinquencyRank;
                    }
                    else
                    {
                      cycleArrearage.DelinquencyRank = prevCycleArrearage.DelinquencyRank;
                      if (cycleArrearage.TotalArrearsAmount > 0)
                      {
                        if (account.Host.Type == Enumerators.General.Host.Atlas_Online
                          && account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
                        {
                          cycleArrearage.TotalArrearsAmount -= cycleArrearage.AmountPaid;
                          if (cycleArrearage.TotalArrearsAmount <= 0)
                            cycleArrearage.DelinquencyRank = 0;
                        }
                        else
                        {
                          cycleArrearage.TotalArrearsAmount -= (cycleArrearage.InstalmentDue - cycleArrearage.AmountPaid);
                        }
                      }
                    }

                    // Delinquency could decrease by more than 1
                    var extraPaidThisCycle = cycleArrearage.ArrearsAmount * -1;
                    if (extraPaidThisCycle > 0)
                    {
                      foreach (var a in arrearage.Where(a => a.PeriodStart < cycle.Item1).OrderByDescending(a => a.PeriodStart).ToList())
                      {
                        if (a.DelinquencyRank == 0 && extraPaidThisCycle <= 0)
                        {
                          break;
                        }
                        else if (a.ArrearsAmount <= extraPaidThisCycle)
                        {
                          extraPaidThisCycle -= a.ArrearsAmount;
                          cycleArrearage.DelinquencyRank--;
                        }
                      }
                    }
                  }
                  else
                  {
                    // Calculate Arrears
                    if (cycleArrearage.PeriodEnd.AddDays(account.AccountType.ArrearageBufferPeriod).Date < DateTime.Today)
                    {
                      cycleArrearage.ArrearsAmount = cycleArrearage.InstalmentDue - cycleArrearage.AmountPaid;
                      cycleArrearage.TotalArrearsAmount = cycleArrearage.ArrearsAmount;
                      if (cycleArrearage.ArrearsAmount > 0)
                        cycleArrearage.DelinquencyRank = 1;
                    }
                  }
                  if (cycleArrearage.ArrearsAmount < 0)
                    cycleArrearage.ArrearsAmount = 0;
                  if (cycleArrearage.TotalArrearsAmount < 0)
                    cycleArrearage.TotalArrearsAmount = 0;

                  if (accountCycles.Count == cycleNo)
                  {
                    // Last Arrears cycle
                    if (account.Status.Type == Enumerators.Account.AccountStatus.Closed)
                      cycleArrearage.TotalArrearsAmount = 0;
                  }
                }

                uow.CommitChanges();
              }
            });
        }
        Task.WaitAll(tasks);
      }
    }

    private void UpdateCycle(ref ACC_Arrearage arrearage, decimal totalCreditAmount, decimal totalDebitAmount)
    {
      arrearage.AmountPaid += totalCreditAmount;
      arrearage.AmountPaid -= totalDebitAmount;
      arrearage.TotalPaid += totalCreditAmount;
      arrearage.TotalPaid -= totalDebitAmount;
    }
  }
}