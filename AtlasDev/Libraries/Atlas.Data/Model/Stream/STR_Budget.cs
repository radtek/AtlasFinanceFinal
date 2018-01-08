using System;

using Atlas.Common.Extensions;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_Budget : XPLiteObject
  {
    private Int32  _budgetId;
    [Key(AutoGenerate = true)]
    public Int32 BudgetId
    {
      get
      {
        return _budgetId;
      }
      set
      {
        SetPropertyValue("BudgetId", ref _budgetId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Stream.Budget BudgetType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.Budget>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.Budget>();
      }
    }

    private string _description;
    [Persistent]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
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

    private long _maxValue;
    [Persistent]
    public long MaxValue
    {
      get
      {
        return _maxValue;
      }
      set
      {
        SetPropertyValue("MaxValue", ref _maxValue, value);
      }
    }

    private long _currentValue;
    [Persistent]
    public long CurrentValue
    {
      get
      {
        return _currentValue;
      }
      set
      {
        SetPropertyValue("CurrentValue", ref _currentValue, value);
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

    public STR_Budget() : base() { }
    public STR_Budget(Session session) : base(session) { }

    #endregion

  }
}
