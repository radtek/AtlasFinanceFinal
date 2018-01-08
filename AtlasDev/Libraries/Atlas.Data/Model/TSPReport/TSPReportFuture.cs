using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
    public class TSPReportFuture : XPCustomObject
    {
        public struct ReportFutureKey
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

            [Persistent("ActionDate")]
            public DateTime ActionDate;
        }

        public TSPReportFuture() : base() { }
        public TSPReportFuture(Session session) : base(session) { }

        [Key, Persistent]
        public ReportFutureKey ReportFuture;

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

        private string _numInstallments;
        [Persistent, Size(20)]
        public string NumInstallments
        {
            get
            {
                return _numInstallments;
            }
            set
            {
                SetPropertyValue("NumInstallments", ref _numInstallments, value);
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
