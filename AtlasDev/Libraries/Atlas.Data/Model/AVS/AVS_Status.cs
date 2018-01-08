using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_Status : XPLiteObject
  {
    private Int32 _statusId;
    [Key]
    public Int32 StatusId
    {
      get
      {
        return _statusId;
      }
      set
      {
        SetPropertyValue("StatusId", ref _statusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.AVS.Status Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.Status>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.Status>();
      }
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

    public AVS_Status() : base() { }
    public AVS_Status(Session session) : base(session) { }

    #endregion
  }
}
