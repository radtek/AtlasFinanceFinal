using Atlas.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Interfaces
{
  public interface IAVSUtil
  {
    List<Tuple<AVS_TransactionDTO, bool>> PerformAVSEnquiry(string username, string key, AVS_BatchDTO batch, List<AVS_TransactionDTO> transactions,
      Delegate updateBatchAction, Delegate importAction);
    Tuple<AVS_TransactionDTO, bool> GetResponse(string username, string key, AVS_TransactionDTO pendingTransaction);
  }
}
