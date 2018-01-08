using System;

namespace Atlas.Domain.DTO
{
    public class AEDOReportUnsettledDTO
    {
        public struct ReportUnsettledKey
        {
            public Int64 TransactionId { get; set; }
            public DateTime ValueDT { get; set; }
        }

        public ReportUnsettledKey ReportUnsettled;
        public int TermId { get; set; }
        public string MerchantNum { get; set; }
        public string Pan { get; set; }
        public int Instalment { get; set; }
        public decimal ContractAmount { get; set; }
        public DateTime ActualDT { get; set; }
        public string ContractNum { get; set; }
        public DateTime StartDT { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public int Frequency { get; set; }
        public string EmployerCode { get; set; }
        public string ContractRef { get; set; }
        public string IdNumber { get; set; }
    }
}
