using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Address : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int AddressId { get; set; }
    [Indexed]
    [Persistent("AddressTypeId")]
    public AddressType AddressType { get; set; }
    [Persistent("ClientId")]
    [Indexed]
    [Association("ClientAddress")]
    public Client Client { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }
    [Persistent, Size(6)]
    public string PostalCode { get; set; }
    public Province Province { get; set; }

    public Address() : base() { }
    public Address(Session session) : base(session) { }
  }
}