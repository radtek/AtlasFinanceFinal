using System;

namespace Atlas.Domain.DTO
{
  public class AEDOLoginDTO
  {
    public Int64 AEDOLoginId { get; set; }
    public string MerchantNum { get; set; }
    public string Password { get; set; }
    public DateTime DeletedDT { get; set; }
    public string UserName { get; set; }
    public string LendorType { get; set; }

  }
}
