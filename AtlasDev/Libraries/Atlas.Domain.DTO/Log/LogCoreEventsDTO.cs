/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for LogCoreEvents
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

namespace Atlas.Domain.DTO
{
  public class LogCoreEventsDTO
  {
    public Int64 LogCoreEventId { get; set; }
    public DateTime EventDT { get; set; }
    public string Thread { get; set; }
    public string Level { get; set; }
    public Enumerators.General.ApplicationIdentifiers AppId { get; set; }
    public string Logger { get; set; }
    public string Message { get; set; }
    public string Exception { get; set; }
    public PER_PersonDTO CreatedBy { get; set; }
    public PER_PersonDTO DeletedBy { get; set; }
    public PER_PersonDTO LastEditedBy { get; set; }
    public DateTime? CreatedDT { get; set; }
    public DateTime? DeletedDT { get; set; }
    public DateTime? LastEditedDT { get; set; }
  }
}
