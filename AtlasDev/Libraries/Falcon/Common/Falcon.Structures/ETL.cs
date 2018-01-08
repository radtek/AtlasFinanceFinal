using System;

namespace Falcon.Common.Structures
{
  public sealed class ETL
  {
    public long BatchId { get; set; }
    public string Stage { get; set; }
    public DateTime LastStageDate { get; set; }
    //public List<ETLDebitOrder> DebitOrders { get; set; }
    public int SourceRequest { get; set; }
  }
}