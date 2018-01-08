using System;

namespace Falcon.Gyrkin.Controllers.Api.Models.Avs
{
  public class AvsTransactionQueryModel
  {
    public long? BranchId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public long? TransactionId { get; set; }
    public string IdNumber { get; set; }
    public long? BankId { get; set; }
  }
}
