/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   DTO for the 'ASS_BranchServer' domain entity
 *   
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-06-12  Created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;


namespace Atlas.Domain.DTO
{
  /// <summary>
  /// DTO class for domain object 'ASS_BranchServer'
  /// </summary> 

  public class ASS_BranchServerDTO
  {
    public Int64 BranchServerId { get; set; }
    public BRN_BranchDTO Branch { get; set; }
    public COR_MachineDTO Machine { get; set; }
    public bool MachineAuthorised { get; set; }
    public DateTime LastSyncDT { get; set; }
    public ASS_DbUpdateScriptDTO RunningDBVersion { get; set; }
    public ASS_DbUpdateScriptDTO UseDBVersion { get; set; }
    private string LastError { get; set; }
    public Int64 LastProcessedClientRecId { get; set; }
  }
}
