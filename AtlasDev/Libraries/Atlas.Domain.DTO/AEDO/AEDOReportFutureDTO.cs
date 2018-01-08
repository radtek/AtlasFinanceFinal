using System;

namespace Atlas.Domain.DTO
{
    public class AEDOReportFutureDTO
    {
        public struct ReportFutureKey
        {
            public Int64 TransactionId { get; set; }
            public string ContractRef { get; set; }
            public string ServiceType { get; set; }
            public DateTime ValueDT { get; set; }
            public DateTime SubmitDT { get; set; }
        }

        public ReportFutureKey ReportFuture;
        public DateTime StartDT { get; set; }
        public DateTime? LastSubmissionDT { get; set; }
        public string SubmitCount { get; set; }
        public string RetryReason { get; set; }
        public string ContractNum { get; set; }
        public decimal ContractAmount { get; set; }
        public int Term { get; set; }
        public int Instalments { get; set; }
        public int InstalmentNum { get; set; }
        public decimal InstalmentAmount { get; set; }
        public string EmployerCode { get; set; }
        public int Frequency { get; set; }
        public int DateAdjustRule { get; set; }
        public string TrackingIndicator { get; set; }
        public string Pan { get; set; }
        public int TerminalNum { get; set; }
        public bool Active { get; set; }
        public string CardAcceptor { get; set; }
        public string InstitutionId { get; set; }
        public string IdNumber { get; set; }
    }
}
