using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;


namespace Atlas.LoanEngine.Account
{
  public class CycleInterestCalculation
  {
    public void Start()
    {
      List<ACC_AccountDTO> accountList = new List<ACC_AccountDTO>();
      PER_PersonDTO system = new PER_PersonDTO();

      using (var uow = new UnitOfWork())
      {
        var accounts = (from a in uow.Query<ACC_Account>()
                        //join dc in uow.Query<DBT_Transaction>() on a equals dc.Control.Account
                        where a.Status.Type == Enumerators.Account.AccountStatus.Open
                        //&& (dc.OverrideActionDate ?? dc.ActionDate).Date == DateTime.Today.Date
                        //&& dc.DebitType.DebitTypeId == (int)Enumerators.Debit.DebitType.Regular
                        //&& (dc.Status.StatusId == (int)Enumerators.Debit.Status.Successful
                        //|| dc.Status.StatusId == (int)Enumerators.Debit.Status.Failed)
                        select a).ToList();

        accountList = AutoMapper.Mapper.Map<List<ACC_Account>, List<ACC_AccountDTO>>(accounts);

        system = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System));
      }

      var noOfAccountPerTask = 20;
      var totalTasks = accountList.Count / noOfAccountPerTask + (accountList.Count % noOfAccountPerTask > 0 ? 1 : 0);

      for (int i = 0; i < totalTasks; i++)
      {
        var tempAccounts = accountList.Skip(i * noOfAccountPerTask).Take(noOfAccountPerTask).ToList();
        var tasks = new Task[tempAccounts.Count()];

        for (int j = 0; j < tempAccounts.Count; j++)
        {
          var account = tempAccounts[j];
          tasks[j] = Task.Run(() =>
          {
            // Add Interest
            new GeneralLedger(account.AccountId).AddInterestUntil(DateTime.Today.Date, system);

            // Calculate new account balance
            new Account.Default(account.AccountId).UpdateAccountBalance();
          });
        }
        Task.WaitAll(tasks);
      }

    }
  }
}