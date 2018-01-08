using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Validations;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Atlas.Online.Web.Models.Steps
{
  public class IncomeExpensesStep : ApplicationStepBase
  {
    public override int Id { get { return 3; } }

    [Required]
    public LoanDto Loan { get; set; }

    [Required]
    [Currency(Symbol="R ")]
    [Display(Name="Total net income")]
    public decimal TotalNetIncome { get; set; }

    [Display(Name = "Other income")]
    [Currency(Symbol = "R ")]
    public decimal? OtherIncome { get; set; }

    [Required]
    [Display(Name = "Total expenses")]
    [Currency(Symbol = "R ")]
    public decimal TotalExpenses { get; set; }

    [Required(ErrorMessage="Please confirm that the above information is true and correct.")]    
    public bool TrueAndCorrect { get; set; }
    
    public override void Populate(Application application)
    {
      this.Loan = new LoanDto(application);

      this.TotalNetIncome = application.TotalIncome;
      
      this.TotalExpenses = application.TotalExpenses;

      this.OtherIncome = application.OtherIncome;

      this.TrueAndCorrect = false;
    }

    public override void Save(ref Application application, HttpRequestBase request)
    {
      if (this.Loan != null) {
        this.Loan.SetApplication(ref application);
      }

      application.TotalIncome = this.TotalNetIncome;      

      application.TotalExpenses = this.TotalExpenses;

      application.OtherIncome = this.OtherIncome;

      application.Save();
    }
  }
}