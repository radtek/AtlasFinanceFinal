using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class ApplicationDto
  {
    public int ApplicationId { get; set; }
    public long OptionId { get; set; }
    public string AccountNo { get; set; }
    public long? AccountId { get; set; }
    public ClientDto Client { get; set; }
    public BankDetailDto BankDetail { get; set; }
    public AddressDto ResidentialAddress { get; set; }
    public AddressDto EmployerAddress { get; set; }
    public AffordabilityDto Affordability { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal? OtherIncome { get; set; }
    public int Period { get; set; }
    public int Step { get; set; }
    public bool IsCurrent { get; set; }
    public Account.AccountStatus Status { get; set; }
    public DateTime RepaymentDate { get; set; }
    public string Hash { get; set; }
    public DateTime CreateDate { get; set; }
  }
}