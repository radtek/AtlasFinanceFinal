using System;


namespace Atlas.FP.Identifier.MessageTypes.PubSub
{
  public sealed class FPDeleteTemplates
  {
    public FPDeleteTemplates(Int64 personId)
    {
      PersonId = personId;
    }

    public long PersonId { get; set; }

  }
}
