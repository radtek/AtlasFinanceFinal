using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models.Dto
{
  public class LoanRulesDto
  {
    public const decimal INTEREST_RATE = 60M/365M;

    public int MaxLoanAmount { get; set; }
    public int MinLoanAmount { get; set; }

    public int MaxLoanPeriod { get; set; }
    public int MinLoanPeriod { get; set; }

    public decimal InterestRate { get; set; }

    public int DefaultTerm { get; set; }


    public static LoanRulesDto CreateDefault()
    {
      return new LoanRulesDto()
      {
        MaxLoanAmount = 2500,
        MaxLoanPeriod = 31,
        MinLoanAmount = 200,
        MinLoanPeriod = 2,
        InterestRate = INTEREST_RATE,
        DefaultTerm = 5
      };
    }
  }
}