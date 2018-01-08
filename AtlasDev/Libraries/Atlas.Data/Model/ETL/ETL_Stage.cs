using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Model
{
  public class ETL_Stage : XPLiteObject
  {
    private int _stageId;
    [Key(AutoGenerate = false)]
    public int StageId
    {
      get
      {
        return _stageId;
      }
      set
      {
        SetPropertyValue("StageId", ref _stageId, value);
      }
    }

    [NonPersistent]
    public Enumerators.ETL.Stage Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.ETL.Stage>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.ETL.Stage>();
      }
    }

    private string _description;
    [Persistent, Size(100)]
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
    public ETL_Stage() : base() { }
    public ETL_Stage(Session session) : base(session) { }
  }
}
