using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.DebitOrder
{
  public class DebitOrderBatch : IDebitOrderBatch
  {
    public long BatchId { get; set; }
    public Debit.BatchStatus BatchStatus { get; set; }
    public string BatchStatusDescription { get; set; }
    public DateTime? LastStatusDate { get; set; }
    public DateTime? SubmitDate { get; set; }
    public DateTime CreateDate { get; set; }
    public int TransmissionNo { get; set; }
    public bool? TransmissionAccepted { get; set; }
    public List<IDebitOrderTransaction> NaedoBatchTransactions { get; set; }
  }
}
