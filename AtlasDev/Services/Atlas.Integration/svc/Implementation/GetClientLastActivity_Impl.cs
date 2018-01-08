using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

using Atlas.Integration.Interface;
using Atlas.Server.Implementation.Token;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class GetClientLastActivity_Impl
  {
    internal static ClientLastActivityResult GetClientLastActivity(ILogging log, IConfigSettings config,
      string loginToken, ClientLastActivityRequest request)
    {
      try
      {
        log.Information("[GetClientLastActivity]- {LoginToken}-{@request}", loginToken, request);

        #region Basic validation
        if (string.IsNullOrEmpty(loginToken))
        {
          return new ClientLastActivityResult() { Error = "Token cannot be empty- login first!" };
        }
        string userId;
        string branch;
        if (!UserToken.TryGetUserInfo(loginToken, out userId, out branch))
        {
          return new ClientLastActivityResult() { Error = "Login token invalid/has expired" };
        }

        if (request == null)
        {
          return new ClientLastActivityResult() { Error = "Parameter 'request' cannot be empty" };
        }

        if (string.IsNullOrEmpty(request.IdNumberOrPassport) || request.IdNumberOrPassport.Length < 5 || request.IdNumberOrPassport.Length > 20)
        {
          return new ClientLastActivityResult() { Error = "Parameter 'request.IdNumberOrPassport' cannot be empty and must be 5-20 characters" };
        }
        var cleanedId = Regex.Replace(request.IdNumberOrPassport, @"[^\d]", string.Empty);
        if (cleanedId.Length < 5)
        {
          return new ClientLastActivityResult() { Error = "Parameter 'request.IdNumberOrPassport' must consist of at least 5 characters" };
        }
        #endregion

        using (var conn = new Npgsql.NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            var foundAtBranches = new List<Tuple<string, string>>();
            cmd.CommandText = "SELECT lrep_brnum, client FROM company.client WHERE identno = @identno ORDER BY userdate DESC NULLS LAST LIMIT 10";
            cmd.Parameters.AddWithValue("identno", NpgsqlTypes.NpgsqlDbType.Char, 20, request.IdNumberOrPassport);
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                foundAtBranches.Add(new Tuple<string, string>(rdr.GetString(0), rdr.GetString(1)));
              }
            }

            if (!foundAtBranches.Any())
            {
              return new ClientLastActivityResult() { };
            }

            var activities = new List<LastActivity>();
            var pairs = string.Join(" OR ", foundAtBranches.Select(s => string.Format("(lrep_brnum='{0}' AND client='{1}')", s.Item1, s.Item2)));
            #region Receipts/paid
            cmd.CommandText = string.Format("SELECT trdate, lrep_brnum FROM company.trans WHERE (trtype = 'P') AND ({0}) ORDER BY trdate DESC NULLS LAST LIMIT 1", pairs);
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                activities.Add(new LastActivity()
                {
                  Activity = ActivityTypeEnum.InstalmentPaid,
                  Date = (DateTime)rdr.GetDate(0),
                  Branch = rdr.GetString(1)
                });
              }
            }
            #endregion

            #region Loans
            cmd.CommandText = string.Format("SELECT status, loandate, lrep_brnum, client, loan FROM company.loans WHERE ({0}) ORDER BY loan DESC NULLS LAST LIMIT 10", pairs);
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                switch (rdr.GetString(0))
                {
                  case "F": // finished
                    activities.Add(new LastActivity()
                    {
                      Activity = ActivityTypeEnum.LoanClosed,
                      Date = (DateTime)rdr.GetDate(1),
                      Notes = string.Format("Branch: {0}, Client: {1}, Loan: {2}", rdr.GetString(2), rdr.GetString(3), rdr.GetString(4)),
                      Branch = rdr.GetString(2)
                    });
                    break;

                  case "P": // part-paid
                  case "N": // new
                    activities.Add(new LastActivity()
                    {
                      Activity = ActivityTypeEnum.NewLoan,
                      Date = (DateTime)rdr.GetDate(1),
                      Notes = string.Format("Branch: {0}, Client: {1}, Loan: {2}", rdr.GetString(2), rdr.GetString(3), rdr.GetString(4)),
                      Branch = rdr.GetString(2)
                    });
                    break;

                  case "H": // handed-over
                    activities.Add(new LastActivity()
                    {
                      Activity = ActivityTypeEnum.HandedOver,
                      Date = (DateTime)rdr.GetDate(1),
                      Notes = string.Format("Branch: {0}, Client: {1}, Loan: {2}", rdr.GetString(2), rdr.GetString(3), rdr.GetString(4)),
                      Branch = rdr.GetString(2)
                    });
                    break;
                }
              }
            }
            #endregion

            return new ClientLastActivityResult() { Activities = activities.ToArray() };
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, "GetClientLastActivity()");
        return new ClientLastActivityResult() { Error = "Unexpected server error" };
      }
    }
  }
}