/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     WCF source request
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     24 May 2013 - Created
 * 
 * 
 *  Comments:
 *  ------------------
 *     
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Runtime.Serialization;


namespace Atlas.DataSync.WCF.Interface
{
  [DataContract(Name = "SourceRequest", Namespace = "http://schemas.datacontract.org/2004/07/ASSServer.WCF.Interface")]
  [Serializable]
  public class SourceRequest
  {
    #region Public constructor

    /// <summary>
    /// Empty constructor required for automatic serialization
    /// </summary>
    public SourceRequest()
    {
    }

    #endregion


    #region Public properties

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
    public string AppName { get; set; }
    [DataMember]
    public string AppVer { get; set; }
    [DataMember]
    public Int64 PersonId { get; set; }

    #endregion
    
  }

}