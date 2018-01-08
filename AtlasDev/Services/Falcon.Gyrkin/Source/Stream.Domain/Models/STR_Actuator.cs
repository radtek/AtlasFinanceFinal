using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_Actuator : XPLiteObject
  {
    private Int32 _actuatorId;
    [Key(AutoGenerate = true)]
    public Int32 ActuatorId
    {
      get
      {
        return _actuatorId;
      }
      set
      {
        SetPropertyValue("ActuatorId", ref _actuatorId, value);
      }
    }

    private STR_ActuatorType _actuatorType;
    [Persistent("ActuatorTypeId")]
    public STR_ActuatorType ActuatorType
    {
      get
      {
        return _actuatorType;
      }
      set
      {
        SetPropertyValue("ActuatorType", ref _actuatorType, value);
      }
    }

    private BRN_Branch _branch;
    [Persistent("BranchId")]
    public BRN_Branch Branch
    {
      get
      {
        return _branch;
      }
      set
      {
        SetPropertyValue("Branch", ref _branch, value);
      }
    }

    private Region _region;
    [Persistent("RegionId")]
    public Region Region
    {
      get
      {
        return _region;
      }
      set
      {
        SetPropertyValue("Region", ref _region, value);
      }
    }

    private DateTime _rangeStart;
    [Persistent]
    public DateTime RangeStart
    {
      get
      {
        return _rangeStart;
      }
      set
      {
        SetPropertyValue("RangeStart", ref _rangeStart, value);
      }
    }

    private DateTime _rangeEnd;
    [Persistent]
    public DateTime RangeEnd
    {
      get
      {
        return _rangeEnd;
      }
      set
      {
        SetPropertyValue("RangeEnd", ref _rangeEnd, value);
      }
    }

    private bool _isActive;
    [Persistent]
    public bool IsActive
    {
      get
      {
        return _isActive;
      }
      set
      {
        SetPropertyValue("IsActive", ref _isActive, value);
      }
    }

    private DateTime? _disableDate;
    [Persistent]
    public DateTime? DisableDate
    {
      get
      {
        return _disableDate;
      }
      set
      {
        SetPropertyValue("DisableDate", ref _disableDate, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
    public DateTime CreateDate
    {
      get
      {
        return _createDate;
      }
      set
      {
        SetPropertyValue("CreateDate", ref _createDate, value);
      }
    }


    #region Constructors

    public STR_Actuator()
    { }
    public STR_Actuator(Session session) : base(session) { }

    #endregion

  }
}
