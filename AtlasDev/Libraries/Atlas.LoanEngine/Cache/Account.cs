using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Collections.Concurrent;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using System.Threading;


namespace Atlas.LoanEngine.Cache
{
  public static class Account
  {
    #region Public properties

    /// <summary>
    /// Is Caching successflluy done
    /// </summary>
    public static bool IsCached = false;

    /// <summary>
    /// Account types
    /// </summary>
    public static ConcurrentBag<ACC_AccountTypeDTO> AccountTypeSet;

    /// <summary>
    /// Account fees
    /// </summary>
    public static ConcurrentBag<ACC_AccountTypeFeeDTO> AccountTypeFeeSet;

    /// <summary>
    /// Period frequency
    /// </summary>
    public static ConcurrentBag<ACC_PeriodFrequencyDTO> PeriodFrequencySet;

    /// <summary>
    /// Affordability options status
    /// </summary>
    public static ConcurrentBag<ACC_AffordabilityOptionStatusDTO> AffordabilityOptionStatusSet;

    /// <summary>
    /// 
    /// </summary>
    public static ConcurrentBag<ACC_AffordabilityOptionTypeDTO> AffordabilityOptionTypeSet;

    #endregion


    #region Public Methods

    /// <summary>
    /// main call into the cache, gets the cache started, object initialized and timer started
    /// </summary>
    public static void StartCache()
    {
      _cacheRefresh = new System.Timers.Timer(3600000);// Every 60 minutes
      _cacheRefresh.Elapsed += cachingTimer_Elapsed;
      _cacheRefresh.Disposed += cachingTimer_Disposed;
      _cacheRefresh.Enabled = true;

      LoadCache();
    }

    /// <summary>
    /// Stops caching and clears objects
    /// </summary>
    public static void StopCache()
    {
      _cacheRefresh.Dispose();
    }

    #endregion


    #region Private Methods

    /// <summary>
    /// Refreshes cached objects
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void cachingTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      _cacheRefresh.Enabled = false;
      try
      {
        LoadCache();
      }
      finally
      {
        _cacheRefresh.Enabled = true;
      }
    }


    private static void LoadCache()
    {
      try
      {
        using (var UoW = new UnitOfWork())
        {
          var accountTypes = new XPQuery<ACC_AccountType>(UoW).Where(a => a.Enabled & a.Host.HostId > 0).ToList();
          var accountTypeFees = new XPQuery<ACC_AccountTypeFee>(UoW).Where(a => a.Enabled && accountTypes.Contains(a.AccountType)).ToList();
          var periodFrequencies = new XPQuery<ACC_PeriodFrequency>(UoW).ToList();
          var affordabilityOptionStatuses = new XPQuery<ACC_AffordabilityOptionStatus>(UoW).ToList();
          var affordabilityOptionTypes = new XPQuery<ACC_AffordabilityOptionType>(UoW).ToList();
          
          var accountTypeSet = new ConcurrentBag<ACC_AccountTypeDTO>();
          foreach (var accountType in accountTypes)
          {
            accountTypeSet.Add(AutoMapper.Mapper.Map<ACC_AccountType, ACC_AccountTypeDTO>(accountType));
          }
          Interlocked.Exchange<ConcurrentBag<ACC_AccountTypeDTO>>(ref AccountTypeSet, accountTypeSet);

          var accountTypeFeeSet = new ConcurrentBag<ACC_AccountTypeFeeDTO>();
          foreach (var accountTypeFee in accountTypeFees)
          {
            accountTypeFeeSet.Add(AutoMapper.Mapper.Map<ACC_AccountTypeFee, ACC_AccountTypeFeeDTO>(accountTypeFee));
          }
          Interlocked.Exchange<ConcurrentBag<ACC_AccountTypeFeeDTO>>(ref AccountTypeFeeSet, accountTypeFeeSet);

          var periodFrequencySet = new ConcurrentBag<ACC_PeriodFrequencyDTO>();
          foreach (var periodFrequency in periodFrequencies)
          {
            periodFrequencySet.Add(AutoMapper.Mapper.Map<ACC_PeriodFrequency, ACC_PeriodFrequencyDTO>(periodFrequency));
          }
          Interlocked.Exchange<ConcurrentBag<ACC_PeriodFrequencyDTO>>(ref PeriodFrequencySet, periodFrequencySet);

          var affordabilityOptionStatusSet = new ConcurrentBag<ACC_AffordabilityOptionStatusDTO>();
          foreach (var item in affordabilityOptionStatuses)
          {
            affordabilityOptionStatusSet.Add(AutoMapper.Mapper.Map<ACC_AffordabilityOptionStatus, ACC_AffordabilityOptionStatusDTO>(item));
          }
          Interlocked.Exchange<ConcurrentBag<ACC_AffordabilityOptionStatusDTO>>(ref AffordabilityOptionStatusSet, affordabilityOptionStatusSet);

          var affordabilityOptionTypeSet = new ConcurrentBag<ACC_AffordabilityOptionTypeDTO>();
          foreach (var item in affordabilityOptionTypes)
          {
            affordabilityOptionTypeSet.Add(AutoMapper.Mapper.Map<ACC_AffordabilityOptionType, ACC_AffordabilityOptionTypeDTO>(item));
          }
          Interlocked.Exchange<ConcurrentBag<ACC_AffordabilityOptionTypeDTO>>(ref AffordabilityOptionTypeSet, affordabilityOptionTypeSet);

          IsCached = true;
        }

      }
      catch (Exception)
      {
        IsCached = false;
      }

    }

    /// <summary>
    /// Clears cached objects
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    static void cachingTimer_Disposed(object sender, EventArgs e)
    {
      AccountTypeSet = null;
      AccountTypeFeeSet = null;
      PeriodFrequencySet = null;
      AffordabilityOptionStatusSet = null;
      AffordabilityOptionTypeSet = null;
    }

    #endregion


    #region Private properties

    private static System.Timers.Timer _cacheRefresh;

    #endregion

  }
}
