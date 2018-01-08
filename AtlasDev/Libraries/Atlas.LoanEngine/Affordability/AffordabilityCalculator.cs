using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.LoanEngine.General;


namespace Atlas.LoanEngine.Affordability
{
  public class AffordabilityCalculator
  {
    /// <summary>
    /// Main Method to start the calculation process
    /// </summary>
    /// <param name="accountId">Id of account</param>
    /// <param name="createdById">Id of person performing the calculation</param>
    /// <param name="topUpId">Id of Top Up, if applicable</param>
    public List<ACC_AffordabilityOptionDTO> CalculateAffordabilityOptions(long accountId, long createdById, bool mainAffordabilityOnly = false, long? topUpId = null)
    {
      // Cache AccountType Items
      var cacheAttempts = 0;
      while (!Cache.Account.IsCached && cacheAttempts <= 3)
      {
        cacheAttempts++;
        Cache.Account.StartCache();
      }
      if (!Cache.Account.IsCached)
      {
        throw new Exception("Cannot cache Affordability Items");
      }

      // retrieve Bureau Expense and expense entered by client
      // choose the higher expense
      // get instalment for the applied amount
      using (var UoW = new UnitOfWork())
      {
        // Get Person object for user
        var createdBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(p => p.PersonId == createdById);

        // Get Account object
        var account = new XPQuery<ACC_Account>(UoW).FirstOrDefault(a => a.AccountId == accountId);

        // Check if Account Id is valid
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist in Database", accountId));

        // Get TopUp Object if it is not null
        ACC_TopUp topUp = null;
        if (topUpId != null)
        {
          topUp = new XPQuery<ACC_TopUp>(UoW).FirstOrDefault(t => t.TopUpId == (long)topUpId && t.Account == account);

          // Check if TopUp is valid
          if (topUp == null)
          {
            throw new Exception(string.Format("TopUp {0} does not exist in Database", accountId));
          }
        }

        // Get latest ITC Enquiry
        var riskEnquiry = new XPQuery<BUR_Enquiry>(UoW).Where(b => b.Account.AccountId == accountId && b.IsSucess &&
          b.EnquiryType == (int)Atlas.Enumerators.Risk.RiskEnquiryType.Credit).OrderByDescending(b => b.EnquiryDate).FirstOrDefault();

        if (riskEnquiry == null)
          throw new Exception(string.Format("Account {0} does not have any ITC enquiries", accountId));

        // Sum the Total Expenses from Bureau
        var totalBureauExpenses = new XPQuery<BUR_Accounts>(UoW).Where(b => b.Enquiry.EnquiryId == riskEnquiry.EnquiryId
          && b.Enabled).Sum(b => ((b.Instalment ?? 0) / ((b.JointParticipants ?? 1) > 0 ? (b.JointParticipants ?? 1) : 1)));

        // Get clients affordability
        var affordability = new XPQuery<ACC_Affordability>(UoW).Where(a => a.Account.AccountId == accountId
          && a.DeleteDate == null);

        // Get Total Expenses entered from Client
        var totalClientExpenses = affordability.Where(a =>
          a.AffordabilityCategory.AffordabilityCategoryType == Atlas.Enumerators.Account.AffordabilityCategoryType.Expense).Sum(a => a.Amount);

        // Use the MAX Expense
        var totalExpenses = (totalBureauExpenses > totalClientExpenses ? totalBureauExpenses : totalClientExpenses);
        if (totalExpenses <= 0)
        {
          // Failed. Cannot give any options to client who does not have any expenses
          throw new Exception("Person has no expenses");
        }

        // Get clients total Income
        var totalIncome = affordability.Where(a =>
          a.AffordabilityCategory.AffordabilityCategoryType == Atlas.Enumerators.Account.AffordabilityCategoryType.Income).Sum(a => a.Amount);

        if (totalIncome <= 0)
          throw new Exception("Client does not have any income");

        var maxInstalment = totalIncome - totalExpenses;

        // Clear older Options
        RemoveOlderOptions(UoW, account, topUp);

        var generatedOptions = new List<ACC_AffordabilityOptionDTO>();

        // Option 1
        Tuple<ACC_AccountTypeDTO, List<ACC_AccountTypeFeeDTO>> accountTypeFees = null;
        var affordabilityOption = DoMainCalculation(UoW, Atlas.Enumerators.Account.AffordabilityOptionType.RequestedOption, account, createdBy, out accountTypeFees,
          ref maxInstalment, topUp);

        var allowMoreOptions = affordabilityOption.AccountType.AllowAffordabilityOptions ?? false;

        if (affordabilityOption.Instalment <= maxInstalment)
        {
          // Client Can Afford loan
          affordabilityOption.CanClientAfford = true;
        }
        else
        {
          affordabilityOption.CanClientAfford = false;
          allowMoreOptions = true;
        }

        // If the Account Type allows more options, the value will be true even if the client can afford the current loan amount chosen
        if (allowMoreOptions && maxInstalment > 0)
        {
          // Calculate percentage difference for the maxInstalment and current Instlament
          var percentageDiff = ((maxInstalment / affordabilityOption.Instalment) * 100);

          if (affordabilityOption.CanClientAfford ?? false)
          {
            if (percentageDiff > 110)
            {
              // MaxInstalment
              var option = CalculateOption(UoW, Atlas.Enumerators.Account.AffordabilityOptionType.MaxInstalment, maxInstalment, affordabilityOption.Amount, percentageDiff ?? 100, account.Period, accountTypeFees.Item1, accountTypeFees.Item2,
                account, createdBy, topUp);
              if (!mainAffordabilityOnly && option != null)
                generatedOptions.Add(option);
            }
          }
          else
          {
            if (percentageDiff < 90)
            {
              // MaxInstalment
              var option = CalculateOption(UoW, Atlas.Enumerators.Account.AffordabilityOptionType.MaxInstalment, maxInstalment, affordabilityOption.Amount, percentageDiff ?? 100, account.Period, accountTypeFees.Item1, accountTypeFees.Item2,
                account, createdBy, topUp);
              if (!mainAffordabilityOnly && option != null)
                generatedOptions.Add(option);
            }
          }

          // MaxInstlment and Max Period
          if (accountTypeFees.Item1.MaxPeriod > account.Period && (accountTypeFees.Item1.MaxPeriod ?? 0) > 0)
          {
            var option = CalculateOption(UoW, Atlas.Enumerators.Account.AffordabilityOptionType.MaxInstalmentMaxPeriod, maxInstalment, affordabilityOption.Amount, percentageDiff ?? 100, (int)accountTypeFees.Item1.MaxPeriod, accountTypeFees.Item1, accountTypeFees.Item2,
              account, createdBy, topUp);
            if (!mainAffordabilityOnly && option != null)
              generatedOptions.Add(option);
          }
        }

        UoW.CommitChanges();

        generatedOptions.Add(AutoMapper.Mapper.Map<ACC_AffordabilityOption, ACC_AffordabilityOptionDTO>(affordabilityOption));

        return generatedOptions;
      }
    }


