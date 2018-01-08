using System;

namespace Atlas.Domain.DTO
{
    public class AEDOReportCancelledDTO
    {
        public struct ReportCancelledKey
        {
            public Int64 TransactionId;
            public string ContractRef;
            public string ServiceType;
            public DateTime ValueDT;
            public DateTime CancelDT;
        }

        public ReportCancelledKey ReportCancelled;
        public string TrackingIndicator { get; set; }
        public string ContractNum { get; set; }
        public string CancellationType { get; set; }
        public string CancelMerchant { get; set; }
        public string EmployerCode { get; set; }
        public string Pan { get; set; }
        public int TerminalNum { get; set; }
        public string InstitutionId { get; set; }
        public string IdNumber { get; set; }
    }
}
