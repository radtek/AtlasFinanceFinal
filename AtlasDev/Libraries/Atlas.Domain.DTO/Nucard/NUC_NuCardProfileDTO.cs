/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Profile
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


namespace Atlas.Domain.DTO.Nucard
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NUC_NuCardProfileDTO
  {
    [DataMember]
    public Int64 NuCardProfileId { get; set; }
    [DataMember]
    public string ProfileNum { get; set; }
    [DataMember]
    public string TerminalId { get; set; }
    [DataMember]
    public string Password { get; set; }
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
  }
}
