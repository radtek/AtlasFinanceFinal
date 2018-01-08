using System;
using System.Collections.Generic;
using System.Data;

using Dapper;
using Npgsql;

using Atlas.Evolution.Server.Code.Data.ASS_Models;


namespace Atlas.Evolution.Server.Code.Data
{
  /// <summary>
  /// ASS Database Dapper DB repository
  /// </summary>
  internal class AssRepository : IDisposable
  {    
    public AssRepository(string connectionString)
    {
      dbConnection = new NpgsqlConnection(connectionString);
      dbConnection.Open();
    }

    public IEnumerable<ASS_Models.loans> GetAllLoans()
    {
      return dbConnection.Query<loans>("SELECT * FROM company.loans", null, null, true, 600, CommandType.Text);
    }

    public IEnumerable<loans> GetLoans(string sql)
    {
      return dbConnection.Query<loans>(sql, null, null, true, 120);
    }


    public IEnumerable<clients> GetAllClients()
    {
      return dbConnection.Query<clients>("SELECT * FROM company.client", null, null, true, 600, CommandType.Text);
    }


    public IEnumerable<clients> GetClients(string sql)
    {
      return dbConnection.Query<clients>(sql, null, null, true, 120);
    }


    public IEnumerable<occupations> GetOccupations()
    {
      return dbConnection.Query<occupations>("SELECT occup, \"name\", lrep_brnum FROM company.occup", null, null, true, 60);
    }


    public void Dispose()
    {
      dbConnection.Close();
    }

    internal IEnumerable<payplan_h> GetPayPlanH(string sql)
    {
      return dbConnection.Query<payplan_h>(sql, null, null, true, 120);
    }
      
    internal IEnumerable<trans> GetTrans(string sql)
    {
      return dbConnection.Query<trans>(sql, null, null, true, 600);
    }


    private readonly IDbConnection dbConnection;

  }

}
