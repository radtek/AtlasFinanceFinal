using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Employer : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int EmployerId { get; set; }
		[Persistent("AddressId")]
		[Indexed]
		public Address Address { get; set; }
		[Indexed]
		[Persistent("IndustryId")]
		public Industry Industry { get; set; }
		[Persistent, Size(255)]
		public string Name { get;set;}
		[Persistent, Size(50)]
		public string ContactNo { get; set; }
		
    public Employer() : base() { }
		public Employer(Session session) : base(session) { }    
  }
}