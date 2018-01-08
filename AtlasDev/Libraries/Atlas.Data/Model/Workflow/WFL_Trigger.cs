using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_Trigger : XPLiteObject
  {
    private int _triggerId;
    [Key]
    public int TriggerId
    {
      get
      {
        return _triggerId;
      }
      set
      {
        SetPropertyValue("TriggerId", ref _triggerId, value);
      }
    }
    
    [NonPersistent]
    public Enumerators.Workflow.Trigger Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.Trigger>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.Trigger>();
      }
    } 

    private string _name;
    [Persistent, Size(10)]
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        SetPropertyValue("Name", ref _name, value);
      }
    }

    #region Constructors

    public WFL_Trigger() : base() { }
    public WFL_Trigger(Session session) : base(session) { }

    #endregion

  }
}
