using System;
using System.Data;
using System.Configuration;
using System.Web;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BLL.Accpac.CB
{
    public class CB_BatchDetail
    {

        #region Enum's

        #endregion

        #region Struc's


        #endregion

        #region Constructor

            public CB_BatchDetail()
            {

            }

            public CB_BatchDetail(int internalBatchDetailID, int internalBatchID, string branchCode, string tranDate, string transType, string cb_Description, string cb_Comment, string sourceCode, string acctID, string acctDesc, decimal dtAmt, decimal crAmt, string batchID, string entryNo, string detailNo, Int16 processedInd, DateTime processedDate, string error)
            {
                _internalBatchDetailID = internalBatchDetailID;
                _internalBatchID = internalBatchID;
                _branchCode = branchCode.Trim();
                _tranDate = tranDate.Trim();
                _transType = transType.Trim();
                _cb_Description = cb_Description.Trim();
                _cb_Comment = cb_Comment.Trim();
                _sourceCode = sourceCode.Trim();
                _acctID = acctID.Trim();
                _acctDesc = acctDesc.Trim();
                _dtAmt = dtAmt;
                _crAmt = crAmt;
                _batchID = batchID.Trim();
                _entryNo = entryNo.Trim();
                _detailNo = detailNo.Trim();
                _processedInd = processedInd;
                _processedDate = processedDate;
                _error = error.Trim();

                PopulateClass();
            }
           
            private void PopulateClass()
            {
            }

        #endregion

        #region Member variables

        private int _internalBatchDetailID = 0;
        private int _internalBatchID = 0;
        private string _branchCode = "";
        private string _tranDate = "";
        private string _transType = "";
        private string _cb_Description = "";
        private string _cb_Comment = "";
        private string _sourceCode = "";
        private string _acctID = "";
        private string _acctDesc = "";
        private decimal _dtAmt = 0;
        private decimal _crAmt = 0;
        private string _batchID = "";
        private string _entryNo = "";
        private string _detailNo = "";
        private Int16 _processedInd = 0;
        private DateTime _processedDate;
        private string _error = "";


        #endregion

        #region Public Properties

            public int InternalBatchDetailID   
            {
                get
                {
                    return _internalBatchDetailID ;
                }
                set
                {
                    _internalBatchDetailID  = value;
                }
            }

            public int InternalBatchID  
            {
                get
                {
                    return _internalBatchID ;
                }
                set
                {
                    _internalBatchID  = value;
                }
            }

            public string BranchCode  
            {
                get
                {
                    return _branchCode ;
                }
                set
                {
                    _branchCode  = value;
                }
            }

            public string TranDate  
            {
                get
                {
                    return _tranDate ;
                }
                set
                {
                    _tranDate  = value;
                }
            }

            public string TransType  
            {
                get
                {
                    return _transType ;
                }
                set
                {
                    _transType  = value;
                }
            }

            public string CB_Description  
            {
                get
                {
                    return _cb_Description ;
                }
                set
                {
                    _cb_Description  = value;
                }
            }

            public string CB_Comment  
            {
                get
                {
                    return _cb_Comment ;
                }
                set
                {
                    _cb_Comment  = value;
                }
            }

            public string SourceCode  
            {
                get
                {
                    return _sourceCode ;
                }
                set
                {
                    _sourceCode  = value;
                }
            }

            public string AcctID  
            {
                get
                {
                    return _acctID ;
                }
                set
                {
                    _acctID  = value;
                }
            }

            public string AcctDesc  
            {
                get
                {
                    return _acctDesc ;
                }
                set
                {
                    _acctDesc  = value;
                }
            }

            public decimal DtAmount 
            {
                get
                {
                    return _dtAmt ;
                }
                set
                {
                    _dtAmt  = value;
                }
            }

            public decimal CrAmount 
            {
                get
                {
                    return _crAmt  ;
                }
                set
                {
                    _crAmt   = value;
                }
            }


            public string BatchID   
            {
                get
                {
                    return _batchID  ;
                }
                set
                {
                    _batchID   = value;
                }
            }

            public string EntryNo   
            {
                get
                {
                    return _entryNo  ;
                }
                set
                {
                    _entryNo   = value;
                }
            }

            public string DetailNo   
            {
                get
                {
                    return _detailNo  ;
                }
                set
                {
                    _detailNo   = value;
                }
            }

        
            public Int16 ProcessedInd    
            {
                get
                {
                    return _processedInd   ;
                }
                set
                {
                    _processedInd    = value;
                }
            }

            public DateTime ProcessedDate    
            {
                get
                {
                    return _processedDate   ;
                }
                set
                {
                    _processedDate    = value;
                }
            }

            public string Error    
            {
                get
                {
                    return _error   ;
                }
                set
                {
                    _error    = value;
                }
            }


        #endregion

    }
}


