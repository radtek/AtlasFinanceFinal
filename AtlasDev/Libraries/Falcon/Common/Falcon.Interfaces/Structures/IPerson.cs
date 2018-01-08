using System;
using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IPerson
  {
    long PersonId { get; set; }
    string Username { get; set; }
    //public PER_Security Security { get;set;}
    //public PER_Type PersonType { get;set;}
    IBranch Branch { get; set; }
    string LegacyClientCode { get; set; }
    string ClientCode { get; set; }
    string Designation { get; set; }
    string FullName { get; set; }
    string Firstname { get; set; }
    string Middlename { get; set; }
    string Lastname { get; set; }
    string Othername { get; set; }
    General.EthnicGroup EthnicGroup { get; set; }
    string Email { get; set; }
    string IdNum { get; set; }
    string SalaryFrequency { get; set; }
    string Gender { get; set; }
    string Race { get; set; }

    bool WebLinked { get; set; }
    string WebReference { get; set; }
    //Host Host { get;set;}
    DateTime DateOfBirth { get; set; }
  }
}