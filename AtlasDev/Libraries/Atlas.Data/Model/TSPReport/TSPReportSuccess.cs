using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Domain.Interface;


namespace Atlas.Domain.Model
{
    public class TSPReportSuccess : XPCustomObject
    {
        public struct ReportSuccessKey
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

        public TSPReportSuccess() : base() { }
        public TSPReportSuccess(Session session) : base(session) { }

        [Key, Persistent]
        public ReportSuccessKey ReportSuccess;

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

        private DateTime _resultDT;
        [Persistent]
        public DateTime ResultDT
        {
            get
            {
                return _resultDT;
            }
            set
            {
                SetPropertyValue("ResultDT", ref _resultDT, value);
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
    }
}