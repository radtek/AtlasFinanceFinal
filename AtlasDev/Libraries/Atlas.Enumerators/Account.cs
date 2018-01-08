using System;
using System.ComponentModel;

namespace Atlas.Enumerators
{
  public class Account
  {
    public enum CreditPolicy
    {
      [Description("Debt Restructuring")]
      Debt_Restructure = 1,
      [Description("Deceased")]
      Deceased = 2,
      [Description("Administration")]
      Administration = 3,
      [Description("Judgements")]
      Judgements = 4,
      [Description("Adverses")]
      Adverse = 5,
      [Description("Accounts in arrears last 90 days")]
      Arrears90Days = 6,
      [Description("Disputed Accounts")]
      Dispute = 7,
      [Description("Under Sequestration")]
      Sequestration = 8
    }

    public enum Policy
    {
      [Description("Not Set")]
      Not_Set = 0,
      [Description("Permanently Employed")]
      Permanently_Employed = 1,
      [Description("Paid via EFT")]
      Paid_Via_Eft = 2,
      [Description("Employed - Minimum Period of Employment")]
      Employed_Minimum_Period = 3,
      [Description("Three (3) Monthly Instalments Paid on Current Account(s)")]
      ThreeMonthlyInstalmentsPaidCurrent = 4,
      [Description("Retirement Age")]
      Retirement_Age = 5,
      [Description("Latest Three (3) Monthly Instalments Paid on Current Account(s)")]
      LatestThreeMonthlyInstalmentPaidCurrentAccounts = 6,
      [Description("Net Income - Minimum")]
      NetIncomeMinimum = 7,
      [Description("Employer Type ")]
      EmployerType = 8,
      [Description("Age - Minimum of 21 Years")]
      AgeMinimum21OfYears = 9,
      [Description("Client Status - Current")]
      Client_Status_Current = 10,
      [Description("Employment Status - Current")]
      Employment_Status_Current = 11,
      [Description("Employer is Blocked")]
      Employer_Blocked = 12,
      [Description("Account Status - Current")]
      Account_Status_Current = 13,
      [Description("Account Status - Previous")]
      Account_Status_Previous = 14,
      [Description("Fraudulent Alert - Declined previous account")]
      Fraud_Alert_Declined_Previous_Account = 15,
      [Description("Age - Maximum of 60 Years")]
      AgeMax60Years = 16,
      [Description("Age - Maximum of 65 Years")]
      AgeMax65Years = 17,
      [Description("Salary Bank Account")]
      Salary_Bank_Account = 18,
      [Description("Country of Birth")]
      Country_Of_Birth = 19,
      [Description("Maximum Open Accounts")]
      Maximum_Open_Accounts = 20,
      [Description("Latest 2 Instalments Not Yet Paid")]
      Latest_Two_Instalments_Not_Yet_Paid = 21,
      [Description("Account Status - Pending")]
      Account_Status_Pending = 22,
      [Description("Client Credit Risk")]
      Client_Credit_Risk = 23,
      [Description("Company Policy")]
      Company_Policy = 24,
      [Description("Account Status - Review")]
      Account_Status_Review = 25,
      [Description("Account Status - PreApproved")]
      Account_Status_PreApproved = 26,
      [Description("Account Status - Approved")]
      Account_Status_Approved = 27,
      [Description("Account Status - Cancelled")]
      Account_Status_Cancelled = 28,
      [Description("Account Status - Inactive")]
      Account_Status_Inactive = 29,
      [Description("Handed Over")]
      Handed_Over = 30,
      [Description("Bad Debt")]
      Bad_Debt = 31
    }


    public enum ArrearageCycle
    {
      [Description("Cycle 1")]
      Cycle01 = 1,
      [Description("Cycle 2")]
      Cycle02 = 2,
      [Description("Cycle 3")]
      Cycle03 = 3,
      [Description("Cycle 4")]
      Cycle04 = 4,
      [Description("Cycle 5")]
      Cycle05 = 5,
      [Description("Cycle 6")]
      Cycle06 = 6,
      [Description("Cycle 7")]
      Cycle07 = 7,
      [Description("Cycle 8")]
      Cycle08 = 8,
      [Description("Cycle 9")]
      Cycle09 = 9,
      [Description("Cycle 10")]
      Cycle10 = 10,
      [Description("Cycle 11")]
      Cycle11 = 11,
      [Description("Cycle 12")]
      Cycle12 = 12
    }

    public enum PeriodFrequency
    {
      [Description("Daily")]
      Daily = 1,
      [Description("Weekly")]
      Weekly = 2,
      [Description("Monthly")]
      Monthly = 3,
      [Description("Bi-Weekly")]
      BiWeekly = 4
    }

    public enum ScoreResult
    {
      [Description("Accepted")]
      Accepted = 1,
      [Description("Accepted - Override")]
      Accepted_Override = 2,
      [Description("Declined")]
      Declined = 3
    }

