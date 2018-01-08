using Atlas.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Serializable.Product
{
  [Serializable]
  public class CellPhone
  {
    [SearchValue(1)]
    public string Make { get; set; }
    [SearchValue(2)]
    public string Model { get; set; }
  }
}
