
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for CardStatus
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
using Atlas.Common.Extensions;


namespace Atlas.Domain.DTO.Nucard
{
  [Serializable]
  [DataContract]
  public class NUC_NuCardStatusDTO
  {
    [DataMember]
    public Int64 NuCardStatusId { get; set; }
    [DataMember]
    public Enumerators.NuCard.NuCardStatus Type
    {
      get { return Description.FromStringToEnum<Enumerators.NuCard.NuCardStatus>(); }
      set { value = Description.FromStringToEnum<Enumerators.NuCard.NuCardStatus>(); }
    }
    [DataMember]
    public string Description { get;set;}
  }
}
