
using System;
using System.Collections.Generic;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class CheckAnyTemplatesMatchRequest
  {
    public CheckAnyTemplatesMatchRequest(byte[] template, ICollection<Tuple<int, byte[]>> templates, int securityLevel)
    {
      Template = template;
      Templates = templates;
      SecurityLevel = securityLevel;
    }

    public byte[] Template { get; set; }

    public ICollection<Tuple<int, byte[]>> Templates { get; set; }

    public int SecurityLevel { get; set; }

  }
}
