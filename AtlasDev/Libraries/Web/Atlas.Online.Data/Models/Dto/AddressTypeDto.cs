using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class AddressTypeDto
  {
    public Int64 AddressTypeId { get; set; }
    public Enumerators.General.AddressType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.General.AddressType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.General.AddressType>();
      }
    }

    public string Description { get; set; }

  }
}