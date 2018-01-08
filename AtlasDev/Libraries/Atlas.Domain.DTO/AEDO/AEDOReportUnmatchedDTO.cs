using System;

namespace Atlas.Domain.DTO
{
    public class AEDOReportUnmatchedDTO
    {
        public DateTime SettlementDT { get; set; }
        public int TermId { get; set; }
        public string MerchantNum { get; set; }
        public string Pan { get; set; }
        public decimal TransactionAmount { get; set; }
        public string ContractRef { get; set; }
    }
}
