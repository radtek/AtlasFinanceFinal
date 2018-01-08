using System;

namespace Atlas.Domain.DTO
{
    public class NAEDOReportTransactionUploadedDTO
    {
        public struct ReportTransactionUploadedKey
        {
            public Int64 TransactionId;
            public int TransactionTypeId;
            public string ProcessMerchant;
            public string ClientRef1;
            public string ClientRef2;
            public DateTime ActionDT;
        }
        public ReportTransactionUploadedKey ReportTransactionUploaded;
        public DateTime? ReplyDT { get; set; }
        public string HomingAccountName { get; set; }
        public string HomingAccountNum { get; set; }
        public decimal Amount { get; set; }
        public string HomingBranch { get; set; }
        public string RCode { get; set; }
        public string QCode { get; set; }
        public string CCardNum { get; set; }
    }
}