    /// <summary>
    /// Deletes older options created if exists
    /// </summary>
    /// <param name="uow">Connection to DB Session</param>
    /// <param name="account">Account to calculate on</param>
    /// <param name="topUp">Top Up object if client applying for TopUp</param>
    private void RemoveOlderOptions(UnitOfWork uow, ACC_Account account, ACC_TopUp topUp)
    {
      var olderOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(o => o.Account.AccountId == account.AccountId);

      if (topUp != null)
      {
        olderOptions = olderOptions.Where(o => o.TopUp.TopUpId == topUp.TopUpId);
      }

      //// Throws Exceptions if there are options already created and has not been cancelled/accepted/rejected
      //if (olderOptions.Count(o => o.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Atlas.Enumerators.Account.AffordabilityOptionStatus.Sent) > 0)
      //{
      //  throw new Exception("Cannot calculate more options. There are still options with Sent Statuses");
      //}

      // Throws Exceptions if there are options accepted. We cannot delete accepted options
      if (olderOptions.Count(o => o.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Atlas.Enumerators.Account.AffordabilityOptionStatus.Accepted) > 0)
        throw new Exception("Cannot calculate more options. There is an existing accepted option");

      var olderOptionFees = new XPQuery<ACC_AffordabilityOptionFee>(uow).Where(f => olderOptions.Contains(f.AffordabilityOption)).ToList();

      foreach (var olderOption in olderOptions.Where(a => a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.New || a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Sent))
      {
        uow.Delete(olderOption);
        uow.Delete(olderOptionFees.Where(a => a.AffordabilityOption == olderOption));
      }
    }


