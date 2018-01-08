using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models.Dto
{
  public class LoanRulesDto
  {
    public const decimal INTEREST_RATE = 60M/365M;
    public const decimal SERVICE_FEE = 50;

    public int MaxLoanAmount { get; set; }
    public int MinLoanAmount { get; set; }

    public int MaxLoanPeriod { get; set; }
    public int MinLoanPeriod { get; set; }

    public int ReapplyInDay { get; set; }

    public int TotalDay { get; set; }

    public decimal InterestRate { get; set; }

    public int DefaultTerm { get; set; }

    public bool CanApply { get; set; }

    public static LoanRulesDto CreateDefault()
    {
      return new LoanRulesDto()
      {
        MaxLoanAmount = 2500,
        MaxLoanPeriod = 50,
        MinLoanAmount = 200,
        MinLoanPeriod = 2,
        ReapplyInDay = 0,
        TotalDay = 10,
        InterestRate = INTEREST_RATE,
        DefaultTerm = 5,
        CanApply = true
      };
    }
  }
}