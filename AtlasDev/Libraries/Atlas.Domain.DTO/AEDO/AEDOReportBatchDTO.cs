using System;

namespace Atlas.Domain.DTO
{
    public class AEDOReportBatchDTO
    {
        public Int64 BatchId { get; set; }
        public string MerchantNum { get; set; }
        public int ReportType { get; set; }
        public DateTime ReportFromDT { get; set; }
        public DateTime ReportToDT { get; set; }
        public Int64 TokenNum { get; set; }
        public Int64 BlockNum { get; set; }
        public DateTime? ReportGenerationDT { get; set; }
        public DateTime CreatedDT { get; set; }
        public int AEDOLoginId { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StartDT { get; set; }
        public DateTime EndDT { get; set; }
    }
}
