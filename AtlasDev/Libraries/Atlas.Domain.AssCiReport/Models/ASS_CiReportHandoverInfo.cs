using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Atlas.Domain.Ass.Models
{
  public class ASS_CiReportHandoverInfo : XPLiteObject
  {
    private long _ciReportHandoverInfoId;

    [Key(AutoGenerate = true)]
    public long CiReportHandoverInfoId
    {
      get { return _ciReportHandoverInfoId; }
      set { SetPropertyValue("CiReportHandoverInfoId", ref _ciReportHandoverInfoId, value); }
    }

    private ASS_CiReportVersion _ciReportVersion;

    [Persistent("CiReportVersionId")]
    [Indexed]
    public ASS_CiReportVersion CiReportVersion
    {
      get { return _ciReportVersion; }
      set { SetPropertyValue("CiReportVersion", ref _ciReportVersion, value); }
    }

    private BRN_Branch _branch;

    [Persistent("BranchId")]
    [Indexed]
    public BRN_Branch Branch
    {
      get { return _branch; }
      set { SetPropertyValue("Branch", ref _branch, value); }
    }

    private DateTime _date;

    [Persistent]
    [Indexed]
    public DateTime Date
    {
      get { return _date; }
      set { SetPropertyValue("Date", ref _date, value); }
    }

    private int _payNo;

    [Persistent]
    public int PayNo
    {
      get { return _payNo; }
      set { SetPropertyValue("PayNo", ref _payNo, value); }
    }

    private int _handedOverLoansQuantity;

    [Persistent]
    public int HandedOverLoansQuantity
    {
      get { return _handedOverLoansQuantity; }
      set { SetPropertyValue("HandedOverLoansQuantity", ref _handedOverLoansQuantity, value); }
    }

    private int _handedOverClientQuantity;

    [Persistent]
    public int HandedOverClientQuantity
    {
      get { return _handedOverClientQuantity; }
      set { SetPropertyValue("HandedOverClientQuantity", ref _handedOverClientQuantity, value); }
    }

    private decimal _handedOverLoansAmount;

    [Persistent]
    public decimal HandedOverLoansAmount
    {
      get { return _handedOverLoansAmount; }
      set { SetPropertyValue("HandedOverLoansAmount", ref _handedOverLoansAmount, value); }
    }

    #region Constructors

    public ASS_CiReportHandoverInfo()
    {
    }

    public ASS_CiReportHandoverInfo(Session session)
      : base(session)
    {
    }

    #endregion
  }
}