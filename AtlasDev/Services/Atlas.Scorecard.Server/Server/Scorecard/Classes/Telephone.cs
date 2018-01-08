using System;


namespace Atlas.ThirdParty.CS.Enquiry
{
  [Serializable]
  public class Telephone
  {
    public string Type { get; set; }
    public string Number { get; set; }
    public DateTime? CreateDate { get; set; }
  }
}
