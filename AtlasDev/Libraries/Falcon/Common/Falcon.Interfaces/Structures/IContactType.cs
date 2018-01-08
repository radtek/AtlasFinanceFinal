using System;
using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IContactType
  {
    Int64 ContactTypeId { get; set; }
    General.ContactType Type { get; set; }
    string Description { get; set; }
  }
}