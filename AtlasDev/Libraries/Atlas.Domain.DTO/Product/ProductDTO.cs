using System;

namespace Atlas.Domain.DTO
{
  public class ProductDTO
  {
    public Int64 ProductId { get; set; }
    public ProductBatchDTO ProductBatch { get; set; }
    public PER_PersonDTO AllocatedPerson { get; set; }
    public string XmlObject { get; set; }
    public string SearchValue1 { get; set; }
    public string SearchValue2 { get; set; }
    public PER_PersonDTO CreatedBy { get; set; }
    public DateTime CreatedDT { get; set; }
    public PER_PersonDTO ReceivedBy { get; set; }
    public DateTime? ReceivedDT { get; set; }
    public PER_PersonDTO AllocatedBy { get; set; }
    public DateTime? AllocatedDT { get; set; }
    public PER_PersonDTO LastEditedBy { get; set; }
    public DateTime? LastEditedDT { get; set; }
  }
}
