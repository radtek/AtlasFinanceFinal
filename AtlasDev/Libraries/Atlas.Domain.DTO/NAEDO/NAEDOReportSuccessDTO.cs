using System;

namespace Atlas.Domain.DTO
{
    public class NAEDOReportSuccessDTO
    {
        public struct ReportSuccessKey
        {
            public Int64 TransactionId { get; set; }
            public int TransactionTypeId { get; set; }
            public string ProcessMerchant { get; set; }
            public string ClientRef1 { get; set; }
            public string ClientRef2 { get; set; }
            public DateTime ActionDT { get; set; }
        }
        public ReportSuccessKey ReportSuccess;
        public DateTime? ReplyDT { get; set; }
        public string NumInstallments { get; set; }
        public string HomingAccountName { get; set; }
        public decimal Amount { get; set; }
        public string CCardNum { get; set; }
    }
}
