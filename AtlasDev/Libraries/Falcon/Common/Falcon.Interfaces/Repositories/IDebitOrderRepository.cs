using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IDebitOrderRepository
  {
    IList<IDebitOrderBatch> GetBatches(long? batchId, long? branchId, Debit.BatchStatus? batchStatus, DateTime? startRange, DateTime? endRange, bool queryBatchOnly);
    IDebitOrderControl GetControl(long controlId, int? specificRepetition);
    IList<IDebitOrderControl> GetControls(General.Host? host, long? branchId, DateTime? startRange, DateTime? endRange, bool controlOnly);
    IDebitOrderControl AddAdditionalDebitOrder(long controlId, decimal instalment, DateTime actionDate, DateTime instalmentDate);
    IDebitOrderControl CancelAdditionalDebitOrder(long controlId, long transactionId);
  }
}
