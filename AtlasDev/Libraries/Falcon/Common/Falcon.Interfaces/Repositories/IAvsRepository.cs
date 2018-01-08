using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Interfaces.Structures.AVS;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IAvsRepository
  {
    List<IBank> GetSupportedBanks();
    List<IAvsService> GetActiveServices();
    Task<Tuple<List<IAvsStatistics>, List<IAvsTransactions>>> GetCachedTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId);
    List<IAvsTransactions> GetTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId);
    List<IAvsStatistics> GetStats(IList<IAvsTransactions> transactions);
    bool Resend(long transactionId, int serviceId);
    void UpdateServiceSchedule(Dictionary<IAvsService, List<IAvsServiceBank>> newServiceSchedules);
  }
}
