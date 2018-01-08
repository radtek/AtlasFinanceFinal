using System;

namespace Atlas.RabbitMQ.Messages.Coupon
{

  #region ICouponBaseMessage

  public interface ICouponBaseMessage
  {
    Guid CorrelationId { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string CellNo { get; set; }
    string IDNo { get; set; }
    string BranchNo { get; set; }
    string BranchDescription { get; set; }
    string RegionDescription { get; set; }
    int RegionNo { get; set; }
  }

  #endregion

  #region Coupon Issue Request Message

  [Serializable]
  public class CouponIssueRequestMessage : ICouponBaseMessage
  {
    public Guid CorrelationId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNo { get; set; }
    public string IDNo { get; set; }
    public string BranchNo { get; set; }
    public string BranchDescription { get; set; }
    public string RegionDescription { get; set; }
    public int RegionNo { get; set; }
  }

  #endregion

  #region Coupon Issue Request Message

  [Serializable]
  public class CouponIssueStartRequestMessage : ICouponBaseMessage
  {
    public Guid CorrelationId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNo { get; set; }
    public string IDNo { get; set; }
    public string BranchNo { get; set; }
    public string BranchDescription { get; set; }
    public string RegionDescription { get; set; }
    public int RegionNo { get; set; }
  }

  #endregion

  #region Coupon Issue Completed Request Message

  [Serializable]
  public class CouponIssueCompletedRequestMessage : ICouponBaseMessage
  {
    public Guid CorrelationId { get; set; }
    public int CampaignId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNo { get; set; }
    public string IDNo { get; set; }
    public string BranchNo { get; set; }
    public string BranchDescription { get; set; }
    public string RegionDescription { get; set; }
    public int RegionNo { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
  }

  #endregion

  #region Coupon Issue Ignore Request Message
  [Serializable]
  public class CouponIssueIgnoreRequestMessage : ICouponBaseMessage
  {
    public Guid CorrelationId { get; set; }
    public int CampaignId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CellNo { get; set; }
    public string IDNo { get; set; }
    public string BranchNo { get; set; }
    public string BranchDescription { get; set; }
    public string RegionDescription { get; set; }
    public int RegionNo { get; set; }
    public string Message { get; set; }
  }
  #endregion
}
