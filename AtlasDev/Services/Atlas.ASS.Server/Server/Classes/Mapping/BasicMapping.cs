using System;


namespace Atlas.Server.Classes.Mapping
{
  internal static class BasicMapping
  {
    public static Atlas.Server.Classes.PersonSecurity MapPersonSecurity(Domain.Model.PER_Security source)
    {
      return new Atlas.Server.Classes.PersonSecurity
      {
        CreatedBy = source.CreatedBy?.PersonId,
        CreatedDT = source.CreatedDT,       
        DeletedBy = source.DeletedBy?.PersonId,
        DeletedDT = source.DeletedDT,
        Hash = source.Hash,
        InvalidUserNameOrPassword = source.InvalidUserNameOrPassword,
        IP = source.IP,
        IsActive = source.IsActive,
        IsLocked = source.IsLocked,
        LastEditedBy = source.LastEditedBy?.PersonId,
        LastEditedDT = source.LastEditedDT,
        LastLoggedIn = source.LastLoggedIn,
        LegacyOperatorId = source.LegacyOperatorId,
        LoggedIn = source.LoggedIn,
        LogginAttemptCount = source.LogInAttemptCount,
        Person = source.Person?.PersonId,
        Salt = source.Salt,
        SecurityId = source.SecurityId,
        Username = source.Username
      };
    }

  }   

}
