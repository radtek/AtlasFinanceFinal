using System;

namespace Atlas.Domain.DTO
{
    public class TSPReportBatchDTO
    {
        public Int64 BatchId { get; set; }
        public string MerchantNumber { get; set; }
        public string ServiceType { get; set; }
        public int ReportType { get; set; }
        public DateTime ReportFromDT { get; set; }
        public DateTime ReportToDT { get; set; }
        public Int64 TokenNumber { get; set; }
        public Int64 BlockNumber { get; set; }
        public DateTime ReportGenerationDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
