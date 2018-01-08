using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  [NonPersistent]
  public class BranchCompany
  {
    public Int64 BranchId { get; set; }
    public string BranchName { get; set; }
    public string BranchNo { get; set; }
  }
}
