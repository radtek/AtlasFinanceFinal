using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class LGR_FeeRangeType : XPLiteObject
  {
    private Int64 _feeRangeTypeId;
    [Key]
    public Int64 FeeRangeTypeId
    {
      get
      {
        return _feeRangeTypeId;
      }
      set
      {
        SetPropertyValue("FeeRangeTypeId", ref _feeRangeTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.GeneralLedger.FeeRangeType Type
    {
      get { return Description.FromStringToEnum<Enumerators.GeneralLedger.FeeRangeType>(); }
      set { value = Description.FromStringToEnum<Enumerators.GeneralLedger.FeeRangeType>(); }
    }

    private string _description;
    [Persistent, Size(20)]
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
    
    #region Constructors

    public LGR_FeeRangeType() : base() { }
    public LGR_FeeRangeType(Session session) : base(session) { }

    #endregion
  }
}
