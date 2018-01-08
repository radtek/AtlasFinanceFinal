using Atlas.Enumerators;
using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models.Steps
{
  public class QuoteAcceptance : ApplicationStepBase
  {
    private LoanDto _loan;

    #region Pre-agreement Statement and Quotation
    public string LoanReference { get; set; }
    public string QuotationNo { get; set; }
    public DateTime QuotationDate { get; set; }
    #endregion

    #region Applicant Details
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public string IdNumber { get; set; }
    public string ContactNo { get; set; }
    public string ResidentialAddress1 { get; set; }
    public string ResidentialAddress2 { get; set; }
    public string ResidentialAddress3 { get; set; }
    public string ResidentialCity { get; set; }
    #endregion

    #region Loan Details
    public LoanDto Loan 
    {
      get { return _loan ?? (_loan = new LoanDto()); }
      set { _loan = value; }
    }
    [Currency(Symbol = "R ")]
    public decimal InitiationFee { get; set; }
     [Currency(Symbol = "R ")]
    public decimal ServiceFee { get; set; }
    public decimal Interest { get; set; }
    #endregion

    #region Banking
    public BankDetailDto Bank { get; set; }
    [Currency(Symbol = "R ")]
    public decimal DebitAmount { get; set; }
    public DateTime DateOfDebit { get; set; }
    #endregion    

    public override int Id
    {
      get { return 6; }
    }    

    public override void Save(ref Application application, HttpRequestBase request) {}

    public void Populate(WebService.Quotation quote)
    {
      this.Bank = new BankDetailDto()
      {
        AccountHolder = quote.BankAccountName,
        AccountNo = quote.BankAccountNo,
        AccountType = quote.BankAccountType,
        BankName = (WebEnumerators.BankName)quote.Bank
      };
      
      this.Loan.Amount = quote.Amount;
      this.Loan.RepaymentAmount = quote.RepaymentAmount;
      this.Loan.RepaymentDate = quote.RepaymentDate;

      this.Firstname = quote.FirstName;
      this.Surname = quote.LastName;
      
      this.ContactNo = quote.ContactNumber;
      this.IdNumber = quote.IdNumber;
      this.ResidentialAddress1 = quote.ResidentialAddressLine1;
      this.ResidentialAddress2 = quote.ResidentialAddressLine2;
      this.ResidentialAddress3 = quote.ResidentialAddressLine3;
      this.ResidentialCity = quote.ResidentialAddressLine4;

      this.DateOfDebit = quote.DateOfDebit;
      this.DebitAmount = quote.DebitAmount;
      this.Interest = quote.InterestRate;
      this.InitiationFee = quote.InitiationFee;
      this.ServiceFee = quote.ServiceFee;

      this.LoanReference = quote.AccountNo;

      this.QuotationDate = quote.QuoteDate;
      this.QuotationNo = quote.QuotationNo;
      
    }

    public override void Populate(Application application)
    {
      throw new NotImplementedException();
    }

 
  }
}