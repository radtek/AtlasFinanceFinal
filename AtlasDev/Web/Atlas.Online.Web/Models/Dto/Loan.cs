using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Common.Serializers;
using Atlas.Online.Web.Validations;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models.Dto
{
  [Serializable]
  public class LoanDto
  {
    public static readonly string CookieKey = typeof(LoanDto).Name;
    private Application application;

    public LoanDto() { }
    public LoanDto(Application application)
    {      
      this.application = application;
      this.Populate();
    }
    
    [Required]
    [Display(Name = "Loan Amount")]
    [Currency(Symbol = "R ")]
    public decimal Amount { get; set; }
    [Required]
    [Display(Name = "Loan Period")]
    public int Period { get; set; }
    [Display(Name="Repayment Amount")]
    [Currency(Symbol = "R ", Decimals=2)]
    public decimal RepaymentAmount { get; set; }
    [Display(Name = "Repayment Date")]
    public DateTime RepaymentDate { get; set; }
    [Required(ErrorMessage="The Reason for Loan is required.")]
    [EnumRequired(WebEnumerators.LoanReason.NotSet)]
    [Display(Name = "Reason for Loan")]
    public WebEnumerators.LoanReason Reason { get; set; }

    public string ToJson()
    {
      return JsonNet.Serialize<LoanDto>(this, Newtonsoft.Json.TypeNameHandling.All);
    }

    public static LoanDto FromCookie(HttpRequestBase request)
    {
      // Get Slider results from cookie
      HttpCookie cookie = request.Cookies[LoanDto.CookieKey];
      if (cookie == null)
      {
        return null;
      }

      return JsonNet.Deserialize<LoanDto>(cookie.Value, Newtonsoft.Json.TypeNameHandling.All);
    }

    public void Populate()
    {
      Amount = application.Amount;
      Period = application.Period;

      RepaymentDate = application.RepaymentDate;
      RepaymentAmount = application.RepaymentAmount;
    
      Reason = application.Reason != null ? application.Reason.Type : default(WebEnumerators.LoanReason);
    }

    public void SetApplication(ref Data.Models.Definitions.Application application)
    {      
      application.Amount = this.Amount;
      application.RepaymentAmount = this.RepaymentAmount;
      if (application.Period != this.Period)
      {
        application.RepaymentDate = DateTime.Now.AddDays(this.Period);
      }

      application.Period = this.Period;
      application.Reason = new XPQuery<LoanReason>(application.Session).FirstOrDefault(x => x.ReasonId == Convert.ToInt32(this.Reason));
    }
  }
}