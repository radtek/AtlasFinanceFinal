using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_EscalationLevel : XPLiteObject
  {
    private int _escalationLevelId;
    [Key]
    public int EscalationLevelId
    {
      get
      {
        return _escalationLevelId;
      }
      set
      {
        SetPropertyValue("EscalationLevelId", ref _escalationLevelId, value);
      }
    }
    
    [NonPersistent]
    public Enumerators.Workflow.EscalationLevel Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.EscalationLevel>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.EscalationLevel>();
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

    public WFL_EscalationLevel() : base() { }
    public WFL_EscalationLevel(Session session) : base(session) { }

    #endregion
  }
}
