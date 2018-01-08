using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IRegion
  {
    long RegionId { get; set; }
    string Description { get; set; }
  }
}
