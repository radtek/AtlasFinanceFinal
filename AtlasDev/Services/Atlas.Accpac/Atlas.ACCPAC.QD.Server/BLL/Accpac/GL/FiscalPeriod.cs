using System;


namespace BLL.Accpac.GL
{
  public class FiscalPeriod
  {
    public FiscalPeriod()
    {
    }
  

    public string FiscalYear { get; set; }
    public Int16 Period { get; set; }
    public decimal BeginDate { get; set; }
    public decimal EndDate { get; set; }

  }
}


