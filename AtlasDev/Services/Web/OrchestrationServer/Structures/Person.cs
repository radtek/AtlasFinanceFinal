using Atlas.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Orchestration.Server.Structures
{
  public sealed class Person
  {
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public General.Gender Gender { get; set; }
    public string IdNo { get; set; }
    public General.Host Host { get; set; }
    public string Email { get; set; }
    public Relation Relation { get; set; }
    public Employer Employer { get; set; }
    public BankDetail BankDetail { get; set; }
    public List<Contact> Contacts { get; set; }
    public List<Address> Addresses { get; set; }
  }
}
