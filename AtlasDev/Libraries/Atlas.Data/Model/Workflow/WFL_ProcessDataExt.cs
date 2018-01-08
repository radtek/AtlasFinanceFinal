using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessDataExt : XPLiteObject
  {
    private int _processDataExtId;
    [Key(AutoGenerate = true)]
    public int ProcessDataExtId
    {
      get
      {
        return _processDataExtId;
      }
      set
      {
        SetPropertyValue("ProcessDataExtId", ref _processDataExtId, value);
      }
    }

    private WFL_DataExtType _dataExtType;
    [Persistent("DataExtTypeId")]
    public WFL_DataExtType DataExtType
    {
      get
      {
        return _dataExtType;
      }
      set
      {
        SetPropertyValue("DataExtType", ref _dataExtType, value);
      }
    }

    private WFL_ProcessJob _processJob;
    [Persistent("ProcessJobId")]
    public WFL_ProcessJob ProcessJob
    {
      get
      {
        return _processJob;
      }
      set
      {
        SetPropertyValue("ProcessJob", ref _processJob, value);
      }
    }

    private string _data;
    [Persistent, Size(500)]
    public string Data
    {
      get
      {
        return _data;
      }
      set
      {
        SetPropertyValue("Data", ref _data, value);
      }
    }

    #region Constructors

    public WFL_ProcessDataExt() : base() { }
    public WFL_ProcessDataExt(Session session) : base(session) { }

    #endregion
  }
}
