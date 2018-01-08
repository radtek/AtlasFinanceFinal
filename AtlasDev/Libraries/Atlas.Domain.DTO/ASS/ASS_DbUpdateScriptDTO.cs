/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   DTO for the 'ASS_DbUpdateScript' domain entity
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
  public class ASS_DbUpdateScriptDTO
  {  
    public Int64 _DbUpdateScriptId { get; set; }  
    public string DbVersion { get; set; }  
    public ASS_DbUpdateScriptDTO PreviousVersion { get; set; }   
    public string UpdateScript { get; set; }  
    public string Description { get; set; }   
    public DateTime CreatedDT { get; set; }
  }
}
