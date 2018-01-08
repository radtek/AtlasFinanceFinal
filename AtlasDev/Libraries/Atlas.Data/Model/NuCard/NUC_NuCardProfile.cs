/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-213 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    NuCard profile information
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
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Domain.Interface;


namespace Atlas.Domain.Model
{
  public class NUC_NuCardProfile : XPLiteObject
  {
    private Int64 _NuCardProfileId;
    [Key(AutoGenerate = true)]
    public Int64 NuCardProfileId
    {
      get { return _NuCardProfileId; }
      set { SetPropertyValue("NuCardProfileId", ref _NuCardProfileId, value); }
    }

    private string _ProfileNum;
    [Persistent, Size(20)]
    public string ProfileNum
    {
      get { return _ProfileNum; }
      set { SetPropertyValue("ProfileNum", ref _ProfileNum, value); }
    }

    private string _TerminalId;
    [Persistent, Size(20)]
    public string TerminalId
    {
      get { return _TerminalId; }
      set { SetPropertyValue("TerminalId", ref _TerminalId, value); }
    }

    private string _Password;
    [Persistent, Size(20)]
    public string Password
    {
      get { return _Password; }
      set { SetPropertyValue("Password", ref _Password, value); }
    }

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get { return _CreatedBy; }
      set { SetPropertyValue("CreatedBy", ref _CreatedBy, value); }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue("CreatedDT", ref _CreatedDT, value); }
    }


    #region Constructors

    public NUC_NuCardProfile() : base() { }
    public NUC_NuCardProfile(Session session) : base(session) { }

    #endregion
  }
}