    /// <summary>
    /// Start of calculation for more options by the system
    /// </summary>
    /// <param name="uow">Connection to DB Session</param>
    /// <param name="optionType">Generally not RequestedOption</param>
    /// <param name="maxInstalment">max instalment calculated by main calculation</param>
    /// <param name="loanAmount">loan amount of account</param>
    /// <param name="percentageDiff">precentage diff of request option instalment and max instalment</param>
    /// <param name="period">length of loan</param>
    /// <param name="accountType">account type selected by main calculation</param>
    /// <param name="accountTypeFees">Fees relating to above account type</param>
    /// <param name="account">account affordability is calculated on</param>
    /// <param name="createdBy">person performing the calculation</param>
    /// <param name="topUp">if client is applying for a top up</param>
    private ACC_AffordabilityOptionDTO CalculateOption(UnitOfWork uow, Atlas.Enumerators.Account.AffordabilityOptionType optionType, decimal maxInstalment, decimal loanAmount, decimal percentageDiff,
      int period, ACC_AccountTypeDTO accountType, List<ACC_AccountTypeFeeDTO> accountTypeFees, ACC_Account account, PER_Person createdBy, ACC_TopUp topUp)
    {
      var affordabilityOption = CalculatePredictedInstlament(optionType, maxInstalment, loanAmount, percentageDiff, account.PeriodFrequency.Type, period, accountType,
        accountTypeFees, account.AccountBalance, topUp == null ? (decimal?)null : topUp.TopUpAmount);

      if (topUp != null)
      {
        var topUpAmount = affordabilityOption.Item1.Amount - account.AccountBalance;
      }

      return AddOption(uow, account, affordabilityOption, createdBy, topUp);
    }


    /// <summary>
    /// Calculates the predicted loan amount by the system, based on current instalment vs max instalment
    /// </summary>
    /// <param name="optionType">Generally not RequestedOption</param>
    /// <param name="maxInstalment">Max instalment the client can afford</param>
    /// <param name="loanAmount">loan amount of account</param>
    /// <param name="percentageDiff">precentage diff of request option instalment and max instalment</param>
    /// <param name="periodFrequency">daily/weekly/monthly</param>
    /// <param name="period">length of loan</param>
    /// <param name="accountType">account type selected by main calculation</param>
    /// <param name="accountTypeFees">Fees relating to above account type</param>
    /// <param name="accountBalance">balance of loan if client applying for top up</param>
    /// <param name="topUpAmount">top up amount if client applying for top up</param>
    /// <returns></returns>
    private Tuple<ACC_AffordabilityOptionDTO, List<ACC_AffordabilityOptionFeeDTO>> CalculatePredictedInstlament(Atlas.Enumerators.Account.AffordabilityOptionType optionType, decimal maxInstalment, decimal loanAmount, decimal percentageDiff, Atlas.Enumerators.Account.PeriodFrequency periodFrequency,
      int period, ACC_AccountTypeDTO accountType, List<ACC_AccountTypeFeeDTO> accountTypeFees, decimal accountBalance, decimal? topUpAmount)
    {
      Tuple<ACC_AffordabilityOptionDTO, List<ACC_AffordabilityOptionFeeDTO>> affordabilityOption;
      do
      {
        var predictedLoanAmount = (loanAmount * (percentageDiff / 100)) + loanAmount;
        predictedLoanAmount = predictedLoanAmount - (predictedLoanAmount % 50);

        affordabilityOption = DoOptionCalculation(optionType, periodFrequency, period, predictedLoanAmount, accountType, accountTypeFees, accountBalance, topUpAmount);

        percentageDiff -= 1;

      } while (affordabilityOption.Item1.Instalment > maxInstalment);

      affordabilityOption.Item1.CanClientAfford = true;

      return affordabilityOption;
    }


