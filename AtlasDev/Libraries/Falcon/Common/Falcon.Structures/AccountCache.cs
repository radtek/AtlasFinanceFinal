using System;

namespace Falcon.Common.Structures
{
  /// <summary>
  /// Used to store cache result of account in redis.
  /// </summary>
  public class AccountCache
  {
    /// <summary>
    /// Account detail record to store in cache
    /// </summary>
    public AccountDetail AccountDetail { get; set; }
    /// <summary>
    /// When last the record was updated from the database
    /// </summary>
    public DateTime LastUpdateDate { get; set; }
    /// <summary>
    /// Touch time of record, if requested touch is increment.
    /// </summary>
    public DateTime TouchDate { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public AccountCache(AccountDetail accountDetail, DateTime lastUpdateDate, DateTime touchDate)
    {
      AccountDetail = accountDetail;
      LastUpdateDate = lastUpdateDate;
      TouchDate = touchDate;
    }
    public AccountCache() { }

  }
}