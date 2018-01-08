using System;

namespace Atlas.Ass.Framework.Structures
{
  public interface IOldestArrears
  {
    string LegacyBranchNumber { get; set; }
    DateTime OldestArrearsDate { get; set; }
  }
}