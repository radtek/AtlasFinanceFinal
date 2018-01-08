using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.WebService;

namespace Atlas.Online.Web.Models.Dto
{
  public class AffordabilityDto
  {
    public LoanDto Loan { get; set; }
    public ApplicationAffordability Afford { get; set; }

    public LoanDto Diff
    {
      get
      {
        return new LoanDto()
        {
          Amount          = Afford.Amount - Loan.Amount,
          Period          = Afford.Period - Loan.Period,
          RepaymentAmount = Afford.RepaymentAmount - Loan.RepaymentAmount,
          RepaymentDate   = DateTime.Now.AddDays(Afford.Period)
        };
      }
    }
  }
}