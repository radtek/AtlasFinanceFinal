using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class AddressDto 
  {
    public int AddressId { get; set; }
    public AddressTypeDto AddressType { get; set; }
    public ClientDto Client { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    public string PostalCode { get; set; }
    public ProvinceDto Province { get; set; }

  }
}