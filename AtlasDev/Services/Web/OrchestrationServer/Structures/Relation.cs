using Atlas.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Orchestration.Server.Structures
{
  public sealed class Relation
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public General.RelationType RelationType { get; set; }
    public List<Contact> Contacts { get; set; }
  }
}