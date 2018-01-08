using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models
{

  public class StatementModel
  {
    public string ClientName { get; set; }
    public string IDNo { get; set; }
    public string PhysicalAddress { get; set; }
    public string ContactNo { get; set; }
    public string AccountNo { get; set; }
    public decimal LoanAmount { get; set; }
    public DateTime RepaymentDate { get; set; }
    public int DaysOverdue { get; set; }
    public int Term { get; set; }
    public decimal RepaymentAmount { get; set; }
    public decimal AmountOverdue { get; set; }
    public float MonlthyInterest { get; set; }
    public DateTime StartDate { get; set; }
    public decimal CurrentBalance { get; set; }

    public DateTime StatementDate { get; set; }
    public decimal CurrentDue { get; set; }
    public decimal Arrears { get; set; }
    public decimal TotalDue { get; set; }

    public List<StatementListModel> StatementList { get; set; }

    public sealed class StatementListModel
    {
      public DateTime Date { get; set; }
      public string Description { get; set; }
      public decimal Debit { get; set; }
      public decimal Credit { get; set; }
      public decimal Balance { get; set; }
    }

    public decimal Arrears150 { get; set; }
    public decimal Arrears120 { get; set; }
    public decimal Arrears90 { get; set; }
    public decimal Arrears60 { get; set; }
    public decimal Arrears30 { get; set; }
    public decimal ArrearsCurrent { get; set; }
    public decimal ArrearsTotalDue { get; set; }

    public decimal PaymentReceived { get; set; }
    public decimal Interestedaccured { get; set; }
    public decimal InterestLevied { get; set; }
    public decimal LegalFeesLevied { get; set; }
    public decimal AdminFeesLevied { get; set; }
    public decimal OtherDebits { get; set; }
    public decimal OtherCredits { get; set; }
  }
}