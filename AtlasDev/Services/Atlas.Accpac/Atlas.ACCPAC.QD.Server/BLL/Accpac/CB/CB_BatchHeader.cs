using System;
using System.Data;
using System.Configuration;
using System.Web;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace BLL.Accpac.CB
{
    public class CB_BatchHeader
    {

        #region Enum's

        #endregion

        #region Struc's


        #endregion

        #region Constructor

            public CB_BatchHeader()
            {
                myCB_BatchDetails = new List<CB_BatchDetail>();
            }

            public CB_BatchHeader(int internalBatchID, string branchCode, string tranDate, string cb_Reference, string cb_Description, string fiscalYear, string fiscalPeriod, int noOfDetails, decimal totAmt, string batchID, string entryNo, Int16 processedInd, DateTime processedDate, string error)
            {
                _internalBatchID = internalBatchID;
                _branchCode = branchCode.Trim();
                _tranDate = tranDate.Trim();
                _cb_Reference = cb_Reference.Trim();
                _cb_Description = cb_Description.Trim();
                _fiscalYear = fiscalYear.Trim();
                _fiscalPeriod = fiscalPeriod.Trim();
                _noOfDetails = noOfDetails;
                _totAmt = totAmt;
                _batchID = batchID.Trim();
                _entryNo = entryNo.Trim();
                _processedInd = processedInd;
                _processedDate = processedDate;
                _error = error.Trim();

                myCB_BatchDetails = new List<CB_BatchDetail>();
                PopulateClass();
            }
           
            private void PopulateClass()
            {
                BusinessLogicLayer.DAL.SQL sql = new BusinessLogicLayer.DAL.SQL();
                myCB_BatchDetails = sql.Get_Accpac_CB_GetCB_BatchDetail(this.InternalBatchID);
            }

        #endregion

        #region Member variables

        private int _internalBatchID = 0;
        private string _branchCode = "";
        private string _tranDate = "";
        private string _cb_Reference = "";
        private string _cb_Description = "";
        private string _fiscalYear = "";
        private string _fiscalPeriod = "";
        private int _noOfDetails = 0;
        private decimal _totAmt = 0;
        private string _batchID = "";
        private string _entryNo = "";
        private Int16 _processedInd = 0;
        private DateTime _processedDate;
        private string _error = "";
                 
        // Children
        public List<BLL.Accpac.CB.CB_BatchDetail> myCB_BatchDetails;

        #endregion

        #region Public Properties

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

            public string CB_Reference  
            {
                get
                {
                    return _cb_Reference;
                }
                set
                {
                    _cb_Reference = value;
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

            public string FiscalYear  
            {
                get
                {
                    return _fiscalYear ;
                }
                set
                {
                    _fiscalYear  = value;
                }
            }

            public string FiscalPeriod  
            {
                get
                {
                    return _fiscalPeriod ;
                }
                set
                {
                    _fiscalPeriod  = value;
                }
            }

            public int NoOfDetails  
            {
                get
                {
                    return _noOfDetails ;
                }
                set
                {
                    _noOfDetails  = value;
                }
            }

            public decimal TotalAmt 
            {
                get
                {
                    return _totAmt  ;
                }
                set
                {
                    _totAmt   = value;
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


