using System;

using System.Collections.Generic;


namespace BLL.LookUps
{
  public class LookUpMain
  {
    #region Enum's

    public enum LookUpType
    {
      Test1 = 1,
      Test2 = 2
    }

    #endregion

    #region Struc's


    #endregion

    #region Constructor

    public LookUpMain()
    {

    }

    public LookUpMain(Int32 id, string lookUpName, string val1, string val2, string val3, string val4, string val5, string val6, string val7)
    {
      ID = id;
      _val1 = val1.Trim();
      _val2 = val2.Trim();
      _val3 = val3.Trim();
      _val4 = val4.Trim();
      _val5 = val5.Trim();
      _val6 = val6.Trim();
      _val7 = val7.Trim();
      _lookUpName = lookUpName.Trim();

      PopulateClass();
    }

    private void PopulateClass()
    {

    }

    #endregion

    #region Member variables
    private string _val1 = "";
    private string _val2 = "";
    private string _val3 = "";
    private string _val4 = "";
    private string _val5 = "";
    private string _val6 = "";
    private string _val7 = "";
    private string _lookUpName = "";
    // Children


    #endregion

    #region Public Properties

    public Int32 ID { get; set; }

    public string LookUpName
    {
      get
      {
        return _lookUpName;
      }
      set
      {
        _lookUpName = value;
      }
    }

    public string Val1
    {
      get
      {
        return _val1;
      }
      set
      {
        _val1 = value;
      }
    }

    public string Val2
    {
      get
      {
        return _val2;
      }
      set
      {
        _val2 = value;
      }
    }

    public string Val3
    {
      get
      {
        return _val3;
      }
      set
      {
        _val3 = value;
      }
    }

    public string Val4
    {
      get
      {
        return _val4;
      }
      set
      {
        _val4 = value;
      }
    }

    public string Val5
    {
      get
      {
        return _val5;
      }
      set
      {
        _val5 = value;
      }
    }

    public string Val6
    {
      get
      {
        return _val6;
      }
      set
      {
        _val6 = value;
      }
    }

    public string Val7
    {
      get
      {
        return _val7;
      }
      set
      {
        _val7 = value;
      }
    }

    #endregion

  }

}