    /// <summary>
    /// This is the main calculation, and where the Account Type, etc is chosen for the Account
    /// More options should not be calculated if this method has not been calculated first
    /// </summary>
    /// <param name="uow">Connection to DB Session</param>
    /// <param name="optionType">Generally 'RequestedOption'</param>
    /// <param name="account">Account affordability is calculated on</param>
    /// <param name="createdBy">Person performing the calculation</param>
    /// <param name="accountTypeFee">fees& account type found by the below is cahced to be sent to the other methods</param>
    /// <param name="maxInstalment">max instalment is set here to allow more option to be calculated</param>
    /// <param name="topUp">applicable if client is applying for a top up</param>
    /// <returns></returns>
    private ACC_AffordabilityOption DoMainCalculation(UnitOfWork uow, Atlas.Enumerators.Account.AffordabilityOptionType optionType, ACC_Account account,
      PER_Person createdBy, out Tuple<ACC_AccountTypeDTO, List<ACC_AccountTypeFeeDTO>> accountTypeFee, ref decimal maxInstalment, ACC_TopUp topUp)
    {
      var option = new ACC_AffordabilityOption(uow) { Account = account };
      // option amount is calculated from top up amount + account balance if client applies for top up, otherwise new loans use the desired loan amount
      if (topUp != null)
      {
        option.TopUp = topUp;
        option.Amount = topUp.TopUpAmount + account.AccountBalance;
      }
      else
      {
        option.Amount = account.LoanAmount;
      }

      option.PeriodFrequency = account.PeriodFrequency;
      option.NumOfInstalment = account.PeriodFrequency.Type == Atlas.Enumerators.Account.PeriodFrequency.Daily ? 1 : account.Period;  // Daily loans only have 1 instalment
      option.AffordabilityOptionStatus = new XPQuery<ACC_AffordabilityOptionStatus>(uow).FirstOrDefault(a=>a.Type ==  Atlas.Enumerators.Account.AffordabilityOptionStatus.New);
      option.LastStatusDate = DateTime.Now;
      option.AffordabilityOptionType = new XPQuery<ACC_AffordabilityOptionType>(uow).FirstOrDefault(o=>o.Type == optionType);
      option.Period = account.Period;
      option.AccountType = account.AccountType;
      option.TotalFees = 0;
      option.CapitalAmount = option.Amount;
      option.CreatedBy = createdBy;
      option.CreatedDate = DateTime.Now;

      // if there's no preset accountType, then system chooses closest account type based on host, period, loan amount & ordinal 
      if (option.AccountType == null)
      {
        var accountType = Cache.Account.AccountTypeSet.Where(a => a.Host.HostId == account.Host.HostId
          && a.Enabled
          && a.PeriodFrequency.Type == option.PeriodFrequency.Type
          && a.MaxAmount >= option.Amount
          && a.MinAmount <= option.Amount
          && ((a.MaxPeriod >= option.Period
              && option.Period > 0)
            || option.Period == 0)).OrderBy(a => a.Ordinal).FirstOrDefault();

        if (accountType == null)
          throw new Exception("There's no Account Type matching for the applied loan");

        option.AccountType = new XPQuery<ACC_AccountType>(uow).FirstOrDefault(a => a.AccountTypeId == accountType.AccountTypeId);
        option.InterestRate = accountType.InterestRate;
      }
      else
      {
        // Set Max Amount possible for loan
        if (option.Amount > option.AccountType.MaxAmount && option.AccountType.MaxAmount > 0)
          option.Amount = option.AccountType.MaxAmount;

        // Set Max Period possible for loan
        if (option.Period > option.AccountType.MaxPeriod && option.AccountType.MaxPeriod > 0)
          option.Period = Convert.ToInt32(option.AccountType.MaxPeriod);
      }

      maxInstalment = maxInstalment * Convert.ToDecimal(((option.AccountType.AffordabilityPercentBuffer ?? 100) / 100));

      // Get fees relating to this account type
      var accountTypeFees = Cache.Account.AccountTypeFeeSet.Where(a => a.AccountType.AccountTypeId == option.AccountType.AccountTypeId
        && a.Enabled).ToList();

      // cache to be sent to other option calculations
      accountTypeFee = new Tuple<ACC_AccountTypeDTO, List<ACC_AccountTypeFeeDTO>>(AutoMapper.Mapper.Map<ACC_AccountType, ACC_AccountTypeDTO>(option.AccountType),
        accountTypeFees);

      var initialFees = new List<ACC_AccountTypeFeeDTO>();

      if (topUp != null)
      {
        initialFees = accountTypeFees.Where(a => a.Fee.AddWithNewTopUp).ToList();// Add fees for TopUp
      }
      else
      {
        initialFees = accountTypeFees.Where(a => a.Fee.IsInitial).ToList();// Add fees for new loan
      }

      // Calculate initial fees
      foreach (var initialFee in initialFees)
      {
        decimal amount = Calculations.CalculateFee(initialFee.Fee, option.Amount, option.Amount + option.TotalFees, option.Period);

        if (amount > 0)
        {
          var optionFee = new ACC_AffordabilityOptionFee(uow)
          {
            AffordabilityOption = option,
            AccountTypeFee = new XPQuery<ACC_AccountTypeFee>(uow).FirstOrDefault(a => a.AccountTypeFeeId == initialFee.AccountTypeFeeId),
            Amount = amount
          };

          option.TotalFees += amount;
        }
      }

      // Fees added to the loan amount to get the Capital Amount
      option.CapitalAmount += option.TotalFees;

      // Calculate interest charged Periods catering for interest free periods
      var interestPeriod = option.Period - (option.AccountType.InterestFreePeriods ?? 0);
      interestPeriod = interestPeriod < 0 ? 0 : interestPeriod;

      // Calculate RepoRate if applicable
      if (option.AccountType.RepoRate != null && option.AccountType.RepoFactor != null)
        option.InterestRate = Calculations.CalculateRepoRate((float)option.AccountType.RepoRate, (float)option.AccountType.RepoFactor, option.AccountType.InterestRate);
      else
        option.InterestRate = option.AccountType.InterestRate;

      // Calc Instalments
      option.Instalment = Calculations.CalculateInstalment(option.CapitalAmount, option.InterestRate ?? 0, option.Period, option.PeriodFrequency.Type);

      var accountTypeFeesMonthly = accountTypeFees.Where(a => a.AccountType.AccountTypeId == option.AccountType.AccountTypeId
        && a.Enabled && a.Fee.IsRecurring);

      // Calculate monthly fees, this will be added to instlament
      foreach (var accountTypeFeeMonthly in accountTypeFeesMonthly)
      {
        decimal amount = Calculations.CalculateFee(accountTypeFeeMonthly.Fee, option.Amount, option.CapitalAmount, option.Period);

        if (amount > 0)
        {
          var optionFee = new ACC_AffordabilityOptionFee(uow)
          {
            AffordabilityOption = option,
            AccountTypeFee = new XPQuery<ACC_AccountTypeFee>(uow).FirstOrDefault(a => a.AccountTypeFeeId == accountTypeFeeMonthly.AccountTypeFeeId),
            Amount = amount
          };

          option.Instalment += amount;
        }
      }

      // total pay back the client will make, if not defaulted, and paid back early
      option.TotalPayBack = option.Instalment * option.NumOfInstalment;

      return option;
    }


