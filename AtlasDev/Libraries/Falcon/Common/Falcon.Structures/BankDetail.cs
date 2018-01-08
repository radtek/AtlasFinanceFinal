using System;

namespace Falcon.Common.Structures
{
  public sealed class BankDetail
  {
    public Int64 DetailId { get; set; }
    public Bank Bank { get; set; }
    public long BankAccountTypeId { get; set; }
    public string BankAccountType { get; set; }
    public string AccountName { get; set; }
    public string AccountNum { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreatedDT { get; set; }
  }
}
