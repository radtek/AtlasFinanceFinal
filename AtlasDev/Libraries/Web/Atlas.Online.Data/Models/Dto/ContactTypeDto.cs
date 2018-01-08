using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class ContactTypeDto
  {
    public int ContactTypeId { get; set; }
    public Enumerators.General.ContactType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.General.ContactType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.General.ContactType>();
      }
    }
    public string Description { get; set; }

  }
}