using Atlas.Common.Extensions;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public class ContactType : IContactType
  {
    public long ContactTypeId { get; set; }

    public General.ContactType Type
    {
      get { return Description.FromStringToEnum<General.ContactType>(); }
      set { value = Description.FromStringToEnum<General.ContactType>(); }
    }

    public string Description { get; set; }
  }
}
