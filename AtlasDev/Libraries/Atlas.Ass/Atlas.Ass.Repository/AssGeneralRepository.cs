using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Framework.Structures.Stream;
using Atlas.Ass.Repository.Properties;
using Atlas.Ass.Structures.Stream;
using Falcon.Common.Interfaces.Services;

namespace Atlas.Ass.Repository
{
  public class AssGeneralRepository : IAssGeneralRepository
  {
    private readonly IConfigService _configService;

    public AssGeneralRepository(IConfigService configService)
    {
      _configService = configService;
    }

    public List<long> GetDeceasedClients(List<long> clientReferences)
    {
      return GetCustomQuery(string.Format(Resources.QRY_GetDeceasedClients,
        !clientReferences.Any() ? "0" : string.Join(",", clientReferences)));
    }

    public List<IAccountTransaction> GetAccountTransactions(long accountReference, DateTime? lastImportReference = null)
    {
      var date = SqlDateTime.MinValue.Value;
      if (lastImportReference.HasValue && lastImportReference.Value > SqlDateTime.MinValue.Value)
      {
        date = lastImportReference.Value;
      }
      var accountTransactions = new List<IAccountTransaction>();
      var transactions = GetCustomQuery<AccountTransaction>(string.Format(Resources.QRY_STR_ImportAccountTransactions,
        accountReference, date.ToString("yyyy-MM-dd")));
      accountTransactions.AddRange(transactions.OrderBy(t => t.Seqno));
      return accountTransactions;
    }

    public List<long> CheckHandoverClients(params long[] clientIds)
    {
      return GetCustomQuery(string.Format(Resources.QRY_STR_ClientsHandedOver,
        !clientIds.Any() ? "0" : string.Join(",", clientIds)));
    }

    public bool CheckClientsWithNewLoans(long clientId, DateTime dateFrom)
    {
      var sb = new StringBuilder();
      sb.AppendLine(string.Format("WHEN CL.recid = {0} THEN to_date('{1}', 'YYYY-MM-DD')", clientId,
        dateFrom.ToString("yyyy-MM-dd")));

      var clients = GetCustomQuery<ClientLoan>(
        string.Format(Resources.QRY_STR_ClientsTookNewerLoan,
          clientId, sb));

      return clients.Count > 0;
    }

    internal List<long> GetCustomQuery(string sql)
    {
      var queryUtil = new Common.Utils.RawSql();
      var data = queryUtil.ExecuteObject(sql, _configService.AssConnection, commandTimeout: 3600);
      return data;
    }

    internal List<T> GetCustomQuery<T>(string sql) where T : class, new()
    {
      var queryUtil = new Common.Utils.RawSql();
      var data = queryUtil.ExecuteObject<T>(sql, _configService.AssConnection, commandTimeout: 3600);
      return data;
    }
  }
}
