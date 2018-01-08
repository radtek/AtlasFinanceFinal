using System;

namespace Atlas.Domain.DTO
{
    public class TSPReportInProcessDTO
    {
        public Int64 TransactionId { get; set; }
        public string TransactionType { get; set; }
        public string ProcessMerchant { get; set; }
        public string ClientRef1 { get; set; }
        public string ClientRef2 { get; set; }
        public DateTime ActionDate { get; set; }
        public string NumInstallments { get; set; }
        public string AccountName { get; set; }
        public decimal Value { get; set; }
    }
}
