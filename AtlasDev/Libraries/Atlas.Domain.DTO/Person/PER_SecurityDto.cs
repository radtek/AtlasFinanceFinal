
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for PersonSecurity
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PER_SecurityDTO
  {
    [DataMember]
    public Int64 SecurityId { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public string LegacyOperatorId { get; set; }
    [DataMember]
    public string Username { get; set; }
    [DataMember]
    public string Salt { get; set; }
    [DataMember]
    public string Hash { get; set; }
    [DataMember]
    public string IP { get; set; }
    [DataMember]
    public bool IsActive { get; set; }
    [DataMember]
    public bool IsLocked { get; set; }
    [DataMember]
    public bool InvalidUserNameOrPassword { get; set; }
    [DataMember]
    public DateTime? LastLoggedIn { get; set; }
    [DataMember]
    public bool LoggedIn { get; set; }
    [DataMember]
    public int LogginAttemptCount { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public PER_PersonDTO DeletedBy { get; set; }
    [DataMember]
    public PER_PersonDTO LastEditedBy { get; set; }
    [DataMember]
    public DateTime? CreatedDT { get; set; }
    [DataMember]
    public DateTime? DeletedDT { get; set; }
    [DataMember]
    public DateTime? LastEditedDT { get; set; }
    [DataMember]
    public DateTime? DateCreated { get; set; }
  }
}
