using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  public class STR_Stream : XPLiteObject
  {
    private int _streamId;
    [Key]
    public int StreamId
    {
      get
      {
        return _streamId;
      }
      set
      {
        SetPropertyValue("StreamId", ref _streamId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Stream.StreamType StreamType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.StreamType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.StreamType>();
      }
    }

    private string _description;
    [Persistent, Size(40)]
    [Indexed]
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

    private STR_Priority _priority;
    [Persistent("PriorityId")]
    [Indexed]
    public STR_Priority Priority
    {
      get
      {
        return _priority;
      }
      set
      {
        SetPropertyValue("Priority", ref _priority, value);
      }
    }

    private STR_Priority _defaultCaseStreamPriority;
    [Persistent("DefaultCaseStreamPriorityId")]
    public STR_Priority DefaultCaseStreamPriority
    {
      get
      {
        return _defaultCaseStreamPriority;
      }
      set
      {
        SetPropertyValue("DefaultCaseStreamPriority", ref _defaultCaseStreamPriority, value);
      }
    }


    #region Constructors

    public STR_Stream() : base() { }
    public STR_Stream(Session session) : base(session) { }

    #endregion

  }
}