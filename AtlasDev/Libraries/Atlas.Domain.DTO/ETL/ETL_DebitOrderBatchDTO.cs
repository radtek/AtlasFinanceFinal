using System;

namespace Atlas.Domain.DTO
{
  public class ETL_DebitOrderBatchDTO
  {
    public long DebitOrderBatchId { get; set; }
    public HostDTO Host { get; set; }
    public CPY_CompanyDTO Company { get; set; }
    public ETL_StageDTO Stage { get; set; }
    public DateTime LastStageDate { get; set; }
    public string File { get; set; }
    public DateTime CreateDate { get; set; }
  }
}