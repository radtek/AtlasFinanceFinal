using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class BankDetailDto
  {
    public int BankDetailId { get; set; }
    public BankDto Bank { get; set; }    
    public BankAccountTypeDto AccountType { get; set; }
    public BankPeriodDto Period { get; set; }
    public string AccountNo { get; set; }
    public string AccountName { get; set; }
    public bool IsActive { get; set; }
    public bool IsVerified { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime CreateDate { get; set; }

  }
}