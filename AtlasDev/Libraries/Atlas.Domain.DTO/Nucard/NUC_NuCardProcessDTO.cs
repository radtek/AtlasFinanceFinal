
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for NuCardProcessDTO
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
  public class NUC_NuCardProcessDTO
  {
    [DataMember]
    public Int64 NuCardProcessId { get; set; }
    [DataMember]
    public NUC_NuCardProcessDTO DependantNuCardProcess { get; set; }
    [DataMember]
    public NUC_NuCardDTO NuCard { get; set; }
    [DataMember]
    public PER_PersonDTO AssignedUser { get; set; }
    [DataMember]
    public NUC_TransactionDTO Transaction { get; set; }
    [DataMember]
    public bool IsApproved { get; set; }
    [DataMember]
    public bool IsDeclined { get; set; }
    [DataMember]
    public DateTime ApprovedDT { get; set; }
    [DataMember]
    public DateTime DeclinedDT { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public DateTime? CreatedDT { get; set; }

  }
}