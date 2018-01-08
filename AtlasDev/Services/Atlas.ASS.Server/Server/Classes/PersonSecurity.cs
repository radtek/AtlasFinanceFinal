using System;


namespace Atlas.Server.Classes
{
  /// <summary>
  /// Cleaner version of PER_SecurityDTO
  /// </summary>
  internal class PersonSecurity
  {    
    public long SecurityId { get; set; }
   
    public long? Person { get; set; }
   
    public string LegacyOperatorId { get; set; }
  
    public string Username { get; set; }
   
    public string Salt { get; set; }
   
    public string Hash { get; set; }
    
    public string IP { get; set; }
   
    public bool IsActive { get; set; }
   
    public bool IsLocked { get; set; }
 
    public bool InvalidUserNameOrPassword { get; set; }
   
    public DateTime? LastLoggedIn { get; set; }
   
    public bool LoggedIn { get; set; }
   
    public int LogginAttemptCount { get; set; }
   
    public long? CreatedBy { get; set; }
   
    public long? DeletedBy { get; set; }
   
    public long? LastEditedBy { get; set; }
    
    public DateTime? CreatedDT { get; set; }
  
    public DateTime? DeletedDT { get; set; }
    
    public DateTime? LastEditedDT { get; set; }
   
  }
}
