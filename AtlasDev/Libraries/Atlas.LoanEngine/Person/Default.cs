using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;


namespace Atlas.LoanEngine.Person
{
  public class Default
  {
    private long _personId { get; set; }


    public Default(long personId)
    {
      _personId = personId;
    }


    public void ActivateBankDetailFromAVS(long avsTransactionId)
    {
      using (var uow = new UnitOfWork())
      {
        var person = new XPQuery<PER_Person>(uow).Where(p => p.PersonId == _personId).FirstOrDefault();
        if (person == null)
          throw new Exception("Person does not exist in DB");

        var avsTransaction = new XPQuery<AVS_Transaction>(uow).Where(t => t.TransactionId == avsTransactionId).FirstOrDefault();
        if (avsTransaction == null)
          throw new Exception("AVS Transaction does not exist");

        // active new bank details
        var newActiveBankDetail = person.GetBankDetails.Select(b=>b.BankDetail).FirstOrDefault(b => b.AccountNum == avsTransaction.AccountNo
          && b.Bank.BankId == avsTransaction.Bank.BankId
          && ((avsTransaction.BankAccountPeriod == null || b.Period == null) ? 1 == 1 : b.Period.PeriodId == avsTransaction.BankAccountPeriod.BankAccountPeriodId)
          && b.Code == avsTransaction.BranchCode);

        if (newActiveBankDetail != null)
        {
          newActiveBankDetail.IsActive = true;

          // deactivate the current active bank details
          var activeBankDetails =
            person.GetBankDetails.Select(b => b.BankDetail)
              .Where(b => b.IsActive && b.DetailId != newActiveBankDetail.DetailId)
              .ToList();
          var debitControlAccounts = new XPQuery<ACC_DebitControl>(uow).Where(a => a.Account.Person == person && a.Account.Status.StatusId < (int)Atlas.Enumerators.Account.AccountStatus.Closed).ToList();

          activeBankDetails.ForEach(activeBankDetail =>
          {
            activeBankDetail.IsActive = false;

            var debitControls = debitControlAccounts.Where(d => d.Control.Bank == activeBankDetail.Bank
              && d.Control.BankAccountName == activeBankDetail.AccountName
              && d.Control.BankAccountNo == activeBankDetail.AccountNum
              && d.Control.BankAccountType == activeBankDetail.AccountType
              && d.Control.BankBranchCode == activeBankDetail.Code).ToList();
            // Update debit order controls in process
            debitControls.ForEach(debitControl =>
            {
              debitControl.Control.Bank = activeBankDetail.Bank;
              debitControl.Control.BankAccountName = activeBankDetail.AccountName;
              debitControl.Control.BankAccountNo = activeBankDetail.AccountNum;
              debitControl.Control.BankAccountType = activeBankDetail.AccountType;
              debitControl.Control.BankBranchCode = activeBankDetail.Code;
            });
          });
        }

        uow.CommitChanges();
      }
    }

  }
}