    /// <summary>
    /// Various account statuses
    /// </summary>
    public enum AccountStatus
    {
      [Description("Inactive")]
      Inactive = 0,
      [Description("Technical Error")]
      Technical_Error = 5,
      [Description("Pending")]
      Pending = 10,
      [Description("Cancelled")]
      Cancelled = 15,
      [Description("Declined")]
      Declined = 20,
      [Description("Review")]
      Review = 25,
      [Description("Pre-Approved")]
      PreApproved = 30,
      [Description("Approved")]
      Approved = 35,
      [Description("Open")]
      Open = 40,
      [Description("Legal")]
      Legal = 50,
      [Description("Written Off")]
      WrittenOff = 60,
      [Description("Settled")]
      Settled = 70,
      [Description("Closed")]
      Closed = 80
    }

    public enum AccountStatusReason
    {
      [Description("Not Set")]
      Not_Set = 0,
      [Description("Affordability")]
      Affordability = 1,
      [Description("Excessive Loans")]
      ExcessiveLoans = 2,
      [Description("Under Administration")]
      UnderAdministration = 3,
      [Description("Employer Declined")]
      EmployerDeclined = 4,
      [Description("Debit Orders Not Allowed")]
      DebitOrdersNotAllowed = 5,
      [Description("No Employment")]
      NoEmployment = 6,
      [Description("Possibly Fraudulent")]
      PossiblyFraudulent = 7,
      [Description("Verification Failed")]
      VerificationFailed = 8,
      [Description("Salary Not EFT")]
      SalaryNotEFT = 9,
      [Description("Possible Retrenchment")]
      PossibleRetrenchment = 10,
      [Description("Indefinite Sick Leave")]
      IndefiniteSickLeave = 11,
      [Description("Employee on Disabliity")]
      EmployeeonDisabliity = 12,
      [Description("Forthcoming Maternity Leave")]
      ForthcomingMaternityLeave = 13,
      [Description("Part-Time Employee")]
      PartTimeEmployee = 14,
      [Description("Contract Employee")]
      ContractEmployee = 15,
      [Description("Commission-Based Employee")]
      CommissionBasedEmployee = 16,
      [Description("Employed Under 6 Months")]
      EmployedUnder6Months = 17,
      [Description("Client Under 21 Years of Age")]
      ClientUnder21YearsofAge = 18,
      [Description("Pensioner")]
      Pensioner = 19,
      [Description("Net Salary Less Than R1000")]
      NetSalaryLessThanR1000 = 20,
      [Description("Handed Over")]
      HandedOver = 21,
      [Description("Fixed arrangement on current loans")]
      Fixedarrangementoncurrentloans = 22,
      [Description("Credit Risk")]
      CreditRisk = 23,
      [Description("Loan Application Incomplete/Incorrect")]
      LoanApplicationIncompleteIncorrect = 24,
      [Description("Under Debt Review")]
      UnderDebtReview = 25,
      [Description("Quotation Expired")]
      QuotationExpired = 26,
      [Description("Net Salary Less Than R1500")]
      NetSalaryLessThanR1500 = 27,
      [Description("Company Policy")]
      CompanyPolicy = 28,
      [Description("Employer Blocked")]
      EmployerBlocked = 29,
      [Description("Legal Account")]
      LegalAccount = 30,
      [Description("Under Debt Mediation")]
      UnderDebtMediation = 31,
      [Description("Fraud")]
      Fraud = 32,
      [Description("Client Status")]
      ClientStatus = 33,
      [Description("No Longer Interested")]
      NoLongerInterested = 34,
      [Description("Rejected Options")]
      RejectedOptions = 35,
      [Description("Uncontactable")]
      Uncontactable = 36,
      [Description("Technical Error")]
      TechnicalError = 37,
      [Description("Product Was Never Required")]
      ProductWasNeverRequired = 38,
      [Description("Multiple SAFPS Incident Listing")]
      MultipleSAFPSIncidentListing = 39,
      [Description("Bad Debt")]
      BadDebt = 40,
      [Description("Disability")]
      Disability = 41,
      [Description("Deceased")]
      Deceased = 42,
      [Description("Retrenched")]
      Retrenched = 43,
      [Description("Authentication")]
      Authentication = 44
    }

