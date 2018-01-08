using System;
using System.Collections.Generic;
using System.Linq;

using Atlas.Common.Interface;
using Atlas.Enumerators;
using AtlasServer.WCF.Interface;
using Atlas.Server.Classes.CustomException;


/*
1) If the loan is paid-up (i.e. Loans->outamnt = 0)
  a. Get max(TRANS->trdate) from trans where:
     i. TRTYPE is != 'R' and TRTYPE <> TRSTAT (these are actual transactions)
  
2) If the loan is still open (i.e. Loans->outamnt <> 0)   
  a. Get max(TRANS->trdate) from trans where:
     i. TRTYPE = anything
*/

namespace AtlasServer.WCF.Implementation
{
  internal static class GetLastClientTransaction_Impl
  {
    internal static int Execute(ILogging log, IConfigSettings config,
      SourceRequest sourceRequest, string idOrPassport,
      out string lastBranchNum, out DateTime lastTransactionDate, out string errorMessage)
    {
      lastBranchNum = string.Empty;
      lastTransactionDate = DateTime.MinValue;
      errorMessage = string.Empty;
      var methodName = "GetLastClientTransaction";
      var branchCode = sourceRequest.BranchCode.ToUpper().PadLeft(3, '0');
      try
      {
        log.Information("{MethodName} starting- {@SourceRequest}- {idOrPassport}", methodName, sourceRequest, idOrPassport);

        #region Check parameters
        if (string.IsNullOrEmpty(sourceRequest.BranchCode))
        {
          throw new BadParamException("Missing branch number");
        }

        if (string.IsNullOrEmpty(sourceRequest.UserIDOrPassport))
        {
          throw new BadParamException("Missing operator ID");
        }

        if (string.IsNullOrEmpty(sourceRequest.MachineUniqueID))
        {
          throw new BadParamException("Missing machine fingerprint");
        }

        if (string.IsNullOrEmpty(sourceRequest.AppVer))
        {
          throw new BadParamException("Missing application version");
        }

        if (string.IsNullOrWhiteSpace(idOrPassport) || idOrPassport.Length < 5)
        {
          throw new BadParamException("Missing/invalid 'idOrPassport' parameter");
        }
        #endregion

        var data = new List<TransDetails>();
        using (var conn = new Npgsql.NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandTimeout = 30; // seconds
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = @"
                WITH CTE_ID_NUM_MATCHING AS
                  (SELECT l.brnum, l.client, l.loan, l.outamnt
                   FROM company.client c
                   JOIN company.loans l on c.client = l.client and c.lrep_brnum = l.lrep_brnum " +
                   $"WHERE (c.identno = '{idOrPassport.Trim()}') AND (l.nctrantype IN ('USE', 'N/A')) " + @"
                   )
  
                SELECT t.brnum, t.client, t.loan, t.trdate, t.trtype, t.trstat, m.outamnt
                FROM company.trans t, CTE_ID_NUM_MATCHING m
                WHERE m.brnum = t.brnum AND m.client = t.client AND m.loan = t.loan";

            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                data.Add(new TransDetails(rdr));
              }
            }
          }
        }

        if (data.Any())
        {
          log.Information("{MethodName} Loaded {count} transactions", methodName, data.Count);
          var grouped = data.GroupBy(s => $"{s.brnum}{s.client}{s.loan}");
          foreach (var trans in grouped)
          {            
            var maxTransByDate = (trans.First().outamnt == 0) ?
              trans.Where(s => s.trtype != "R" && s.trtype != s.trstat).OrderByDescending(s => s.trdate).FirstOrDefault() :
              trans.OrderByDescending(s => s.trdate).FirstOrDefault();

            if (maxTransByDate != null && maxTransByDate.trdate > lastTransactionDate && maxTransByDate.brnum != branchCode)
            {
              lastBranchNum = maxTransByDate.brnum;
              lastTransactionDate = maxTransByDate.trdate;
            }
          }
        }

        log.Information("{MethodName} result- {lastTransactionDate}, {lastBranchNum}", methodName, lastTransactionDate, lastBranchNum);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return (err is BadParamException) ? (int)General.WCFCallResult.BadParams : (int)General.WCFCallResult.ServerError;
      }
    }

  }


  class TransDetails
  {
    public TransDetails(System.Data.IDataReader reader)
    {
      brnum = reader.GetString(0).PadLeft(3, '0');
      client = reader.GetString(1);
      loan = reader.GetString(2);
      trdate = reader.GetDateTime(3);
      trtype = reader.GetString(4);
      trstat = !reader.IsDBNull(5) ? reader.GetString(5) : null;
      outamnt = !reader.IsDBNull(6) ? reader.GetDecimal(6) : 0;
    }

    public string brnum;
    public string client;
    public string loan;
    public DateTime trdate;
    public string trtype;
    public string trstat;
    public decimal outamnt;
  }
}
