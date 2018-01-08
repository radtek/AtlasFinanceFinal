using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class NextOfKin : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int NextOfKinId { get; set; }
    [Indexed]
    [Persistent("ClientId")]
    public Client Client { get; set; }
    [Persistent("ApplicationId")]
    public Application Application { get; set; }
    [Persistent]
    [Indexed]
    public General.RelationType Relation { get; set; }
    [Persistent, Size(20)]
    public string FirstName { get; set; }
    [Persistent, Size(30)]
    public string Surname { get; set; }
    [Persistent, Size(50)]
    public string ContactNo { get; set; }

    public NextOfKin() : base() { }
    public NextOfKin(Session session) : base(session) { }
  }
}