    public enum AccountStatusSubReason
    {
      [Description("Not Set")]
      Not_Set = 0,
      [Description("Personal - Confirmed")]
      PersonalConfirmed = 1,
      [Description("Personal - Suspect")]
      PersonalSuspect = 2,
      [Description("Identity Theft - Confirmed")]
      IdentityTheftConfirmed = 3,
      [Description("Identity Theft - Suspect")]
      IdentityTheftSuspect = 4,
      [Description("Age - Maximum of 60 Years")]
      AgeMaximumof60Years = 5,
      [Description("Age - Minimum of 21 Years")]
      AgeMinimumof21Years = 6,
      [Description("Country of Birth")]
      CountryofBirth = 7,
      [Description("Employed - Minimum Period of Employment")]
      EmployedMinimumPeriodofEmployment = 8,
      [Description("Employer is Blocked")]
      EmployerIsBlocked = 9,
      [Description("Employer Type ")]
      EmployerType = 10,
      [Description("Fraudulent Alert - Declined previous loan")]
      FraudulentAlertDeclinedPreviousLoan = 11,
      [Description("Latest Instalments Paid")]
      LatestInstalmentsPaid = 12,
      [Description("Loan Status - Current ")]
      LoanStatusCurrent = 13,
      [Description("Loan Status - Previous")]
      LoanStatusPrevious = 14,
      [Description("Net Income - Minimum")]
      NetIncomeMinimum = 15,
      [Description("Paid Via EFT")]
      PaidViaEFT = 16,
      [Description("Permanently Employed")]
      PermanentlyEmployed = 17,
      [Description("Retirement Age")]
      RetirementAge = 18,
      [Description("Salary Bank Account")]
      SalaryBankAccount = 19,
      [Description("Status - Current")]
      StatusCurrent = 20,
      [Description("Three (3) Monthly Instalments Paid")]
      ThreeMonthlyInstalmentsPaid = 21,
      [Description("Under Administration")]
      UnderAdministration = 22,
      [Description("Under Debt Review")]
      UnderDebtReview = 23,
      [Description("Maximum Open Loans")]
      MaximumOpenLoans = 24,
      [Description("Affordability - No Options")]
      AffordabilityNoOptions = 25,
      [Description("SAFPS - Multiple Incident Listings")]
      SAFPSMultipleIncidentListings = 26
    }

    public enum AffordabilityCategoryType
    {
      [Description("Expense")]
      Expense = 1,
      [Description("Income")]
      Income = 2,
      [Description("Display")]
      Display = 3
    }

    public enum Affordability
    {
      [Description("Gross Salary")]
      Gross_Salary = 1,
      [Description("Nett Salary")]
      Nett_Salary = 2,
      [Description("Other Income")]
      Other_Income = 3,
      [Description("Rent")]
      Rent = 4,
      [Description("Entertainment")]
      Entertainment = 5,
      [Description("Other Expenses")]
      Other_Expenses = 6,
      [Description("Total Expenses")]
      Total_Expenses = 7
    }

    public enum AffordabilityOptionStatus
    {
      [Description("New")]
      New = 1,
      [Description("Sent")]
      Sent = 2,
      [Description("Accepted")]
      Accepted = 3,
      [Description("Cancelled")]
      Cancelled = 4,
      [Description("Rejected")]
      Rejected = 5
    }

    public enum AffordabilityOptionType
    {
      [Description("Requested Option")]
      RequestedOption = 1,
      [Description("Max Instalment")]
      MaxInstalment = 2,
      [Description("Max Instalment Max Period")]
      MaxInstalmentMaxPeriod = 3
    }

    public enum PayDateType
    {
      [Description("Day Of Week")]
      DayOfWeek = 1,
      [Description("Day Of Month")]
      DayOfMonth = 2
    }

    public enum SettlementStatus
    {
      [Description("New")]
      New = 1,
      [Description("Sent")]
      Sent = 2,
      [Description("Missed")]
      Missed = 3,
      [Description("Settled")]
      Settled = 4,
      [Description("Cancelled")]
      Cancelled = 5
    }

    public enum SettlementType
    {
      [Description("Normal")]
      Normal = 1,
      [Description("Inter-Account")]
      InterAccount = 2
    }

    public enum PayRule
    {
      [Description("Fri , Sat, Sun -> Monday")]
      Fri_Sat_Sun_To_Mon = 1,
      [Description("Sat , Sun -> Friday")]
      Sat_Sun_To_Fri = 2,
      [Description("Sat , Sun -> Monday")]
      Sat_Sun_To_Mon = 3,
      [Description("Sat -> Friday, Sun -> Monday")]
      Sat_To_Fri_Sun_To_Mon = 4,
      [Description("Sun -> Monday")]
      Sun_To_Mon = 5,
      [Description("Sat , Sun , Monday -> Friday")]
      Sat_Sun_Mon_To_Friday = 6,
      [Description("Last working day of month")]
      Last_Working_Day_Of_Month = 7,
      [Description("2nd Last working day of month")]
      Second_Last_Working_Day_Of_Month = 8,
      [Description("Last Sunday")]
      Last_Sunday = 9,
      [Description("Last Monday")]
      Last_Monday = 10,
      [Description("Last Tuesday")]
      Last_Tuesday = 11,
      [Description("Last Wednesday")]
      Last_Wednesday = 12,
      [Description("Last Thursday")]
      Last_Thursday = 13,
      [Description("Last Friday")]
      Last_Friday = 14,
      [Description("Last Saturday")]
      Last_Saturday = 15,
      [Description("Second last Friday")]
      Second_Last_Friday = 16,
      [Description("Friday Before or On the 25th")]
      Friday_Before_Or_On_the_25th = 17
    }

    public enum QuotationStatus
    {
      [Description("New")]
      New = 1,
      [Description("Issued")]
      Issued = 2,
      [Description("Accepted")]
      Accepted = 3,
      [Description("Rejected")]
      Rejected = 4,
      [Description("Expired")]
      Expired = 5
    }
  }
}
