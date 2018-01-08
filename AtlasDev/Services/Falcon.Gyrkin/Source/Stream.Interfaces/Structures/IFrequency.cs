using System;

namespace Stream.Framework.Structures
{
  public interface IFrequency
  {
    Int32 FrequencyId { get; set; }
    Enumerators.Stream.FrequencyType Type { get; set; }
    string Description { get; set; }
  }
}