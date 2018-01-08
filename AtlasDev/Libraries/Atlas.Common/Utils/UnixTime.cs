using System;


namespace Atlas.Common.Utils
{
  public sealed class UnixTime
  {    
    public UnixTime()
    {
      _date = DateTime.Now;
    }
    
    public UnixTime(int timestamp)
    {     
      _date = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).ToLocalTime();
    }
    
    public DateTime ToDateTime()
    {
      return _date;
    }
    
    public long ToTimeStamp()
    {
      var span = (_date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime());
      return Convert.ToInt64(span.TotalSeconds);
    }


    private readonly DateTime _date;

  }
}