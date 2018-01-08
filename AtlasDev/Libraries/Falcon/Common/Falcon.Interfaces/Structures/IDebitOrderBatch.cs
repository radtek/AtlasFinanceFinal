using System;
using System.Collections.Generic;
using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IDebitOrderBatch
  {
    long BatchId { get; set; }
    Debit.BatchStatus BatchStatus { get; set; }
    string BatchStatusDescription { get; set; }
    DateTime? LastStatusDate { get; set; }
    DateTime? SubmitDate { get; set; }
    DateTime CreateDate { get; set; }
    int TransmissionNo { get; set; }
    bool? TransmissionAccepted { get; set; }

    List<IDebitOrderTransaction> NaedoBatchTransactions { get; set; }
  }
}