    /// <summary>
    /// Used to Add new option and fees to DB from DTO's
    /// </summary>
    /// <param name="uow">Connection to DB session</param>
    /// <param name="account">Account affordability is calculated on</param>
    /// <param name="options">calculated option to be added to DB</param>
    /// <param name="createdBy">Person performing Affordability</param>
    /// <param name="topUp">used only if client is applying for a top up</param>
    /// <returns></returns>
    private ACC_AffordabilityOptionDTO AddOption(UnitOfWork uow, ACC_Account account, Tuple<ACC_AffordabilityOptionDTO, List<ACC_AffordabilityOptionFeeDTO>> options, PER_Person createdBy, ACC_TopUp topUp = null)
    {
      // Create Option
      var option = new ACC_AffordabilityOption(uow);
      option.Account = account;
      option.TopUp = topUp;
      option.AccountType = new XPQuery<ACC_AccountType>(uow).FirstOrDefault(a => a.AccountTypeId == options.Item1.AccountType.AccountTypeId);
      option.Amount = options.Item1.Amount;
      option.PeriodFrequency = new XPQuery<ACC_PeriodFrequency>(uow).FirstOrDefault(f => f.Type == options.Item1.PeriodFrequency.Type);
      option.NumOfInstalment = options.Item1.PeriodFrequency.Type == Atlas.Enumerators.Account.PeriodFrequency.Daily ? 1 : options.Item1.Period;
      option.AffordabilityOptionStatus = new XPQuery<ACC_AffordabilityOptionStatus>(uow).FirstOrDefault(a => a.Type == options.Item1.AffordabilityOptionStatus.Type);
      option.LastStatusDate = DateTime.Now;
      option.AffordabilityOptionType = new XPQuery<ACC_AffordabilityOptionType>(uow).FirstOrDefault(o=>o.Type == options.Item1.AffordabilityOptionType.Type);
      option.Period = options.Item1.Period;
      option.TotalFees = options.Item1.TotalFees;
      option.CapitalAmount = option.Amount;
      option.CreatedBy = createdBy;
      option.CreatedDate = options.Item1.CreatedDate;
      option.InterestRate = options.Item1.InterestRate;
      option.CapitalAmount = options.Item1.CapitalAmount;
      option.Instalment = options.Item1.Instalment;
      option.TotalPayBack = options.Item1.TotalPayBack;
      option.CanClientAfford = options.Item1.CanClientAfford;

      var accountTypeFeesQuery = new XPQuery<ACC_AccountTypeFee>(uow).Where(a => a.AccountType.AccountTypeId == option.AccountType.AccountTypeId);
      var accountTypeFeeIds = options.Item2.Select(t => t.AccountTypeFee.AccountTypeFeeId).ToArray();
      var accountTypeFees = accountTypeFeesQuery.Where(a => accountTypeFeeIds.Contains(a.AccountTypeFeeId)).ToList();

      foreach (var fee in options.Item2)
      {
        var optionFee = new ACC_AffordabilityOptionFee(uow)
        {
          AffordabilityOption = option,
          AccountTypeFee = accountTypeFees.FirstOrDefault(a => a.AccountTypeFeeId == fee.AccountTypeFee.AccountTypeFeeId),
          Amount = fee.Amount
        };
      }

      uow.CommitChanges();
      return AutoMapper.Mapper.Map<ACC_AffordabilityOption, ACC_AffordabilityOptionDTO>(option);
    }


