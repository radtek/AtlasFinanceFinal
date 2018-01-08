using System;

namespace Atlas.Domain.DTO
{
  public class ProductBatchDTO
  {
    public Int64 ProductBatchId { get; set; }
    public ProductTypeDTO ProductType { get; set; }
    public Enumerators.General.ProductBatchStatus Status { get; set; }
    public CPY_CompanyDTO Courier { get; set; }
    public string TrackingNum { get; set; }
    public BRN_BranchDTO DeliverToBranch { get; set; }
    public DateTime CapturedDT { get; set; }
    public DateTime? SentDT { get; set; }
    public DateTime? LastEditedDT { get; set; }
    public PER_PersonDTO CapturedBy { get; set; }
    public PER_PersonDTO SentBy { get; set; }
    public PER_PersonDTO LastEditedBy { get; set; }
    public int Quantity { get; set; }
    public string Comment { get; set; }
  }
}
