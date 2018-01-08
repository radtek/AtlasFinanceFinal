using System;
using System.Collections.Generic;


namespace Atlas.FP.Identifier.MessageTypes.PubSub
{
  public sealed class FPNewTemplates
  {

    public FPNewTemplates(ICollection<Tuple<Int64, int, byte[], int>> templates) // personid, finger, template, orientation: 0- normal, 1- upside down
    {
      Templates = templates;      
    }


    public ICollection<Tuple<long, int, byte[], int>> Templates { get; private set; }

  }
}
