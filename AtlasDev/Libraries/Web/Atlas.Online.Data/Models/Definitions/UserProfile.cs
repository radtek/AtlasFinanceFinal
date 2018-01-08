using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class UserProfile : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int UserId { get; set; }
    public string Email { get; set; }

    public UserProfile() : base() { }
    public UserProfile(Session session) : base(session) { }
  }
}