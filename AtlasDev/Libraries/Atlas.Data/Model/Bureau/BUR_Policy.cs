namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;
  using Atlas.Common.Extensions;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public sealed class BUR_Policy : XPLiteObject
  {
    private int _policyId;
    [Key(AutoGenerate = false)]
    public int PolicyId
    {
      get { return _policyId; }
      set
      {
        SetPropertyValue("PolicyId", ref _policyId, value);
      }
    }

    
    [NonPersistent]
    public Enumerators.Risk.Policy Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Risk.Policy>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Risk.Policy>();
      }
    }


    private string _Description;
    public string Description
    {
      get { return _Description; }
      set
      {
        SetPropertyValue("Description", ref _Description, value);
      }
    }

    private bool _enabled;
    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    #region Constructors

    public BUR_Policy() : base() { }
    public BUR_Policy(Session session) : base(session) { }

    #endregion
  }
}