    /// <summary>
    /// Calculate options for MaxInstalment and Max Period
    /// </summary>
    /// <param name="optionType">can be but RequestedOption</param>
    /// <param name="periodFrequency">Daily/Weekly/Monthly</param>
    /// <param name="period">length of loan</param>
    /// <param name="loanAmount">amount to be borrowed</param>
    /// <param name="accountType">Account type of loan. Generally chosen from the Main Calculation/pre-selected</param>
    /// <param name="accountTypeFees">Fee relating to the above account type</param>
    /// <param name="accountBalance">balance of the current loan if client wants a top up</param>
    /// <param name="topUpAmount">Top Up client applied for</param>
    /// <returns></returns>
    private Tuple<ACC_AffordabilityOptionDTO, List<ACC_AffordabilityOptionFeeDTO>> DoOptionCalculation(Atlas.Enumerators.Account.AffordabilityOptionType optionType, Atlas.Enumerators.Account.PeriodFrequency periodFrequency,
      int period, decimal loanAmount, ACC_AccountTypeDTO accountType, List<ACC_AccountTypeFeeDTO> accountTypeFees, decimal? accountBalance = null, decimal? topUpAmount = null)
    {
      var option = new ACC_AffordabilityOptionDTO();
      var optionFees = new List<ACC_AffordabilityOptionFeeDTO>();

      option.AccountType = accountType;

      // calculate new option amount based on current account balance+top up amount
      if (topUpAmount != null)
      {
        option.Amount = Convert.ToDecimal(accountBalance) + Convert.ToDecimal(topUpAmount);
      }

      // Set the max amount that can be taken on loan based on account type
      if (loanAmount > accountType.MaxAmount && accountType.MaxAmount > 0)
        loanAmount = (decimal)accountType.MaxAmount;

      // Set the max period that can be taken on loan based on account type
      if (period > accountType.MaxPeriod && accountType.MaxPeriod > 0)
        period = Convert.ToInt32(accountType.MaxPeriod);

      option.Amount = loanAmount;
      option.TotalFees = 0;
      option.CapitalAmount = loanAmount;
      option.Period = period;
      option.PeriodFrequency = Cache.Account.PeriodFrequencySet.FirstOrDefault(f=>f.Type == periodFrequency);
      option.NumOfInstalment = periodFrequency == Atlas.Enumerators.Account.PeriodFrequency.Daily ? 1 : period;
      option.AffordabilityOptionStatus = Cache.Account.AffordabilityOptionStatusSet.FirstOrDefault(a=>a.Type == Atlas.Enumerators.Account.AffordabilityOptionStatus.New);
      option.AffordabilityOptionType = Cache.Account.AffordabilityOptionTypeSet.FirstOrDefault(a => a.Type == optionType);
        ;      option.InterestRate = accountType.InterestRate;
      option.CreatedDate = DateTime.Now;

      var initialFees = new List<ACC_AccountTypeFeeDTO>();

      if (topUpAmount != null)
      {
        initialFees = accountTypeFees.Where(a => a.Fee.AddWithNewTopUp).ToList();// Add fees for TopUp
      }
      else
      {
        initialFees = accountTypeFees.Where(a => a.Fee.IsInitial).ToList();// Add fees for new loan
      }

      // Calculate inital fees for new loan .top up
      foreach (var initialFee in initialFees)
      {
        decimal Amount = Calculations.CalculateFee(initialFee.Fee, option.Amount, option.Amount + option.TotalFees, option.Period);

        if (Amount > 0)
        {
          var optionFee = new ACC_AffordabilityOptionFeeDTO()
          {
            AffordabilityOption = option,
            AccountTypeFee = initialFee,
            Amount = Amount
          };

          optionFees.Add(optionFee);

          option.TotalFees += Amount;
        }
      }

      // Add fees to loan amount
      option.CapitalAmount += option.TotalFees;

      // Calculate interest charged Periods catering for interest free periods
      var interestPeriod = option.Period - (accountType.InterestFreePeriods ?? 0);
      interestPeriod = interestPeriod < 0 ? 0 : interestPeriod;

      // Calculate RepoRate if applicable
      if (accountType.RepoRate != null && accountType.RepoFactor != null)
      {
        option.InterestRate = Calculations.CalculateRepoRate((float)accountType.RepoRate, (float)accountType.RepoFactor, accountType.InterestRate);
      }

      // Calc Instalments
      option.Instalment = Calculations.CalculateInstalment(option.CapitalAmount, option.InterestRate ?? 0, period, option.PeriodFrequency.Type);

      // + Monthly Fees if monthly/weekly
      var accountTypeFeesMonthly = accountTypeFees.Where(a => a.AccountType.AccountTypeId == option.AccountType.AccountTypeId
        && a.Enabled && a.Fee.IsRecurring).ToList();

      //Calculate monthly fees, this is added to instalment but not capital balance
      foreach (var accountTypeFeeMonthly in accountTypeFeesMonthly)
      {
        decimal Amount = Calculations.CalculateFee(accountTypeFeeMonthly.Fee, option.Amount, option.CapitalAmount, option.Period);

        if (Amount > 0)
        {
          var optionFee = new ACC_AffordabilityOptionFeeDTO()
          {
            AffordabilityOption = option,
            AccountTypeFee = accountTypeFeeMonthly,
            Amount = Amount
          };

          optionFees.Add(optionFee);

          option.Instalment += Amount;
        }
      }

      // Total pay back will be number of instalments * instalment amount. This is a predicted amount, i.e., the client does not default/pay in advance
      option.TotalPayBack = option.Instalment * option.NumOfInstalment;

      return new Tuple<ACC_AffordabilityOptionDTO, List<ACC_AffordabilityOptionFeeDTO>>(option, optionFees);
    }

  }
}