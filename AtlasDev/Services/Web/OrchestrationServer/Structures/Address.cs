using Atlas.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Orchestration.Server.Structures
{
  public sealed class Address
  {

    public string Line1 { get;set;}
    public string Line2 { get;set;}
    public string Line3 { get;set;}
    public string Line4 { get;set;}
    public string Code { get;set;}
    public General.Province Province { get;set;}
    public General.AddressType AddressType { get; set; }    
  }
}
