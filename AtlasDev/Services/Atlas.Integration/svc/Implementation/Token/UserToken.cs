using System;
using Newtonsoft.Json;
using StackExchange.Redis;


namespace Atlas.Server.Implementation.Token
{
  /// <summary>
  /// Very simple class to issue/verify tokens with Redis caching/expiring the token
  /// </summary>
  public static class UserToken
  {
    private readonly static Lazy<ConnectionMultiplexer> _connection =
      new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("172.31.91.165"));


    public static string IssueNewToken(string userId, string branch, TimeSpan expiresIn)
    {
      var db = _connection.Value.GetDatabase();
      var token = string.Format("{0}{1}", Guid.NewGuid().ToString("N"), DateTime.Now.Ticks);
      var serialized = JsonConvert.SerializeObject(new UserInfo(DateTime.Now, branch, userId));
      db.StringSet(token, serialized, expiresIn);      
      return token;
    }


    public static bool TryGetUserInfo(string token, out string userId, out string branch)
    {
      var db = _connection.Value.GetDatabase();
      var val = db.StringGet(token);
      if (!string.IsNullOrEmpty(val))
      {
        var userInfo = JsonConvert.DeserializeObject<UserInfo>(val);
        branch = userInfo.Branch;
        userId = userInfo.UserId;        
        return true;
      }

      //Log.Warning("[TryGetUserInfo]- {Token} not found", token);
      userId = null;
      branch = null;
      return false;
    }


    sealed class UserInfo
    {
      public UserInfo(DateTime issued, string branch, string userId)
      {
        Issued = issued;
        Branch = branch;
        UserId = userId;
      }

      public DateTime Issued { get; set; }
      public string Branch { get; set; }
      public string UserId { get; set; }
    }

  }

}