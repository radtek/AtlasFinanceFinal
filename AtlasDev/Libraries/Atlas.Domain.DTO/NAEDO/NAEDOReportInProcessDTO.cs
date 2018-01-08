using System;

namespace Atlas.Domain.DTO
{
    public class NAEDOReportInProcessDTO
    {
        public struct ReportInProcessKey
        {
            public Int64 TransactionId { get; set; }
            public int TransactionTypeId { get; set; }
            public string ProcessMerchant { get; set; }
            public string ClientRef1 { get; set; }
            public string ClientRef2 { get; set; }
            public DateTime ActionDT { get; set; }
        }
        public ReportInProcessKey ReportInProcess;
        public string NumInstallments { get; set; }
        public string HomingAccountName { get; set; }
        public decimal Amount { get; set; }
        public string CCardNum { get; set; }
        public string Tracking { get; set; }
        public DateTime TrackDT { get; set; }
        public string InstStatus { get; set; }
    }
}
