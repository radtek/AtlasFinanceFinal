
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2016 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Config
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
  [Indices("DataEntity;DataType")]
  public class Config : XPCustomObject
  {
    [Key(AutoGenerate = true)]
    public Int64 ConfigId { get; set; }
    
    private string _DataEntity;
    [Persistent]
    [Indexed(Name = "IDX_DATAENTITY")]
    public string DataEntity
    {
      get { return _DataEntity; }
      set { SetPropertyValue(nameof(DataEntity), ref _DataEntity, value); }
    }
    
    private int _DataType;
    [Persistent]
    [Indexed(Name = "IDX_DATATYPE1")]
    public int DataType
    {
      get { return _DataType; }
      set { SetPropertyValue(nameof(DataType), ref _DataType, value); }
    }

    private string _DataSection;
    [Persistent]
    public string DataSection
    {
      get { return _DataSection; }
      set { SetPropertyValue(nameof(DataSection), ref _DataSection, value); }
    }
    
    private string _DataValue;
    [Persistent, Size(int.MaxValue)]
    public string DataValue
    {
      get { return _DataValue; }
      set { SetPropertyValue(nameof(DataValue), ref _DataValue, value); }
    }
    
    private string _Description;
    [Persistent]
    public string Description
    {
      get { return _Description; }
      set { SetPropertyValue(nameof(Description), ref _Description, value); }
    }

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get { return _CreatedBy; }
      set { SetPropertyValue(nameof(CreatedBy), ref _CreatedBy, value); }
    }

    private PER_Person _DeletedBy;
    [Persistent]
    public PER_Person DeletedBy
    {
      get { return _DeletedBy; }
      set { SetPropertyValue(nameof(DeletedBy), ref _DeletedBy, value); }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue(nameof(CreatedDT), ref _CreatedDT, value); }
    }



    #region Constructors

    public Config() : base() { }
    public Config(Session session) : base(session) { }

    #endregion
  }
}
