using System.Collections.Generic;

namespace Falcon.Common.Structures
{
  public class CompuScanProduct
  {
    public long BranchId { get; set; }
    public List<Product> Products { get; set; }

    public long EnquiryId { get; set; }
  }

  public class Product
  {
    public string Name { get; set; }
    public int Count { get; set; }
  }
}
