using System;
using Atlas.Ass.Framework.Structures;

namespace Atlas.Ass.Structures
{
  public class OldestArrears : IOldestArrears
  {
    public string LegacyBranchNumber { get; set; }
    public DateTime OldestArrearsDate { get; set; }
  }
}