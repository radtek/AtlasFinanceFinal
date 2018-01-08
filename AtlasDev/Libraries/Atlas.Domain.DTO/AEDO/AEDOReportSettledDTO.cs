using System;

namespace Atlas.Domain.DTO
{
    public class AEDOReportSettledDTO
    {
        public struct ReportSettledKey
        {
            public Int64 TransactionId { get; set; }
            public DateTime SettlementDT { get; set; }
            public int Instalment { get; set; }
        }
        public ReportSettledKey ReportSettled;
        public int TermId { get; set; }
        public string MerchantNum { get; set; }
        public string Pan { get; set; }
        public DateTime TransmitDT { get; set; }
        public string ContractNum { get; set; }
        public decimal TransactionAmount { get; set; }
        public string ContractRef { get; set; }
        public string IdNumber { get; set; }
    }
}
