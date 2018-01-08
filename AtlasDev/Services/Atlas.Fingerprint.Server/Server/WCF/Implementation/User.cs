using System;


namespace Atlas.WCF.FPServer.WCF.Implementation
{
  public class User
  {
    public User()
    {
      
    }

    public User(Int64 userId, Int64 personId, string legacyOperatorId, string idOrPassport)
    {
      UserId = userId;
      PersonId = personId;
      LegacyOperatorId = legacyOperatorId;
      IDOrPassport = idOrPassport;
    }

    public Int64 UserId { get; private set; }

    public  Int64 PersonId { get; private set; }

    public string LegacyOperatorId { get; private set; }

    public string IDOrPassport { get; private set; }

  }
}
