using System;
using System.Collections.Generic;


namespace Atlas.ThirdParty.CS.Enquiry
{
  public sealed class Product
  {
    public string Description { get; set; }
    public string Outcome { get; set; }
    public List<Reason> Reasons { get; set; }
  }
}
