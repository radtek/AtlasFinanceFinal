using System;
using System.Collections.Generic;
using Atlas.Ass.Framework.Structures.Stream;

namespace Atlas.Ass.Framework.Repository
{
  public interface IAssGeneralRepository
  {
    List<long> GetDeceasedClients(List<long> clientReferences);
    /// <summary>
    /// Gets transactions for the specified account and since the last import reference or all
    /// </summary>
    /// <param name="accountReference"></param>
    /// <param name="lastImportReference"></param>
    /// <returns></returns>
    List<IAccountTransaction> GetAccountTransactions(long accountReference, DateTime? lastImportReference = null);

    /// <summary>
    /// Given a list of client id's, filter out good clients, and return clients with handedover loans within the last 12 months (according to SQL query)
    /// </summary>
    /// <param name="clientIds">list of client ids to filter</param>
    /// <returns>the client ids with handed over loans</returns>
    List<long> CheckHandoverClients(params long[] clientIds);
    
    /// <summary>
    /// Given a client and a date, return a true if the client has taken out a newer loan since the given date
    /// </summary>
    /// <param name="clientId">list of client ids to filter</param>
    /// <param name="dateFrom">date to check from </param>
    /// <returns>a true if the client has a newer loan, otherwise false</returns>
    bool CheckClientsWithNewLoans(long clientId, DateTime dateFrom);
  }
}
