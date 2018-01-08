using System;
using Atlas.Enumerators;

namespace Falcon.Gyrkin.Controllers.Api.Models
{
  public class DebitOrderModel
  {
    public class BatchQueryModel
    {
      public long? BatchId { get; set; }
      public long? BranchId { get; set; }
      public Debit.BatchStatus? BatchStatus { get; set; }
      public DateTime? StartRange { get; set; }
      public DateTime? EndRange { get; set; }
      public bool QueryBatchOnly { get; set; }
    }

    public class ControlQueryModel
    {
      public long controlId { get; set; }
      public int? specificRepetition { get; set; }
    }

    public class ControlsQueryModel
    {
      public General.Host? Host {get;set;}
      public long? BranchId {get;set;}
      public DateTime? StartRange {get;set;}
      public DateTime? EndRange {get;set;}
      public bool ControlOnly { get; set; }
    }

    public class AdditionalDebitOrderModel
    {
      public long ControlId { get; set; }
      public decimal Instalment { get; set; }
      public DateTime ActionDate { get; set; }
      public DateTime InstalmentDate { get; set; }
    }

    public class CancelAdditionalDebitOrderModel
    {
      public long ControlId {get;set;}
      public long TransactionId { get; set; }
    }
  }
}