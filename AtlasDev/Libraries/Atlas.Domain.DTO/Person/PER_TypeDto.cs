
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Person Type
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


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PER_TypeDTO
  {
    [DataMember]
    public Int64 TypeId { get; set; }
    [DataMember]
    public Enumerators.General.PersonType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.PersonType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.PersonType>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}