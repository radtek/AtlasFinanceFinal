using System.Collections.Generic;

namespace Falcon.Common.Structures
{
  public sealed class Employer
  {
    public long CompanyId { get; set; }
    public string Name { get; set; }
    public int NoOfBranches { get; set; }
    public List<Contact> Contacts { get; set; }
    public List<Address> Addresses { get; set; }
  }
}
