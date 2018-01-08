using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
    public class TSPReportTransactionUploaded : XPCustomObject
    {
        public struct ReportTransactionUploadedKey
        {
            [Persistent("TransactionId")]
            public Int64 TransactionId;

            [Persistent("TransactionType"), Size(20)]
            public string TransactionType;

            [Persistent("ProcessMerchant"), Size(20)]
            public string ProcessMerchant;

            [Persistent("ClientRef1"), Size(20)]
            public string ClientRef1;

            [Persistent("ClientRef2"), Size(20)]
            public string ClientRef2;

            [Persistent("ActionDT")]
            public DateTime ActionDT;
        }

        public TSPReportTransactionUploaded() : base() { }
        public TSPReportTransactionUploaded(Session session) : base(session) { }

        [Key, Persistent]
        public ReportTransactionUploadedKey ReportTransactionUploaded;

        private TSPReportBatch _TSPReportBatch;
        [Persistent]
        public TSPReportBatch ReportBatch
        {
            get
            {
                return _TSPReportBatch;
            }
            set
            {
                SetPropertyValue("ReportBatch", ref _TSPReportBatch, value);
            }
        }

        private DateTime _replyDT;
        [Persistent]
        public DateTime ReplyDT
        {
            get
            {
                return _replyDT;
            }
            set
            {
                SetPropertyValue("ReplyDate", ref _replyDT, value);
            }
        }

        private string _accountName;
        [Persistent, Size(20)]
        public string AccountName
        {
            get
            {
                return _accountName;
            }
            set
            {
                SetPropertyValue("AccountName", ref _accountName, value);
            }
        }

        private decimal _value;
        [Persistent]
        public decimal Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetPropertyValue("Value", ref _value, value);
            }
        }


        private string _branchCode;
        [Persistent, Size(20)]
        public string BranchCode
        {
            get
            {
                return _branchCode;
            }
            set
            {
                SetPropertyValue("BranchCode", ref _branchCode, value);
            }
        }

        private string _rCode;
        [Persistent, Size(10)]
        public string RCode
        {
            get
            {
                return _rCode;
            }
            set
            {
                SetPropertyValue("RCode", ref _rCode, value);
            }
        }

        private string _qCode;
        [Persistent, Size(10)]
        public string QCode
        {
            get
            {
                return _qCode;
            }
            set
            {
                SetPropertyValue("QCode", ref _qCode, value);
            }
        }

        private string _accountNum;
        [Persistent, Size(20)]
        public string AccountNum
        {
            get
            {
                return _accountNum;
            }
            set
            {
                SetPropertyValue("AccountNum", ref _accountNum, value);
            }
        }
    }
}
