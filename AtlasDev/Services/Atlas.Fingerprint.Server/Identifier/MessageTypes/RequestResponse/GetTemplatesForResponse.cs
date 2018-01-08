using System;
using System.Collections.Generic;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class GetTemplatesForResponse
  {
    public GetTemplatesForResponse(ICollection<Tuple<int, byte[]>> templates )
    {
      Templates = templates;
    }


    // FingerId / Template
    public ICollection<Tuple<int, byte[]>> Templates { get; set; }

  }
}
