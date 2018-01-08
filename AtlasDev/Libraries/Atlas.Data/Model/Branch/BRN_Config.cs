/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Config
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

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  [Indices("DataType")]
  public class BRN_Config : XPCustomObject
  {
    private Int64 _BranchConfigId;
    [Key(AutoGenerate = true)]
    public Int64 BranchConfigId
    {
      get
      {
        return _BranchConfigId;
      }
      set
      {
        SetPropertyValue("BranchConfigId", ref _BranchConfigId, value);
      }
    }


    private BRN_Branch _Branch;
    [Persistent("BranchId")]
    public BRN_Branch Branch
    {
      get
      {
        return _Branch;
      }
      set
      {
        SetPropertyValue("BranchId", ref _Branch, value);
      }
    }


    private Enumerators.General.BranchConfigDataType _DataType;
    [Persistent]
    [Indexed(Name = "IDX_DATATYPE")]
    public Enumerators.General.BranchConfigDataType DataType
    {
      get
      {
        return _DataType;
      }
      set
      {
        SetPropertyValue("DataType", ref _DataType, value);
      }
    }

    private string _DataSection;
    [Persistent]
    public string DataSection
    {
      get
      {
        return _DataSection;
      }
      set
      {
        SetPropertyValue("DataSection", ref _DataSection, value);
      }
    }

    private string _DataValue;

    [Persistent, Size(int.MaxValue)]
    public string DataValue
    {
      get
      {
        return _DataValue;
      }
      set
      {
        SetPropertyValue("DataValue", ref _DataValue, value);
      }
    }

    private string _Description;
    [Persistent]
    public string Description
    {
      get
      {
        return _Description;
      }
      set
      {
        SetPropertyValue("Description", ref _Description, value);
      }
    }

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _CreatedBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _CreatedBy, value);
      }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get
      {
        return _CreatedDT;
      }
      set
      {
        SetPropertyValue("CreatedDT", ref _CreatedDT, value);
      }
    }   

    #region Constructors

    public BRN_Config() : base() { }
    public BRN_Config(Session session) : base(session) { }

    #endregion
  }
}
