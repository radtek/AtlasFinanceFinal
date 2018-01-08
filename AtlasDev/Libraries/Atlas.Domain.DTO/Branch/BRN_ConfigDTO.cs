/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Config
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
  [DataContract]
  public class BRN_ConfigDTO
  {
    [DataMember]
    public Int64 BranchConfigId { get; set; }
    [DataMember]
    public BRN_BranchDTO Branch { get; set; }
    [DataMember]
    public Enumerators.General.BranchConfigDataType DataType { get; set; }
    [DataMember]
    public string DataSection { get; set; }
    [DataMember]
    public string DataValue { get; set; }
    [DataMember]
    public string Description { get; set; }
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
