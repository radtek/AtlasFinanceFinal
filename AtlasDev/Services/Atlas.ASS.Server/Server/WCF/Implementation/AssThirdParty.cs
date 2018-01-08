using System;
using System.ServiceModel;
using System.Data;

using DevExpress.Xpo;
using Npgsql;

using Atlas.WCF.Interface;
using Atlas.Domain.Model;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace Atlas.WCF.Implementation
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class AssThirdParty : IAssThirdParty
  {
    public AssThirdParty(ILogging log, ICacheServer cache, IConfigSettings config)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    public long AddUserOverride(AddUserOverrideArgs userOverrideArgs)
    {
      _log.Information("AddUserOverride- {@userOverrideArgs}", userOverrideArgs);

      Int64 result = -1;
      try
      {
        #region Validate
        if (userOverrideArgs.StartDate < DateTime.Today)
        {
          _log.Error("AddUserOverride- Invalid {StartDate}", userOverrideArgs.StartDate);
          return -1;
        }
        if (userOverrideArgs.EndDate > DateTime.Today.AddDays(1))
        {
          _log.Error("AddUserOverride- Invalid {EndDate}", userOverrideArgs.EndDate);
          return -1;
        }
        if (string.IsNullOrEmpty(userOverrideArgs.UserOperatorCode) || userOverrideArgs.UserOperatorCode.Length != 4)
        {
          _log.Error("AddUserOverride- Invalid {userOperatorCode}", userOverrideArgs.UserOperatorCode);
          return -1;
        }
        if (string.IsNullOrEmpty(userOverrideArgs.BranchNum) || userOverrideArgs.BranchNum.Length != 2)
        {
          _log.Error("AddUserOverride- Invalid {BranchNum}", userOverrideArgs.BranchNum);
          return -1;
        }
        if (string.IsNullOrEmpty(userOverrideArgs.RegionalOperatorId) || userOverrideArgs.RegionalOperatorId.Length != 4)
        {
          _log.Error("AddUserOverride- Invalid {RegionalOperatorId}", userOverrideArgs.RegionalOperatorId);
          return -1;
        }
        if (userOverrideArgs.NewLevel == 0 || userOverrideArgs.NewLevel > 9)
        {
          _log.Error("AddUserOverride- Invalid {NewLevel}", userOverrideArgs.NewLevel);
          return -1;
        }
        #endregion

        #region Add to ass
        decimal recId = 0;
        using (var conn = new NpgsqlConnection(_config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "INSERT INTO company.asstauth(authdate, oper, brnum, popup_code, oper_reg, temp_level, end_date) " +
              "VALUES(:authdate, :oper, :brnum, :popup_code, :oper_reg, :temp_level, :end_date) RETURNING sr_recno";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 20;
            cmd.Parameters.AddWithValue("authdate", NpgsqlTypes.NpgsqlDbType.Date, userOverrideArgs.StartDate);
            cmd.Parameters.AddWithValue("oper", NpgsqlTypes.NpgsqlDbType.Text, userOverrideArgs.UserOperatorCode);
            cmd.Parameters.AddWithValue("brnum", NpgsqlTypes.NpgsqlDbType.Text, userOverrideArgs.BranchNum.PadLeft(2, '0'));
            cmd.Parameters.AddWithValue("popup_code", NpgsqlTypes.NpgsqlDbType.Text, "N/A"); // look-up...?
            cmd.Parameters.AddWithValue("oper_reg", NpgsqlTypes.NpgsqlDbType.Text, userOverrideArgs.RegionalOperatorId);
            cmd.Parameters.AddWithValue("temp_level", NpgsqlTypes.NpgsqlDbType.Text, userOverrideArgs.NewLevel.ToString());
            cmd.Parameters.AddWithValue("end_date", NpgsqlTypes.NpgsqlDbType.Date, userOverrideArgs.EndDate);

            using (var rdr = cmd.ExecuteReader())
            {
              if (rdr.Read())
              {
                recId = rdr.GetDecimal(0);
              }
            }

            if (recId > 0)
            {
              // Update SQLRDD synthetic index column
              cmd.CommandText = string.Format(
                "UPDATE company.asstauth " +
                "SET indkey_001 = to_char(authdate, 'yyyyMMDD') || brnum || oper || substring(to_char(sr_recno, lpad('', 15, '9')) from 2) " +  // dtos(authdate)+brnum+oper
                "WHERE sr_recno = {0}", recId);
              cmd.ExecuteNonQuery();
            }
          }
        }
        #endregion

        #region Add to ass record tracking
        if (recId > 0)
        {
          using (var uow = new UnitOfWork())
          {
            var addTracking = new ASS_MasterTableChangeTracking(uow)
            {
              ChangedTS = DateTime.Now,
              TableName = "asstauth",
              KeyFieldName = "sr_recno",
              KeyFieldValue = recId.ToString("N0")
            };
            uow.CommitChanges();
            result = addTracking.RecId;
          }
        }
        #endregion

        _log.Information("AddUserOverride completed successfully {recId}", recId);
      }
      catch (Exception err)
      {
        _log.Error(err, "AddUserOverride");
      }

      return result;
    }


    private readonly ILogging _log;
    private readonly ICacheServer _cache;
    private readonly IConfigSettings _config;

  }
}
