using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Orchestration.Server.Structures
{
  public sealed class Employer
  {
    public string Name { get;set;}

    public List<Address> Addresses { get;set;}
    public List<Contact> Contacts { get;set;}

    
        
  }
}
