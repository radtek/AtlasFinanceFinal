using System;

namespace Atlas.Domain.DTO
{
    public class AEDOReportFailedDTO
    {
        public struct ReportFailedKey
        {
            public Int64 TransactionId { get; set; }
            public string ContractRef { get; set; }
            public string ServiceType { get; set; }
            public DateTime ValueDT { get; set; }
            public DateTime FailDT { get; set; }
        }
        public ReportFailedKey ReportFailed;
        public DateTime StartDT { get; set; }
        public string ContractNum { get; set; }
        public string Reason { get; set; }
        public decimal ContractAmount { get; set; }
        public int InstalmentNum { get; set; }
        public decimal InstalmentAmount {get;set;}
        public string EmployerCode { get; set; }
        public string TrackingIndicator { get; set; }
        public int Frequency { get; set; }
        public string Pan { get; set; }
        public int TerminalNum { get; set; }
        public bool Resubmit { get; set; }
        public string CardAcceptor { get; set; }
        public string InstitutionId { get; set; }
        public string IdNumber { get; set; }
    }
}
