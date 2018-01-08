/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2013 Atlas Finance (Pty() Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    Source request details
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 
 * 
 * 
 *  Notes:
 *  ------------------
  *     
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Runtime.Serialization;

#endregion


namespace Atlas.NuCard.WCF.Interface
{
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/AtlasServer.WCF.Interface")]
  public class SourceRequest
  {
    [DataMember]
    public string BranchCode { get; set; }

    [DataMember]
    public DateTime MachineDateTime { get; set; }

    [DataMember]
    public string MachineIPAddresses { get; set; }

    [DataMember]
    public string MachineName { get; set; }

    [DataMember]
    public string MachineUniqueID { get; set; }

    [DataMember]
    public string UserIDOrPassport { get; set; }

    [DataMember]
    public string AdminIDOrPassport { get; set; }

    [DataMember]
    public string AppName { get; set; }

    [DataMember]
    public string AppVer { get; set; }

    [DataMember]
    // Any additional comms info (Crypt/hash type used)
    public string Options { get; set; }

    [DataMember]
    public string FingerprintTemplate { get; set; }

    [DataMember]
    public int FingerprintFinger { get; set; }

    [DataMember]
    // This is the SALTing value
    public string CommOptions1 { get; set; }

    [DataMember]
    // This is the encrypted key used
    public string CommOptions2 { get; set; }
  }
}
