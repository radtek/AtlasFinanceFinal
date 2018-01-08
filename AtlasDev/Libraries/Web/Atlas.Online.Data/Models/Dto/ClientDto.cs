using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class ClientDto
  {
    public int ClientId { get; set; }
    public long UserId { get; set; }
    public long? PersonId { get; set; }
    public string Title { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public string IDNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public char Gender { get; set; }
    public bool OTPVerified { get; set; }
  }
}